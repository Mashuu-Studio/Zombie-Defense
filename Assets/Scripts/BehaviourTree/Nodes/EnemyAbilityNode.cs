using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbilityNode : BTDecoratorNode
{
    [SerializeField] private bool invisible;
    [SerializeField] private bool flight;
    EnemyObject enemy;
    protected override void OnStart()
    {
        enemy = context.gameObject.GetComponent<EnemyObject>();
        if (enemy) enemy.SetSpecialAbility(invisible, flight);
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (enemy)
        {
            child.Update();
            return State.Running;
        }
        else return State.Failure;
    }
}
