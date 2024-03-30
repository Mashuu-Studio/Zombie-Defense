using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineEnemyObject : EnemyObject, ICombineObject
{
    public bool IsCombined { get { return isCombined; } }
    private bool isCombined;
    public bool IsCombining { get { return isCombining; } }
    private bool isCombining;

    public bool CheckHPState { get { return Hp <= data.thresholdHp; } }

    CombineEnemyObject combineTarget = null;
    public bool DetectOtherObject()
    {
        // 방법 1. 특정 범위 내에 합체할 유닛이 있나 체크.
        // 방법 2. EnemyController로부터 enemy를 전부 받아온 뒤 ICombineObject를 가지고 있는지 체크. 가까운 유닛에게 접근

        // 이 때 이미 해당 유닛이 합체를 하고있는 상태라면 Detecting이 되지 않아야함.

        if (combineTarget != null) return true;

        foreach (var enemy in EnemyController.Instance.SpawnEnemies)
        {
            if (data.key != enemy.Data.key || enemy == this) continue;
            // 캐스트를 해서 캐스트가 된다면 combine할 수 있는 유닛임.
            CombineEnemyObject ce = (CombineEnemyObject)enemy;
            if (ce && !ce.IsCombined)
            {
                // 비어있고 거리가 10f 안쪽일 경우
                // 비어있지는 않지만 새로 찾은 유닛이 더 가까울 경우
                if ((combineTarget == null && Vector2.Distance(transform.position, ce.transform.position) < 7.5f)
                    || (combineTarget != null && Vector2.Distance(transform.position, combineTarget.transform.position)
                         > Vector2.Distance(transform.position, ce.transform.position)))
                {
                    combineTarget = ce;
                }
            }
        }

        return combineTarget != null;
    }

    public void Combine()
    {
        // 해당 위치까지 우선 이동.
        moveTarget = combineTarget.transform;
        SetPath();
        isCombined = true;

        combineTarget.Combined(true);
        // 두 유닛 모두 합체상태임을 세팅해주어야 함.
        StartCoroutine(Combining());
    }

    public IEnumerator Combining()
    {
        // 서로 만날 때까지 대기.
        while (Vector2.Distance(transform.position, combineTarget.transform.position) > 1f) yield return null;
        combineTarget.LookAt(transform.position);
        LookAt(combineTarget.transform.position);
        isCombining = true;
        // 만났다면 일정 시간동안 대기
        // 일정 시간 동안은 합체를 준비하는 단계.
        // 이 시간 안에 잡으면 보상을 온전히 받을 수 있음.
        float time = 0f;
        while (time < 1.5f && IsCombining)
        {
            if (!GameController.Instance.Pause) time += Time.deltaTime;
            // 타겟이 중간에 죽게 되면 다시 탐색 해야함.
            if (combineTarget.gameObject.activeSelf == false)
            {
                isCombining = false;
                isCombined = false;
                combineTarget = null;
                moveTarget = Player.Instance.transform;
                SetPath();
            }
            yield return null;
        }

        if (isCombining)
        {
            // 타겟의 체력을 해당 유닛의 남아있는 체력만큼 회복 후 사망
            combineTarget.ActivateBuff(new BuffInfo() { hp = Hp });
            Dead();
        }
    }

    public void Combined(bool b)
    {
        isCombined = b;
        isCombining = b;
    }

    public override void Dead()
    {
        base.Dead();
        if (combineTarget != null) combineTarget.Combined(false);
        combineTarget = null;
        isCombined = false;
        moveTarget = Player.Instance.transform;
    }
}
