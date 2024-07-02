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
        // remainSep�� �����ϴ� ���� �ƴ� ���� ����.
        this.remainSep = (remainSep != -1) ? remainSep : data.separate;
        // �и��� ����ŭ ü���� divide
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
        // ������ �ʴ� ���·� ����.
        Color color = InvisibleColor;
        if (isAttacking) animator.speed = ADelay;
        else animator.speed = 1;

        if (visible)
        {
            // ������ �ʴ� ���������� ���̴� ������ ��
            if (invisible) color = ScanedInvisibleColor;
            // �׳� ���̴� ������ ��
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
        // �Ӽ��� ���� ���� ��ġ�� �°� ������ ����.
        if (data.resistances.ContainsKey(attribute))
            dmg = (int)(dmg * data.resistances[attribute]);

        // ���º��� �������� ���ٸ� �������� 1�� ����.
        dmg -= Def;
        if (dmg < 0) dmg = 1;

        hp -= dmg;
        if (gameObject.activeSelf)
        {
            if (changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);
            changeColorCoroutine = ChangeColor(Color.red);
            StartCoroutine(changeColorCoroutine);
        }

        // ���
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
        // ���� ������ �������� �� Failure�� ����� ��. (���� ��� ���̳� �÷��̾�)
        // �� �ܿ� ������ �پ����� ���� �̵��ϴ� ��ĵ� ����� �ʿ䰡 ����.
        // ���� ��� ������ �پ��ְ� ���� ���� ���� ���̶�� �̵��� �� �ʿ䰡 ����.
        // �ٸ� �� ���� �������ݿ� ����. ���Ÿ����� �����̶�� �տ� �ٰŸ� ������ ���� ���� �����Ƿ�
        // ������ ������ ���� ������ �ؾ��� ���� ����.
        // �̷� ��� ��Ȳ�� ���� ó���� �ʿ���.

        // ���� ���� ������ ���� Player�� �ֺ��� �ִ��� üũ
        // �ִٸ� �ش� ������ Ÿ������ ����
        // ���ٸ� Building�̶� �ִ��� üũ
        // �ִٸ� ���� ù �ͷ��� Ÿ������ ����.

        // �������̶�� �ش� Ÿ���� ������ ���� ������ üũ
        // �װ� �ƴ϶�� 0.75������ �ȿ� �ֳ� üũ

        // �������� �� ���� �ִϸ��̼��� ��â �������̶�� ��ŵ.
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
        // ��Ž���� �ƴ���, �� �� �� �ִ� ��尡 �ִ��� Ȯ��.
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
