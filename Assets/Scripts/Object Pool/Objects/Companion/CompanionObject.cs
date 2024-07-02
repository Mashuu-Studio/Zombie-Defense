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

    private int hp;
    private int maxhp;
    private int def;
    private int maxdef;
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
        def = maxdef = data.def;
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
        // ���� ����� ���ٸ� ��ŵ
        if (weapon != null && weapon.key == key) return false;

        Weapon w = WeaponManager.GetWeapon(key);
        if (w.infAmount || Player.Instance.ItemAmount(key) > 0)
        {
            // �ٸ��ٸ� ������ ���� ����� ��ü
            if (weapon != null && key != weapon.key)
            {
                // ����ǰ���� ���ư�.
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
    public PatrolType PType { get { return patrolType; } }
    private PatrolType patrolType;
    private bool move;
    private Poolable movePoint;
    public float Speed { get { return speed * (1 + ActivatedBuff.speed); } }


    private List<Vector2> holdPatrolPosList;
    private int patrolIndex;

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
        // ���̶� ��ġ�� ������ �ִٸ� �н�
        foreach (var pos in list)
        {
            if (MapGenerator.PosOnWall(pos)) return;
        }

        patrolType = PatrolType.HOLD;

        AdjustMove(false);
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }

        transform.position = list[0];
        holdPatrolPosList = list;
        patrolIndex = 0;
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

                // ȸ������ 0�� �� �������� �ٶ󺸰� ����.
                // �̸� �������� y���� ��ȯ���� ��ġ�� ��������.
                // �� �� Player�� �������� ȸ������ ��ġ�� ����.
                // ��, �׻� Player �տ� �����ؾ��ϹǷ� ĳ���Ͱ� ���� ��ȯ��Ű�� ������ �������־�� ��.
                case PatrolType.LEAD:
                    x = 2f;
                    y = Random.Range(-2.5f, 2.5f);
                    targetPos +=
                        (Vector2)(Quaternion.Euler(
                            new Vector3(0, 0, Player.Instance.transform.rotation.eulerAngles.z - 90))
                        * new Vector2(x, y));
                    break;

                case PatrolType.BACK:
                    x = -2f;
                    y = Random.Range(-2.5f, 2.5f);
                    targetPos +=
                        (Vector2)(Quaternion.Euler(
                            new Vector3(0, 0, Player.Instance.transform.rotation.eulerAngles.z - 90))
                        * new Vector2(x, y));
                    break;

                case PatrolType.HOLD:
                    patrolIndex = (patrolIndex + 1) % 2;
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
            // ������ �Դٸ� Ÿ�̸� �۵�.
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

            // �̵� ���⿡ ������ �浹ü�� ���ٸ� �̵� ����.
            // �̵��� ��ġ�� OverlapCircleAll�� ������� �浹ü�� �ִ��� üũ.
            var amount = Time.fixedDeltaTime * Speed;
            int layermask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Companion"));
            var hits = Physics2D.OverlapCircleAll(rigidbody.position + dir * amount, collider.radius, layermask);
            bool movable = true;
            foreach (var hit in hits)
            {
                if (hit != null && hit.isTrigger == false)
                {
                    movable = false;
                    break;
                }
            }
            if (movable) rigidbody.MovePosition(rigidbody.position + dir * amount);

            if (moveSoundTime > Player.MOVE_SOUND_TIME)
            {
                SoundController.Instance.PlaySFX(transform, "CHARACTER.MOVE");
                moveSoundTime = 0;
            }
            int end = IMovingObject.EndOfPath(rigidbody.position, next, dir * Speed * Time.fixedDeltaTime, collider.radius);
            if (end != -1)
            {
                // ���� �����ٸ� ���� ��ȯ
                if (patrolType == PatrolType.HOLD && end == 1)
                {
                    patrolIndex = (patrolIndex + 1) % 2;
                    movePoint.transform.position = holdPatrolPosList[patrolIndex];
                    SetPath();
                }
                else pathIndex++;
            }
        }
    }

    private IEnumerator PatrolTimer()
    {
        // ���� �ð� �Ŀ� �ٽ� ��Ʈ�� ����
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
    public int Def { get { return def; } }
    public int MaxDef { get { return maxdef; } }


    IEnumerator changeColorCoroutine;
    public void Heal()
    {
        hp = maxhp;
    }

    public void FillArmor()
    {
        def = maxdef;
    }

    public void Damaged(int dmg, ObjectData.Attribute attribute = ObjectData.Attribute.NONE)
    {
        dmg = IDamagedObject.Armoring(dmg, ref def);
        hp -= dmg;
        SoundController.Instance.PlaySFX(transform, "CHARACTER.DAMAGED", true);

        if (changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);
        changeColorCoroutine = ChangeColor(Color.red);
        if (gameObject.activeSelf) StartCoroutine(changeColorCoroutine);

        if (hp < 0) Dead();
    }

    public void Dead()
    {
        spriteRenderer.material.SetColor("_Color", Color.white);
        StopAllCoroutines();
        ClearBuff();
        PoolController.Push("Movepoint", movePoint);
        CompanionController.Instance.RemoveCompanion(this);
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
        // ��Ʈ�� �ʱ�ȭ
        AdjustMove(false);
        // �켱������ ���� ���� �����ϴ� �ڵ尡 �� ����
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
        // �ܼ� ȸ���� ��� ��� �ߵ�
        if (buff.IsHeal)
        {
            if (buff.hp > 0) Heal(buff.hp);
            //if (buff.def > 0) RefillArmor(buff.def);
        }
        else
        {
            if (buffs.ContainsKey(buff))
            {
                StopCoroutine(buffs[buff]);
                buffs[buff] = BuffTimer(buff);
            }
            else buffs.Add(buff, BuffTimer(buff));

            if (gameObject.activeSelf) StartCoroutine(buffs[buff]);
        }
    }

    public IEnumerator BuffTimer(BuffInfo buff)
    {
        float time = 0;
        int count = 1;
        while (time < buff.time)
        {
            if (!GameController.Instance.Pause) time += Time.deltaTime;
            if (buff.IsDotDamage && time >= count * buff.delay)
            {
                count++;
                Damaged(buff.hp);
            }
            yield return null;
        }
        buffs.Remove(buff);
    }
    public void ClearBuff()
    {
        foreach (var buff in buffs)
        {
            StopCoroutine(buff.Value);
        }
        buffs.Clear();
    }
    #endregion
    private IEnumerator reloadCoroutine;
    public void Reload()
    {
        // Magazine�� �� �������ٸ� �������� ��ȯ
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

