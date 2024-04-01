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

        // 기존 방식과 통합이 가능한지 체크
        // layer를 조절하는 등의 방식을 사용하면 괜찮게 합칠 수 있을 것 같음.

        targetCollider = null;
        targetIsTurret = true;
        meleeAttack = true;
        FindTargets(meleeRange, .8f, 1 << LayerMask.NameToLayer("Turret"));

        // 원거리 공격이 있을 때 근접 공격 대상을 찾지 못했다면
        // 원거리 기반으로 탐색 시작
        if (data.siegeRadius > 0 && targetCollider == null
            && FindTargets(Range, .8f, 1 << LayerMask.NameToLayer("Turret")).Length > 0)
        {
            meleeAttack = false;
        }

        // 터렛을 아예 찾지 못했다면 근접 범위에서 플레이어 탐색
        if (targetCollider == null
            && FindTargets(meleeRange, .75f, 1 << LayerMask.NameToLayer("Player")).Length > 0)
        {
            targetIsTurret = false;
        }

        if (targetCollider == null) isAttacking = false;
        else LookAt(targetCollider.transform.position);

        return targetCollider != null;
    }

    public override void Attack()
    {
        AdjustMove(false);
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
