using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestEnemyObject : EnemyObject, IRestObject
{
    public bool CheckHPState { get { return Hp <= data.thresholdHp; } }
    public bool IsArrived { get { return aiPath.canMove && EndOfPath(); } }
    public bool IsRunningAway { get { return isRunningAway; } }
    public bool IsHealed { get { return isHealed; } }

    private bool isRunningAway;
    private bool isHealed;

    private Poolable runawayPoint;

    public override void SetData(Enemy data, int remainSep = -1)
    {
        base.SetData(data, remainSep);
        runawayPoint = PoolController.Pop("MOVEPOINT");
        aiDestinationSetter.target = Player.Instance.transform;
    }

    public void Rest()
    {
        // 이동 중지, 회복 시작
        isRunningAway = false;
        isHealed = true;
        aiPath.canMove = false;
        StartCoroutine(Resting());
    }

    public void Runaway()
    {
        isRunningAway = true;
        // 도망치는 위치는 플레이어로부터 반대의 위치로 이동.
        Vector3 dir = transform.position - Player.Instance.transform.position;
        dir = dir.normalized;
        runawayPoint.transform.position = transform.position + dir * 10;
        aiDestinationSetter.target = runawayPoint.transform;
        aiPath.canMove = true;
        aiPath.SearchPath();
    }

    int pathCount = 0;
    float lastRemainDistance;
    public bool EndOfPath()
    {
        if (Mathf.Abs(aiPath.remainingDistance - lastRemainDistance) < 0.03f) pathCount++;
        else pathCount = 0;
        lastRemainDistance = aiPath.remainingDistance;

        return aiPath.reachedEndOfPath || pathCount > 1 / Time.deltaTime;
    }

    IEnumerator Resting()
    {
        float time = 0;
        while (Hp < data.hp)
        {
            while (time < 1f)
            {
                if (!GameController.Instance.Pause) time += Time.deltaTime;
                yield return null;
            }
            time -= 1f;
            Heal(data.restHealAmount);
        }
        // 회복이 끝났다면 다시 플레이어를 찾으러 감.
        aiDestinationSetter.target = Player.Instance.transform;
        isHealed = false;
    }

    public override void Dead()
    {
        base.Dead();
        PoolController.Push("MOVEPOINT", runawayPoint);
        isHealed = false;
        isRunningAway = false;
    }
}
