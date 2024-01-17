using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTRepeatNode : BTDecoratorNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        child.Update();
        return State.Running;
    }
}