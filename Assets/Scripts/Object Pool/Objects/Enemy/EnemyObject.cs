using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("Poolable/Enemy (Poolable)")]
public class EnemyObject : BTPoolable,
    IDamagedObject, IAttackObject, IMovingObject, IBuffTargetObject
{
    [Space]
    [SerializeField] private ObjectHpBar hpBar;

    [Space]
    [SerializeField] protected Rigidbody2D rigidbody;
    [SerializeField] protected SpriteRenderer spriteRenderer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected EnemyHitBox hitbox;
    [SerializeField] protected CircleCollider2D collider;

    [Space]
    [SerializeField] protected Pathfinding.Seeker seeker;

    public Enemy Data { get { return data; } }
    protected Enemy data;

    protected int hp;
    protected int maxhp;

    protected int dmg;
    protected int def;
    protected float range;
    protected float aDelay;
    protected float speed;

    protected int money;

    protected bool flight;
    protected bool invisible;
    protected bool visible;

    protected int remainSep;

    public void SetSpecialAbility(bool inv, bool fly)
    {
        invisible = inv;
        visible = !inv;
        flight = fly;
    }

    public void SetVisible(bool b)
    {
        visible = b;
    }

    public virtual void SetData(Enemy data, int remainSep = -1)
    {
        this.data = data;

        hitbox.Init(this);

        visible = !invisible;
        // remainSep를 이전하는 경우와 아닌 경우로 구분.
        this.remainSep = (remainSep != -1) ? remainSep : data.separate;
        // 분리된 수만큼 체력을 divide
        maxhp = hp = (int)(data.hp * GameController.Instance.Difficulty.hp / Mathf.Pow(2, data.separate - this.remainSep));
        hpBar.SetHpBar(this, maxhp);

        speed = data.speed;
        dmg = data.dmg;
        range = data.range;
        aDelay = data.adelay;

        money = data.money;

        WaitAttack = false;

        moveTarget = Player.Instance.transform;
        StartCoroutine(DetectingPath());
    }

    private Color currentColor;
    private static Color VisibleColor = new Color(1, 1, 1, 1);
    private static Color InvisibleColor = new Color(1, 1, 1, 0);
    private static Color ScanedInvisibleColor = new Color(1, 1, 1, .5f);

    public override void Update()
    {
        base.Update();
        // 보이지 않는 상태로 세팅.
        Color color = InvisibleColor;
        if (isAttacking) animator.speed = ADelay;
        else animator.speed = 1;

        if (visible)
        {
            // 보이지 않는 유닛이지만 보이는 상태일 때
            if (invisible) color = ScanedInvisibleColor;
            // 그냥 보이는 유닛일 때
            else if (!invisible) color = VisibleColor;
        }
        SetColor(color);
    }

    private void FixedUpdate()
    {
        if (!gameObject.activeSelf || GameController.Instance.Pause) return;
        if (move) Move();
    }

    private void SetColor(Color color)
    {
        if (currentColor == color) return;
        currentColor = color;
        spriteRenderer.color = color;
        hpBar.gameObject.SetActive(visible);
    }

    #region IDamagedObject
    IEnumerator changeColorCoroutine;
    public int Hp { get { return hp; } }
    public int Def { get { return def + ActivatedBuff.def; } }

    public void Heal(int amount)
    {
        hp += amount;
        if (hp > maxhp) hp = maxhp;

        if (changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);
        changeColorCoroutine = ChangeColor(Color.green);
        StartCoroutine(changeColorCoroutine);
    }

    public void Damaged(int dmg, ObjectData.Attribute attribute = ObjectData.Attribute.NONE)
    {
        // 속성에 따른 저항 수치에 맞게 데미지 감소.
        if (data.resistances.ContainsKey(attribute))
            dmg = (int)(dmg * data.resistances[attribute]);

        // 방어력보다 데미지가 높다면 데미지는 1로 고정.
        dmg -= Def;
        if (dmg < 0) dmg = 1;

        hp -= dmg;
        if (gameObject.activeSelf)
        {
            if (changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);
            changeColorCoroutine = ChangeColor(Color.red);
            StartCoroutine(changeColorCoroutine);
        }

        // 사망
        if (hp <= 0)
        {
            if (remainSep > 0)
            {
                remainSep--;
                SetData(data, remainSep);

                Vector3 sepAmount = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * Vector2.right / 2;

                EnemyObject sepObject = EnemyController.Instance.AddEnemy(data, transform.position + sepAmount);
                transform.position -= sepAmount;

                sepObject.SetData(data, remainSep);
            }
            else
            {
                string deadSfx = Data.key + ".DEAD";
                if (SoundController.Instance.ContainsSFX(deadSfx) == false) deadSfx = "ENEMY.DEAD";
                SoundController.Instance.PlaySFX(transform, deadSfx, true);

                if (Data.dropItem != null)
                {
                    int rand = Random.Range(0, 100) + 1;
                    if (rand <= Data.dropItem.prob) ItemObject.Drop(transform.position, Data.dropItem);
                }

                Player.Instance.GetReward(money);
                Dead();
            }
        }
    }

    public virtual void Dead()
    {
        ClearBuff();
        StopAllCoroutines();
        EnemyController.Instance.DeadEnemy(this);
        spriteRenderer.material.SetColor("_Color", Color.white);
        PoolController.Push(data.key, this);
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

    protected bool isAttacking { get { return animator.GetCurrentAnimatorStateInfo(0).IsTag("ATTACK"); } }
    protected Collider2D targetCollider;
    public Collider2D TargetCollider { get { return targetCollider; } }
    public int Dmg { get { return (int)((dmg + ActivatedBuff.dmg) * GameController.Instance.Difficulty.dmg); } }
    public float Range { get { return range; } }
    public float ADelay { get { return aDelay * (1 + ActivatedBuff.aspeed); } }
    public bool WaitAttack { get; set; }
    private Vector2 targetPos;

    public virtual bool DetectTarget()
    {
        // 가는 방향이 막혀있을 때 Failure를 띄워야 함. (예를 들어 벽이나 플레이어)
        // 그 외에 적끼리 붙어있을 때도 이동하는 방식도 고민할 필요가 있음.
        // 예를 들어 적끼리 붙어있고 앞의 적이 공격 중이라면 이동을 할 필요가 없음.
        // 다만 이 경우는 근접공격에 한함. 원거리공격 유닛이라면 앞에 근거리 유닛이 없을 수도 있으므로
        // 앞으로 비집고 들어가서 공격을 해야할 수도 있음.
        // 이런 모든 상황에 대한 처리가 필요함.

        // 가장 기초 세팅은 먼저 Player가 주변에 있는지 체크
        // 있다면 해당 유닛을 타겟으로 세팅
        // 없다면 Building이라도 있는지 체크
        // 있다면 가장 첫 터렛을 타겟으로 세팅.

        // 공격중이라면 해당 타겟이 실질적 공격 범위로 체크
        // 그게 아니라면 0.75사이즈 안에 있나 체크

        // 공격중일 때 아직 애니메이션이 한창 진행중이라면 스킵.
        if (isAttacking
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f) return true;

        float range = Range;
        float ratio = 0.75f;
        if (animator.GetBool("attack")) ratio = 1f;
        FindTarget(range, ratio, 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Companion"));
        if (targetCollider == null)
        {
            int layerMask = 1 << LayerMask.NameToLayer("Building");
            FindTarget(range, ratio, layerMask);
        }

        animator.SetBool("attack", targetCollider != null);
        return targetCollider != null;
    }

    protected bool FindTarget(float range, float detectRatio, int layerMask)
    {
        range *= detectRatio;
        targetCollider = Physics2D.OverlapCircle(transform.position, range, layerMask);

        return targetCollider != null;
    }

    public virtual void Damaging(GameObject target)
    {
        if (Data.debuff != null)
        {
            IBuffTargetObject buffTargetObject = target.transform.parent.GetComponent<IBuffTargetObject>();
            if (buffTargetObject != null) buffTargetObject.ActivateBuff(Data.debuff);
        }
        IDamagedObject damagedObject = target.transform.parent.GetComponent<IDamagedObject>();
        damagedObject.Damaged(Dmg);
    }
    public void RangeAttack()
    {
        string attackSfxName = Data.key + ".ATTACK";
        if (SoundController.Instance.ContainsSFX(attackSfxName) == false) attackSfxName = "ENEMY.ATTACK";
        SoundController.Instance.PlaySFX(transform, attackSfxName, true);

        string projName = Data.key.Replace("ENEMY", "PROJECTILE");
        var proj = (EnemyProjectile)PoolController.Pop(projName);
        var summonUnit = data.projSummon ? data.summonUnit : "";
        proj.SetProj(transform.position, targetPos, transform.rotation.eulerAngles.z,
            Data.isSiege, Dmg, data.projSpeed, Data.debuff, summonUnit, Data.summonProb);
    }

    public virtual void Attack()
    {
        AdjustMove(false);
    }

    public void LookAtTarget()
    {
        if (targetCollider != null)
        {
            targetPos = targetCollider.transform.position;
            LookAt(targetPos);
        }
    }

    public void LookAt(Vector3 target)
    {
        Vector2 dir = target - transform.position;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(0, 0, degree + 90);
    }
    #endregion

    #region IMovingObject
    public float Speed { get { return speed * (1 + ActivatedBuff.speed); } }
    protected Transform moveTarget;
    protected List<Pathfinding.GraphNode> path;
    private int pathIndex;
    private bool move;

    private IEnumerator DetectingPath()
    {
        while (true)
        {
            float time = 0;
            while (time < .5f)
            {
                if (!GameController.Instance.Pause) time += Time.deltaTime;
                yield return null;
            }
            while (!seeker.IsDone()) yield return null;
            SetPath();
        }
    }

    protected void SetPath()
    {
        path = seeker.StartPath(rigidbody.position, moveTarget.position).path;
        pathIndex = 1;
    }

    public virtual bool DetectPath()
    {
        if (isAttacking) return false;
        // 길탐색이 됐는지, 더 갈 수 있는 노드가 있는지 확인.
        return seeker.IsDone() && path != null && path.Count - 1 > pathIndex;
    }

    public void AdjustMove(bool b)
    {
        move = b;
    }

    private Vector2 next;
    private Vector2 dir;
    public void Move()
    {
        if (seeker.IsDone() && path.Count > pathIndex)
        {
            next = IMovingObject.GetPos(path[pathIndex].position);
            dir = (next - rigidbody.position).normalized;
            LookAt(next);
            rigidbody.MovePosition(rigidbody.position + dir * Speed * Time.fixedDeltaTime);
            if (IMovingObject.EndOfPath(rigidbody.position, next, dir * Speed * Time.fixedDeltaTime, collider.radius) != -1) pathIndex++;
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

    public void ActivateBuff(BuffInfo buff)
    {
        // 단순 회복일 경우 즉시 발동
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
        while (time < buff.time)
        {
            if (!GameController.Instance.Pause) time += Time.deltaTime;
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
}
