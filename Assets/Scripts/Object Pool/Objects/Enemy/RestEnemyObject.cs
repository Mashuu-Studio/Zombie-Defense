using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestEnemyObject : EnemyObject, IRestObject
{
    public bool CheckHPState { get { return Hp <= data.thresholdHp; } }
    // 길이 없다면 도착한 것임.
    public bool IsArrived { get { return !DetectPath(); } }
    public bool IsRunningAway { get { return isRunningAway; } }
    public bool IsHealed { get { return isHealed; } }

    private bool isRunningAway;
    private bool isHealed;

    private Poolable runawayPoint;

    public override void SetData(Enemy data, int remainSep = -1)
    {
        base.SetData(data, remainSep);
        runawayPoint = PoolController.Pop("Movepoint");
        moveTarget = Player.Instance.transform;
    }

    public void Rest()
    {
        // 이동 중지, 회복 시작
        isRunningAway = false;
        isHealed = true;
        AdjustMove(false);
        StartCoroutine(Resting());
    }

    public void Runaway()
    {
        // 도망치는 위치는 플레이어로부터 반대의 위치로 이동.
        Vector3 dir = transform.position - Player.Instance.transform.position;
        dir = dir.normalized;
        runawayPoint.transform.position = transform.position;
        for (int i = 0; i < 10; i++)
        {
            // 반대위치로 이동할 때 맵 밖이라면 해당위치에서 스탑.
            Vector2Int nextPos = MapGenerator.RoundToInt(runawayPoint.transform.position + dir);
            if (MapGenerator.PosOnMap(MapGenerator.ConvertToMapPos(nextPos))) runawayPoint.transform.position += dir;
            else break;
        }
        moveTarget = runawayPoint.transform;
        SetPath();
        isRunningAway = true;
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
        moveTarget = Player.Instance.transform;
        isHealed = false;
    }

    public override void Dead()
    {
        base.Dead();
        PoolController.Push("Movepoint", runawayPoint);
        isHealed = false;
        isRunningAway = false;
    }
}
