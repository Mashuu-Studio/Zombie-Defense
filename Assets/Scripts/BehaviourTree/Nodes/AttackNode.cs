using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackNode : BTActionNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (context.attackObject == null) return State.Failure;

        // 탐색에 성공했다면 공격딜레이 동안에는 running
        // 딜레이가 아니라면 공격 시행 및 성공 리턴
        if (context.attackObject.DetectTarget())
        {
            if (context.attackObject.WaitAttack) return State.Running;

            context.attackObject.Attack();
            return State.Success;
        }
        return State.Failure;
    }
}
