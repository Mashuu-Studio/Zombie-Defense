using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestNode : BTActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        // 도착했다면 휴식 시작.
        if (context.restObject.IsRunningAway)
        {
            if (context.restObject.IsArrived) context.restObject.Rest();
            else context.movingObject.Move();
            return State.Running;
        }

        if (context.restObject.IsHealed) return State.Running;

        // 체력을 체크해서 특정 수치 이하가 되면 도망침
        if (context.restObject.CheckHPState)
        {
            context.restObject.Runaway();
            return State.Success;
        }
        return State.Failure;
    }
}
