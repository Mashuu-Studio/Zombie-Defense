using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Turret (Poolable)")]
public class TurretObject : Poolable, IDamagedObject
{
    int hp;
    int range;
    int dmg;
    float adelay;
    int speed;
    bool waitAttack;

    //private BehaviourTree bt;

    public void Init(Turret data)
    {
        hp = data.hp;
        range = data.range;
        dmg = data.dmg;
        adelay = data.adelay;
        speed = data.speed;
        waitAttack = false;
    }
    /*
    private void Start()
    {
        bt = new BehaviourTree(SetBT());
    }

    private void Update()
    {
        if (dmg > 0 && !waitAttack) bt.Operate();
    }

    private IBTNode SetBT()
    {
        return new SequenceNode(new List<IBTNode>()
        {
            new ActionNode(DetectEnemy),
            new ActionNode(SelectEnemy),
            new ActionNode(AttackEnemy)
        });
    }

    Collider2D[] targets;
    Transform target;

    private IBTNode.NodeState DetectEnemy()
    {
        targets = Physics2D.OverlapCircleAll(transform.position, range, 1 << LayerMask.NameToLayer("Enemy"));

        if (targets == null || targets.Length == 0) return IBTNode.NodeState.Failure;
        return IBTNode.NodeState.Success;
    }

    private IBTNode.NodeState SelectEnemy()
    {
        // 우선순위에 따라 적을 선택하는 코드가 들어갈 예정
        target = targets[0].transform;

        return IBTNode.NodeState.Success;
    }

    private IBTNode.NodeState AttackEnemy()
    {
        if (!waitAttack)
        {
            Vector2 dir = target.position - transform.position;
            ((Bullet)PoolController.Pop("Bullet")).SetBullet(transform.position, dir, dmg, speed);
            StartCoroutine(AttackTimer());
        }
        return IBTNode.NodeState.Success;
    }*/

    IEnumerator AttackTimer()
    {
        waitAttack = true;
        float time = 0;
        while (time < adelay)
        {
            time += Time.deltaTime;
            yield return null;
        }
        waitAttack = false;
    }

    public void Damaged(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            PoolController.Push(gameObject.name, this);
            StopAllCoroutines();
        }
    }
}
