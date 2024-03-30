using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : TurretObject, IAttackObject
{
    public Collider2D TargetCollider { get; }

    public int Dmg { get { return data.dmg; } }

    public float Range { get { return data.range; } }

    public float ADelay { get { return data.adelay; } }

    public bool WaitAttack { get; set; }

    private Collider2D[] targets;
    public bool DetectTarget()
    {
        if (WaitAttack) return false;

        targets = Physics2D.OverlapBoxAll(transform.position, Vector2.one, 0, 1 << LayerMask.NameToLayer("Enemy"));
        return targets.Length > 0;
    }

    public void Attack()
    {
        // 공격 시에 내구도 1씩 차감.
        if (!WaitAttack)
        {
            foreach (var target in targets)
            {
                ActivateTrap(target);
            }
            Damaged(1);
            StartCoroutine(AttackTimer());
        }
    }

    private void ActivateTrap(Collider2D target)
    {
        if (Dmg > 0)
        {
            IDamagedObject damagedObject = target.transform.parent.GetComponent<IDamagedObject>();
            damagedObject.Damaged(Dmg);
        }

        if (data.buff != null)
        {
            IBuffTargetObject buffTarget = target.transform.parent.GetComponent<IBuffTargetObject>();
            buffTarget.ActivateBuff(data.buff);
        }
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

    public override void Damaged(int dmg)
    {
        // 공격 받을 때에는 무조건 1의 데미지를 받음.
        dmg = 1;
        base.Damaged(dmg);
    }
}
