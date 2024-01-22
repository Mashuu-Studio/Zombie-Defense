using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Enemy (Poolable)")]
public class EnemyObject : BTPoolable, IDamagedObject, IAttackObject, IMovingObject
{
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Pathfinding.AIPath aiPath;
    [SerializeField] private Pathfinding.AIDestinationSetter aIDestinationSetter;

    private int hp;
    private Collider2D targetCollider;
    private int dmg;
    private float range;
    private float aDelay;
    private bool isAttacking;
    private int speed;

    private int exp;
    private int money;

    private void Start()
    {
        // 후에 player를 관리하는 컨트롤러를 통해서 받아오면 좋을 듯.
        aIDestinationSetter.target = Player.Instance.transform;
    }

    public void Init(Enemy data)
    {
        hp = data.hp;
        aiPath.maxSpeed = speed = data.speed;
        dmg = data.dmg;
        range = data.range;
        aDelay = data.adelay;

        exp = data.exp;
        money = data.money;
    }

    public override void Update()
    {
        base.Update();
        if (GameController.Instance.Pause) aiPath.canMove = false;
    }

    #region IDamagedObject
    public int Hp { get { return hp; } }
    public void Damaged(int dmg)
    {
        hp -= dmg;
        StartCoroutine(ChangeColor());
        if (hp <= 0)
        {
            Player.Instance.Reward(exp, money);
            PoolController.Push(gameObject.name, this);
            spriteRenderer.color = Color.green;
            StopAllCoroutines();
        }
    }

    IEnumerator ChangeColor()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.green;
    }
    #endregion

    #region IAttackObject
    public Collider2D TargetCollider { get { return targetCollider; } }
    public int Dmg { get { return dmg; } }
    public float Range { get { return range; } }
    public float ADelay { get { return aDelay; } }
    public bool WaitAttack { get; set; }

    public bool DetectTarget()
    {
        aiPath.canMove = false;
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
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range, 1 << LayerMask.NameToLayer("Turret"));
            if (cols != null && cols.Length > 0) targetCollider = cols[0];
        }

        if (targetCollider == null) isAttacking = false;

        return targetCollider != null;
    }

    public void Attack()
    {
        isAttacking = true;
        if (!WaitAttack)
        {
            IDamagedObject damagedObject = targetCollider.GetComponent<IDamagedObject>();
            damagedObject.Damaged(dmg);
            StartCoroutine(AttackTimer());
        }
    }

    public IEnumerator AttackTimer()
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
    public int Speed { get { return speed; } }
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
        direction = direction.normalized;
        rigidbody.rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        aiPath.canMove = true;
    }

    #endregion
}
