using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Attacking Turret (Poolable)")]
public class AttackTurretObject : TurretObject, IAttackObject
{
    private Collider2D targetCollider;
    private int dmg;
    private float range;
    private float aDelay;
    private int speed;
    private bool isAttacking;

    public Collider2D TargetCollider { get { return targetCollider; } }
    public int Dmg { get { return dmg; } }
    public float Range { get { return range; } }
    public float ADelay { get { return aDelay; } }
    public bool WaitAttack { get; set; }

    public override void Init(Turret data)
    {
        base.Init(data);
        dmg = data.dmg;
        range = data.range;
        aDelay = data.adelay;
        speed = data.speed;
    }

    Collider2D[] targets;
    Transform target;
    public bool DetectTarget()
    {
        targets = Physics2D.OverlapCircleAll(transform.position, range, 1 << LayerMask.NameToLayer("Enemy"));
        return targets != null && targets.Length > 0;
    }

    public void Attack()
    {
        // 우선순위에 따라 적을 선택하는 코드가 들어갈 예정
        target = targets[0].transform;

        if (!WaitAttack)
        {
            Vector2 dir = target.position - transform.position;
            ((Bullet)PoolController.Pop("Bullet")).SetBullet(transform.position, dir, dmg, speed);
            StartCoroutine(AttackTimer());
        }
    }

    public IEnumerator AttackTimer()
    {
        WaitAttack = true;
        float time = 0;
        while (time < ADelay)
        {
            time += Time.deltaTime;
            yield return null;
        }
        WaitAttack = false;
    }
}
