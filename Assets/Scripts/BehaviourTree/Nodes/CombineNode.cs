using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineNode : BTActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (context.combineObject.IsCombined) return State.Success;

        if (context.combineObject.CheckHPState && context.combineObject.DetectOtherObject())
        {
            context.combineObject.Combine();
            return State.Success;
        }
        return State.Failure;
    }
}
