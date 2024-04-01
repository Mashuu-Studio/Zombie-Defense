using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNode : BTActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (context.movingObject == null) return State.Failure;

        // 탐색에 성공했다면 공격딜레이 동안에는 running
        // 딜레이가 아니라면 공격 시행 및 성공 리턴
        if (context.movingObject.DetectPath())
        {
            context.movingObject.AdjustMove(true);
            return State.Success;
        }
        return State.Failure;
    }
}
