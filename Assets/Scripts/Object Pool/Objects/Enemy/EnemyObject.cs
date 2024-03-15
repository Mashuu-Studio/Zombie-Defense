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
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Space]
    [SerializeField] private Pathfinding.AIPath aiPath;
    [SerializeField] private Pathfinding.AIDestinationSetter aIDestinationSetter;

    Enemy data;

    private int hp;
    private int maxhp;
    private Collider2D targetCollider;
    private int dmg;
    private int def;
    private float range;
    private float aDelay;
    private bool isAttacking;
    private int speed;

    private int exp;
    private int money;

    private bool flight;
    private bool invisible;
    private bool visible;

    private int remainSep;

    private void Start()
    {
        aIDestinationSetter.target = Player.Instance.transform;
    }

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

        visible = !invisible;
        // remainSep를 이전하는 경우와 아닌 경우로 구분.
        this.remainSep = (remainSep != -1) ? remainSep : data.separate;
        // 분리된 수만큼 체력을 divide
        maxhp = hp = (int)(data.hp / Mathf.Pow(2, data.separate - this.remainSep));
        Vector2 spriteSize = new Vector2(spriteRenderer.sprite.rect.width, spriteRenderer.sprite.rect.height) / spriteRenderer.sprite.pixelsPerUnit;
        hpBar.SetHpBar(maxhp, new Vector2(spriteSize.x * 3 / 2, 0.25f), spriteSize.y * 3 / 4);

        speed = data.speed;
        dmg = data.dmg;
        range = data.range;
        aDelay = data.adelay;

        exp = data.exp;
        money = data.money;

        WaitAttack = false;
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
        if (visible)
        {
            // 보이지 않는 유닛이지만 보이는 상태일 때
            if (invisible) color = ScanedInvisibleColor;
            // 그냥 보이는 유닛일 때
            else if (!invisible) color = VisibleColor;
        }
        SetColor(color);

        if (GameController.Instance.Pause) aiPath.canMove = false;
    }

    private void SetColor(Color color)
    {
        if (currentColor == color) return;
        currentColor = color;
        spriteRenderer.color = color;
        hpBar.gameObject.SetActive(visible);
    }

    #region IDamagedObject
    public int Hp { get { return hp; } }
    public int Def { get { return def + ActivatedBuff.def; } }

    public void Heal(int amount)
    {
        hp += amount;
        if (hp > maxhp) hp = maxhp;
        hpBar.UpdateHpBar(hp);
    }

    public void Damaged(int dmg)
    {
        // 방어력보다 데미지가 높다면 데미지는 1로 고정.
        dmg -= Def;
        if (dmg < 0) dmg = 1;

        hp -= dmg;
        hpBar.UpdateHpBar(hp);
        if (gameObject.activeSelf) StartCoroutine(ChangeColor());
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
                int rand = Random.Range(0, 2);
                if (rand == 0) Item.Drop(transform.position);

                Player.Instance.GetReward(exp, money);
                Dead();
            }
        }
    }

    public void Dead()
    {
        PoolController.Push(gameObject.name, this);
        //spriteRenderer.color = Color.green;
        buffs.Clear();
        StopAllCoroutines();
        EnemyController.Instance.DeadEnemy(this);
    }

    IEnumerator ChangeColor()
    {
        //spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        //spriteRenderer.color = Color.green;
    }
    #endregion

    #region IAttackObject
    public Collider2D TargetCollider { get { return targetCollider; } }
    public int Dmg { get { return dmg + ActivatedBuff.dmg; } }
    public float Range { get { return range; } }
    public float ADelay { get { return aDelay * (1 + ActivatedBuff.aspeed); } }
    public bool WaitAttack { get; set; }

    public bool DetectTarget()
    {
        // 가는 방향이 막혀있을 때 Failure를 띄워야 함. (예를 들어 벽이나 플레이어)
        // 그 외에 적끼리 붙어있을 때도 이동하는 방식도 고민할 필요가 있음.
        // 예를 들어 적끼리 붙어있고 앞의 적이 공격 중이라면 이동을 할 필요가 없음.
        // 다만 이 경우는 근접공격에 한함. 원거리공격 유닛이라면 앞에 근거리 유닛이 없을 수도 있으므로
        // 앞으로 비집고 들어가서 공격을 해야할 수도 있음.
        // 이런 모든 상황에 대한 처리가 필요함.

        // 가장 기초 세팅은 먼저 Player가 주변에 있는지 체크
        // 있다면 해당 유닛을 타겟으로 세팅
        // 없다면 Turret이라도 있는지 체크
        // 있다면 가장 첫 터렛을 타겟으로 세팅.

        // 공격중이라면 해당 타겟이 실질적 공격 범위로 체크
        // 그게 아니라면 0.75사이즈 안에 있나 체크

        float range = this.range;
        if (!isAttacking) range *= .75f;

        targetCollider = Physics2D.OverlapCircle(transform.position, range, 1 << LayerMask.NameToLayer("Player"));
        if (targetCollider == null)
        {
            int turretLayer = 1 << LayerMask.NameToLayer("Turret");
            // 원거리 공격의 경우 Trap을 공격할 수 있음.
            if (Range >= 3f) turretLayer |= 1 << LayerMask.NameToLayer("Trap");
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range, turretLayer);
            if (cols != null && cols.Length > 0) targetCollider = cols[0];
        }

        if (targetCollider == null) isAttacking = false;
        else
        {
            LookAt(targetCollider.transform.position);
            aiPath.canMove = false;
        }

        return targetCollider != null;
    }

    public void Attack()
    {
        isAttacking = true;
        if (!WaitAttack)
        {
            IDamagedObject damagedObject = targetCollider.transform.parent.GetComponent<IDamagedObject>();
            damagedObject.Damaged(Dmg);
            StartCoroutine(AttackTimer());
        }
    }

    private void LookAt(Vector3 target)
    {
        Vector2 dir = target - transform.position;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(0, 0, degree - 90);
    }

    private IEnumerator AttackTimer()
    {
        WaitAttack = true;
        float time = 0;
        while (time < ADelay)
        {
            if (!GameController.Instance.Pause) time += Time.deltaTime;
            yield return null;
        }
        WaitAttack = false;
    }
    #endregion

    #region IMovingObject
    public float Speed { get { return speed * ( 1 + ActivatedBuff.speed); } }
    private Vector3 direction;
    private float moveAmount;
    private bool isMove;
    public bool DetectPath()
    { 
        // 우선 path를 무조건 찾는다고 가정.
        return true;
    }

    public void Move()
    {
        aiPath.maxSpeed = Speed;
        direction = direction.normalized;
        rigidbody.rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        aiPath.canMove = true;
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
}
