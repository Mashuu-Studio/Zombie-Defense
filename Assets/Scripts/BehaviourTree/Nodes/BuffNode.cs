using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffNode : BTActionNode
{
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (context.buffObject == null) return State.Failure;

        // 버프 준비 상태가 아니고 탐색에 성공했다면 버프 시전.
        if (!context.buffObject.WaitBuff && context.buffObject.DetectBuffTarget())
        {
            context.buffObject.GiveBuff();
            return State.Success;
        }
        return State.Failure;
    }
}
