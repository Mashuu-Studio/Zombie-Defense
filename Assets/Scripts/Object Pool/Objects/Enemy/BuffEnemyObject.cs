using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEnemyObject : EnemyObject, IBuffObject
{
    public BuffInfo Buff { get { return buff; } }
    private BuffInfo buff;

    private float buffRange;

    public bool WaitBuff { get; set; }
    public float BuffRange { get { return buffRange; } }
    public float BDelay { get { return Buff.delay; } }

    private List<IBuffTargetObject> buffTargets = new List<IBuffTargetObject>();

    public override void SetData(Enemy data)
    {
        base.SetData(data);
        buff = data.buff;
        buffRange = 3; // 임시세팅
        WaitBuff = false;
    }

    public bool DetectBuffTarget()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, buffRange, 1 << LayerMask.NameToLayer("Enemy"));
        buffTargets.Clear();
        foreach (var col in cols)
        {
            var target = col.transform.parent.GetComponent<IBuffTargetObject>();
            if (target != null)
            {
                // 광역버프일 경우 대상을 가리지 않고 버프 부여
                if (Buff.area) buffTargets.Add(target);
                // 단일버프일 경우 버프가 없는 유닛에게만 버프 부여 및 반복문 탈출
                else if (!target.BuffIsActivated)
                {
                    buffTargets.Add(target);
                    break;
                }
            }
        }
        return buffTargets.Count > 0;
    }

    public void GiveBuff()
    {
        foreach (var target in buffTargets)
        {
            target.ActivateBuff(buff);
        }
        StartCoroutine(GiveBuffTimer());
    }

    public IEnumerator GiveBuffTimer()
    {
        WaitBuff = true;
        float time = 0;
        while (time < BDelay)
        {
            if (!GameController.Instance.Pause) time += Time.deltaTime;
            yield return null;
        }
        WaitBuff = false;
    }
}
