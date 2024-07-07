using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SiegeEnemyObject : EnemyObject
{
    private static float meleeRange = 2.5f;
    private bool meleeAttack;
    private bool targetIsBuilding;
    public override bool DetectTarget()
    {
        // �켱 �ٰŸ� ���� ���� ������ Ÿ���� �ִ��� üũ
        // �� �� ���Ÿ� ���� ���� ������ Ÿ���� �ִ��� üũ
        // �� �Ŀ����� �÷��̾� Ž��.
        // �� �� Ʈ���� ���� �������� ����. Ʈ���� ���Ÿ� ���� �������� �μ��� ����.

        // �������̶�� �ش� Ÿ���� ������ ���� ������ üũ
        // �װ� �ƴ϶�� 0.8������ �ȿ� �ֳ� üũ

        // ���� ��İ� ������ �������� üũ
        // layer�� �����ϴ� ���� ����� ����ϸ� ������ ��ĥ �� ���� �� ����.

        // �������� �� ���� �ִϸ��̼��� ��â �������̶�� ��ŵ.
        if (isAttacking
            && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f) return true;

        targetCollider = null;
        targetIsBuilding = true;
        meleeAttack = true;

        float ratio = 0.8f;
        if (animator.GetBool("attack")) ratio = 1f;

        int layerMask = 1 << LayerMask.NameToLayer("Building");
        FindTarget(meleeRange, ratio, layerMask);

        if (targetCollider == null
            && FindTarget(Range, ratio, layerMask))
        {
            meleeAttack = false;
        }

        // �ͷ��� �ƿ� ã�� ���ߴٸ� ���� �������� �÷��̾� Ž��
        layerMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Companion");
        if (targetCollider == null && FindTarget(meleeRange, ratio, layerMask))
        {
            meleeAttack = true;
            targetIsBuilding = false;
        }
        animator.SetBool("melee", meleeAttack);
        animator.SetBool("attack", targetCollider != null);

        return targetCollider != null;
    }

    public override void Damaging(GameObject target)
    {
        int dmg = (int)(Dmg * (targetIsBuilding ? 1.5f : 1f));
        IDamagedObject damagedObject = target.transform.parent.GetComponent<IDamagedObject>();
        damagedObject.Damaged(dmg);
    }
}
