using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegeEnemyObject : EnemyObject
{
    private static float meleeRange = 2.5f;
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

        float ratio = 0.8f;
        if (animator.GetBool("attack")) ratio = 1f;
        FindTargets(meleeRange, ratio, 1 << LayerMask.NameToLayer("Turret"));

        if (targetCollider == null
            && FindTargets(Range, ratio, 1 << LayerMask.NameToLayer("Turret")).Length > 0)
        {
            meleeAttack = false;
        }

        // 터렛을 아예 찾지 못했다면 근접 범위에서 플레이어 탐색
        if (targetCollider == null
            && FindTargets(meleeRange, ratio, 1 << LayerMask.NameToLayer("Player")).Length > 0)
        {
            meleeAttack = true;
            targetIsTurret = false;
        }
        animator.SetBool("melee", meleeAttack);
        animator.SetBool("attack", targetCollider != null);

        return targetCollider != null;
    }

    public override void Damaging(GameObject target)
    {
        int dmg = (int)(Dmg * (targetIsTurret ? 1.5f : 1f));
        IDamagedObject damagedObject = target.transform.parent.GetComponent<IDamagedObject>();
        damagedObject.Damaged(dmg);
    }
}
