using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionObject : BTPoolable,
    IMovingObject, IDamagedObject, IAttackObject, IBuffTargetObject
{
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private CircleCollider2D collider;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject shootingPoint;
    [SerializeField] private BoxCollider2D autoTargetCollider;
    [SerializeField] protected Pathfinding.Seeker seeker;

    public string Key { get { return key; } }
    private string key;

    private int maxhp;
    private int hp;
    private int def;
    private float speed = 5;

    private Weapon weapon;
    private bool reloading;
    public Weapon UsingWeapon { get { return weapon; } }

    private float moveSoundTime = Player.MOVE_SOUND_TIME;
    public override void Update()
    {
        base.Update();
        moveSoundTime += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!gameObject.activeSelf || GameController.Instance.Pause) return;
        if (move) Move();
    }

    public void Summon(string key)
    {
        this.key = key;
        var data = CompanionManager.GetCompanion(key);
        hp = maxhp = data.hp;
        def = data.def;
        speed = data.speed;
        reloading = false;
        SetBasicWeapon();

        movePoint = PoolController.Pop("Movepoint");
    }

    public void SetBasicWeapon()
    {
        ChangeWeapon("WEAPON.PISTOL");
    }

    public bool ChangeWeapon(string key)
    {
        Weapon w = WeaponManager.GetWeapon(key);
        if (w.infAmount || Player.Instance.ItemAmount(key) > 0)
        {
            // 다르다면 기존에 가진 무기와 교체
            if (weapon != null && key != weapon.key)
            {
                // 소지품으로 돌아감.
                Player.Instance.AdjustItemAmount(weapon.key, 1);
            }
            weapon = new Weapon(w);

            float weaponParam = 0;
            List<Weapon> weapons = WeaponManager.GetWeapons();
            for (int i = 0; i < weapons.Count; i++)
            { 
                if (weapons[i].key == weapon.key)
                {
                    weaponParam = (float)i / (weapons.Count - 1);
                    break;
                }
            }
            animator.SetFloat("weapon", weaponParam);
            if (w.infAmount == false) Player.Instance.AdjustItemAmount(key, -1);
            return true;
        }
        return false;
    }

    #region IMovingObject
    public enum PatrolType { NARROWLY = 0, WIDELY, LEAD, BACK, HOLD, }
    private PatrolType patrolType;
    private bool move;
    private Poolable movePoint;
    public float Speed { get { return speed * (1 + ActivatedBuff.speed); } }


    private List<Vector2> holdPatrolPosList;
    private int patrolIndex;
    private bool patrolForward;

    protected List<Pathfinding.GraphNode> path;
    private int pathIndex;

    public void SetPatrolType(PatrolType type)
    {
        patrolType = type;
        if (type == PatrolType.HOLD)
            SetHoldPatrol(new List<Vector2>() { transform.position });
    }

    public void SetHoldPatrol(List<Vector2> list)
    {
        // 벽이랑 겹치는 구간이 있다면 패스
        foreach (var pos in list)
        {
            if (MapGenerator.PosOnWall(pos)) return;
        }

        patrolType = PatrolType.HOLD;

        AdjustMove(false);
        transform.position = list[0];
        holdPatrolPosList = list;
        patrolIndex = 0;
        patrolForward = true;
    }

    public bool DetectPath()
    {
        if (patrolCoroutine != null) return false;
        if (!move)
        {
            Vector2 targetPos = Player.Instance.transform.position;

            float x, y;
            switch (patrolType)
            {
                case PatrolType.NARROWLY:
                    x = Random.Range(1, 2f);
                    y = Random.Range(1, 2f);

                    x *= (Random.Range(1, 3) % 2 == 0) ? 1 : -1;
                    y *= (Random.Range(1, 3) % 2 == 0) ? 1 : -1;

                    targetPos += new Vector2(x, y);
                    break;

                case PatrolType.WIDELY:
                    x = Random.Range(3, 4f);
                    y = Random.Range(3, 4f);

                    x *= (Random.Range(1, 3) % 2 == 0) ? 1 : -1;
                    y *= (Random.Range(1, 3) % 2 == 0) ? 1 : -1;

                    targetPos += new Vector2(x, y);
                    break;

                // 회전각이 0일 때 오른쪽을 바라보고 있음.
                // 이를 기준으로 y값만 변환시켜 위치를 지정해줌.
                // 이 후 Player를 기준으로 회전시켜 위치를 잡음.
                // 단, 항상 Player 앞에 존재해야하므로 캐릭터가 각을 변환시키면 빠르게 조정해주어야 함.
                case PatrolType.LEAD:
                    x = 2f;
                    y = Random.Range(-2.5f, 2.5f);
                    targetPos += (Vector2)(Quaternion.Euler(Player.Instance.transform.rotation.eulerAngles) * new Vector2(x, y));
                    break;

                case PatrolType.BACK:
                    x = -2f;
                    y = Random.Range(-2.5f, 2.5f);
                    targetPos += (Vector2)(Quaternion.Euler(Player.Instance.transform.rotation.eulerAngles) * new Vector2(x, y));
                    break;

                case PatrolType.HOLD:
                    if (patrolForward) patrolIndex++;
                    else patrolIndex--;

                    if (patrolIndex >= holdPatrolPosList.Count)
                    {
                        patrolIndex--;
                        patrolForward = false;
                    }
                    else if (patrolIndex < 0)
                    {
                        patrolIndex++;
                        patrolForward = true;
                    }
                    targetPos = holdPatrolPosList[patrolIndex];
                    break;
            }
            movePoint.transform.position = targetPos;
            SetPath();
        }
        return true;
    }

    private void SetPath()
    {
        path = seeker.StartPath(rigidbody.position, movePoint.transform.position).path;
        pathIndex = 1;
    }

    private IEnumerator patrolCoroutine;

    public void AdjustMove(bool b)
    {
        move = b;
    }

    public void Move()
    {
        if (seeker.IsDone())
        {
            // 끝까지 왔다면 타이머 작동.
            if (path.Count <= pathIndex)
            {
                AdjustMove(false);
                patrolCoroutine = PatrolTimer();
                StartCoroutine(patrolCoroutine);
                return;
            }
            var next = IMovingObject.GetPos(path[pathIndex].position);
            var dir = (next - rigidbody.position).normalized;
            LookAt(next);
            rigidbody.position += dir * Speed * Time.fixedDeltaTime;
            if (moveSoundTime > Player.MOVE_SOUND_TIME)
            {
                SoundController.Instance.PlaySFX(transform.position, "CHARACTER.MOVE");
                moveSoundTime = 0;
            }
        if (IMovingObject.EndOfPath(rigidbody.position, next, dir * Speed * Time.fixedDeltaTime, collider.radius)) pathIndex++;
        }
    }

    private IEnumerator PatrolTimer()
    {
        // 일정 시간 후에 다시 패트롤 진행
        float time = 0;
        while (time < 0.2f)
        {
            if (!GameController.Instance.Pause) time += Time.deltaTime;
            yield return null;
        }
        patrolCoroutine = null;
    }

    #endregion
    #region IDamagedObject
    public int Hp { get { return hp; } }
    public int MaxHp { get { return maxhp; } }
    public int Def { get { return def + ActivatedBuff.def; } }


    IEnumerator changeColorCoroutine;
    public void Heal()
    {
        hp = maxhp;
    }

    public void Damaged(int dmg, ObjectData.Attribute attribute = ObjectData.Attribute.NONE)
    {
        hp -= dmg;
        SoundController.Instance.PlaySFX(transform.position, "CHARACTER.DAMAGED");

        if (changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);
        changeColorCoroutine = ChangeColor(Color.red);
        StartCoroutine(changeColorCoroutine);

        if (hp < 0)
        {
            StopAllCoroutines();
            PoolController.Push("Movepoint", movePoint);
            CompanionController.Instance.RemoveCompanion(this);
        }
    }
    IEnumerator ChangeColor(Color color)
    {
        Color reverse = Color.white - color;
        float time = 0.2f;
        while (time > 0)
        {
            if (!GameController.Instance.Pause)
            {
                spriteRenderer.material.SetColor("_Color", color);
                time -= Time.deltaTime;
                color += reverse * Time.deltaTime * 5;
            }
            yield return null;
        }
        spriteRenderer.material.SetColor("_Color", Color.white);
    }
    #endregion
    #region IAttackObject
    public int Dmg { get { return (weapon != null) ? weapon.dmg : 0; } }
    public float Range { get { return (weapon != null) ? weapon.range : 0; } }
    public float ADelay { get { return (weapon != null) ? weapon.adelay : 0; } }
    public bool WaitAttack { get { return weapon != null ? weapon.Wait : false; } }

    public Collider2D TargetCollider { get { return targetCollider; } }
    private Collider2D targetCollider;

    Collider2D[] targets;
    Transform target;
    public bool DetectTarget()
    {
        if (weapon == null || reloading) return false;

        targets = Physics2D.OverlapCircleAll(transform.position, Range / 2, 1 << LayerMask.NameToLayer("Enemy"));
        return targets.Length > 0;
    }

    public void LookAt(Vector3 target)
    {
        Vector2 dir = target - transform.position;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(0, 0, degree + 90);
    }

    public void Attack()
    {
        // 패트롤 초기화
        AdjustMove(false);
        // 우선순위에 따라 적을 선택하는 코드가 들어갈 예정
        target = targets[0].transform;

        if (!WaitAttack)
        {
            if (weapon.curammo <= 0)
            {
                Reload();
                return;
            }
            else if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
                reloadCoroutine = null;
            }

            LookAt(target.transform.position);
            animator.SetTrigger("fire");
            weapon.Fire(shootingPoint.transform.position, target.position, transform.rotation.eulerAngles.z);

            StartCoroutine(weapon.AttackDelay());
        }
    }
    #endregion
    #region IBuffTargetObject
    public List<BuffInfo> Buffs { get { return buffs.Keys.ToList(); } }
    private Dictionary<BuffInfo, IEnumerator> buffs = new Dictionary<BuffInfo, IEnumerator>();
    public BuffInfo ActivatedBuff
    {
        get
        {
            BuffInfo buff = new BuffInfo();
            if (Buffs != null) Buffs.ForEach(b => buff += b);
            return buff;
        }
    }
    public void Heal(int amount)
    {
        hp += amount;
        if (hp > maxhp) hp = maxhp;
    }

    public void ActivateBuff(BuffInfo buff)
    {
        // 단순 회복일 경우 즉시 발동
        if (buff.IsHeal) Heal(buff.hp);
        else
        {
            if (buffs.ContainsKey(buff))
            {
                StopCoroutine(buffs[buff]);
                buffs[buff] = BuffTimer(buff);
            }
            else buffs.Add(buff, BuffTimer(buff));

            StartCoroutine(buffs[buff]);
        }
    }

    public IEnumerator BuffTimer(BuffInfo buff)
    {
        float time = 0;
        while (time < buff.time)
        {
            if (!GameController.Instance.Pause) time += Time.deltaTime;
            yield return null;
        }
        buffs.Remove(buff);
    }
    #endregion
    private IEnumerator reloadCoroutine;
    public void Reload()
    {
        // Magazine이 다 떨어졌다면 권총으로 변환
        if (!Player.Instance.HasMagazine(weapon.key))
        {
            SetBasicWeapon();
            return;
        }
        if (reloading) return;
        reloadCoroutine = weapon.Reloading();
        StartCoroutine(reloadCoroutine);
    }
}

