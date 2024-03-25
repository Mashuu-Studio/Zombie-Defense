using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegeEnemyObject : EnemyObject
{
    private static float meleeRange = 1.5f;
    private bool meleeAttack;
    private bool targetIsTurret;
    public override bool DetectTarget()
    {
        // 우선 근거리 범위 내에 공성할 타워가 있는지 체크
        // 이 후 원거리 범위 내에 공성할 타워가 있는지 체크
        // 그 후에서야 플레이어 탐색.
        // 이 때 트랩은 굳이 공격하지 않음. 트랩은 원거리 범위 공격으로 부수는 느낌.

        // 공격중이라면 해당 타겟이 실질적 공격 범위로 체크
        // 그게 아니라면 0.8사이즈 안에 있나 체크

        targetCollider = null;

        int turretLayer = 1 << LayerMask.NameToLayer("Turret");
        targetIsTurret = true;
        float range = meleeRange;
        if (!isAttacking) range *= .8f;
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range, turretLayer);
        if (cols != null && cols.Length > 0)
        {
            targetCollider = cols[0];
            meleeAttack = true;
        }

        if (data.siegeRadius > 0)
        {
            // 밀리 공격을 찾지 못함.
            if (targetCollider == null)
            {
                range = Range;
                if (!isAttacking) range *= .8f;
                cols = Physics2D.OverlapCircleAll(transform.position, range, turretLayer);
                if (cols != null && cols.Length > 0)
                {
                    targetCollider = cols[0];
                    meleeAttack = false;
                }
            }
        }

        // 공성 터렛을 여전히 찾지 못함.
        if (targetCollider == null)
        {
            range = meleeRange;
            if (!isAttacking) range *= .8f;
            targetCollider = Physics2D.OverlapCircle(transform.position, range, 1 << LayerMask.NameToLayer("Player"));
            if (targetCollider != null) targetIsTurret = false;
        }

        if (targetCollider == null) isAttacking = false;
        else
        {
            LookAt(targetCollider.transform.position);
            aiPath.canMove = false;
        }

        return targetCollider != null;
    }

    public override void Attack()
    {
        // 원거리 공격과 근거리 공격을 구분하여 작동시킴.
        isAttacking = true;
        // 해당 부분 공격 함수의 위치 조정.
        if (!WaitAttack)
        {
            IDamagedObject damagedObject = targetCollider.transform.parent.GetComponent<IDamagedObject>();
            if (meleeAttack)
            {
                int dmg = (int)(Dmg * (targetIsTurret ? 1.5f : 1f));
                damagedObject.Damaged(dmg);
            }
            else
            {
                // 원거리 공격은 범위 공격으로 돌같은 투사체를 던지는 방식.
                var proj = (Projectile)PoolController.Pop("Projectile");
                proj.SetProj(transform.position, targetCollider.transform.position, Dmg, data.siegeRadius, 10);
            }
            StartCoroutine(AttackTimer());
        }
    }
}
