using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Enemy (Poolable)")]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyObject : Poolable, IDamagedObject
{
    [SerializeField] private GameObject target;

    private Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;
    private BehaviourTree bt;

    private int hp;
    private int speed;
    private int dmg;
    private float radius;
    private float adelay;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        bt = new BehaviourTree(SetBT());
    }

    public void Init(Enemy data)
    {
        hp = data.hp;
        speed = data.speed;
        dmg = data.dmg;
        radius = data.range;
        adelay = data.adelay;
    }

    private void Update()
    {
        bt.Operate();
    }

    public void Damaged(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
            PoolController.Push(gameObject.name, this);
    }

    #region BT
    private IBTNode SetBT()
    {
        return new SelectorNode(
            new List<IBTNode>()
            {
                new SequenceNode(new List<IBTNode>()
                {
                    new ActionNode(Detect),
                    new ActionNode(Attack),
                }
                ),
                new SequenceNode(new List<IBTNode>()
                {
                    new ActionNode(CheckMove),
                    new ActionNode(Move),
                }
                ),
            }
            );
    }
    #region Attack

    bool isAttacking;
    Collider2D targetCollider;
    bool waitAttack;
    private IBTNode.NodeState Detect()
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

        float range = radius;
        if (!isAttacking) range *= .75f;

        targetCollider = Physics2D.OverlapCircle(transform.position, range, 1 << LayerMask.NameToLayer("Player"));
        if (targetCollider != null) return IBTNode.NodeState.Success;
        else
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range, 1 << LayerMask.NameToLayer("Turret"));
            if (cols != null && cols.Length > 0)
            {
                targetCollider = cols[0];
                return IBTNode.NodeState.Success;
            }
        }

        isAttacking = false;
        return IBTNode.NodeState.Failure;
    }

    private IBTNode.NodeState Attack()
    {
        isAttacking = true;
        if (!waitAttack)
        {
            IDamagedObject damagedObject = targetCollider.GetComponent<IDamagedObject>();
            damagedObject.Damaged(dmg);
            StartCoroutine(AttackTimer());
        }
        return IBTNode.NodeState.Success;
    }

    IEnumerator AttackTimer()
    {
        spriteRenderer.color = Color.red;
        waitAttack = true;
        float time = 0;
        while (time < adelay)
        {
            time += Time.deltaTime;
            yield return null;
            spriteRenderer.color = Color.yellow;
        }
        waitAttack = false;
    }
    #endregion
    #region Move
    private Vector3 direction;
    private float moveAmount;
    private IBTNode.NodeState CheckMove()
    {
        List<Vector2Int> path = MapGenerator.Instance.FindPath(transform.position);
        moveAmount = Time.deltaTime * speed;

        if (path.Count > 1) // 처음에는 자신의 위치가 기본적으로 들어감.
        {
            /* 우선 다음 도착지까지의 남은 거리를 체크함.
             * path[1] - curPos = 방향.
             * path[1] - pos = 남은 거리
             * moveAmount = 프레임당 이동거리
             * 남은 거리 < 이동거리 일 경우 다음 이동 경로를 체크해야함.
             */

            int nextDestination = 1;
            float remainDistance;
            direction = (path[nextDestination] - (Vector2)transform.position);

            /* path[0]는 기본적으로 자신의 위치라고 세팅된 자리임.
             * 다만, 위치 체크 방식으로 인해 해당 위치에 도달하지 못했을 수 있음.
             * 따라서 path[1]까지의 방향을 확인한 뒤
             * 해당 이동 방향이 path[0]까지의 방향과 반대라면 path[1]로 세팅해주는 방식으로 진행.
             */

            Vector2 checkDirection = (path[0] - (Vector2)transform.position);
            if (direction.x * checkDirection.x <= 0 && direction.y * checkDirection.y <= 0)
            {
                // 반대방향이라면 path[1]로 방향을 세팅해 줌.
                direction = direction.normalized;
                remainDistance = Vector2.Distance(path[1], transform.position);
            }
            else
            {
                // 아니라면 path[0]로 방향을 세팅해 줌.
                nextDestination = 0;
                direction = checkDirection.normalized;
                remainDistance = Vector2.Distance(path[0], transform.position);
            }

            // 이동량이 남은 거리보다 많을 때
            // 적을 때는 세팅된 방향으로 세팅된 이동량만큼 이동하면 됨.
            if (remainDistance < moveAmount)
            {
                if (path.Count > nextDestination + 1) // 다음 목적지가 최종 도착지가 아님
                {
                    /* 다음 목적지로 가는 방향을 체크
                     * 이 후 최종적으로 도착할 곳을 확인
                     * 기존 목적지에서 남은 이동량만큼 다음 목적지 방향으로 이동한 위치
                     * 해당 위치에 최종적으로 도착할 수 있도록 방향과 이동량을 세팅.
                     * 해당 위치에서 현재 위치를 빼는 것으로 방향 세팅.
                     * magnitude를 통해 이동량 세팅.
                     */
                    Vector2 nextDirection = path[nextDestination + 1] - path[nextDestination];
                    Vector2 finalDestination = path[nextDestination] + nextDirection.normalized * remainDistance;

                    direction = finalDestination - (Vector2)transform.position;
                    moveAmount = direction.magnitude;
                    direction = direction.normalized;
                }
                else // 다음 목적지가 최종 도착지임.
                {
                    // 최종 목적지에 도착할 수 있도록 세팅.
                    moveAmount = remainDistance;
                }
            }
            return IBTNode.NodeState.Success;
        }
        // 이동할 곳이 없다면 Failure
        return IBTNode.NodeState.Failure;
    }

    private IBTNode.NodeState Move()
    {
        spriteRenderer.color = Color.green;
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg, Vector3.forward);
        rigidbody.MovePosition(transform.position + direction * moveAmount);
        return IBTNode.NodeState.Success;
    }
    #endregion
    #endregion

    private void OnDrawGizmos()
    {
        if (MapGenerator.Instance == null) return;
        List<Vector2Int> path = MapGenerator.Instance.FindPath(transform.position);

        if (path.Count > 0)
        {
            for (int i = 0; i < path.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector3(path[i].x, path[i].y), new Vector3(path[i + 1].x, path[i + 1].y));
            }
        }
    }
}
