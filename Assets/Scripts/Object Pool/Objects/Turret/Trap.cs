using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : BuildingObject, IAttackObject
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
            string particleName = gameObject.name.Replace("BUILDING", "PARTICLE");
            if (particleName != gameObject.name)
            {
                var particle = PoolController.Pop(particleName);
                if (particle != null)
                {
                    particle.transform.position = transform.position;
                    ((ParticleObject)particle).Play(0, Data.range);
                }
            }

            // 범위가 있다면 타겟 재설정 후 공격
            if (Data.range > 0) targets = Physics2D.OverlapCircleAll(transform.position, Data.range / 2, 1 << LayerMask.NameToLayer("Enemy"));

            foreach (var target in targets)
            {
                ActivateTrap(target);
            }

            Damaged(1);
            if (gameObject.activeSelf) StartCoroutine(AttackTimer());
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

    public override void Damaged(int dmg, ObjectData.Attribute attribute = ObjectData.Attribute.NONE)
    {
        // 공격 받을 때에는 무조건 1의 데미지를 받음.
        dmg = 1;
        base.Damaged(dmg);
    }
}
