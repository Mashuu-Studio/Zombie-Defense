using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonNode : BTActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (context.summonObject == null) return State.Failure;

        if (context.summonObject.CanSummon && context.summonObject.DetectTarget())
        {
            context.summonObject.Summon();
            return State.Success;
        }
        return State.Failure;
    }
}
