using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTree
{
    public IBTNode rootNode;

    public BehaviourTree(IBTNode root)
    {
        rootNode = root;
    }

    public void Operate()
    {
        rootNode.Evaluate();
    }
}

public interface IBTNode
{
    public enum NodeState
    {
        Running,
        Success,
        Failure,
    }

    public NodeState Evaluate();
}

public sealed class ActionNode : IBTNode
{
    Func<IBTNode.NodeState> onUpdate = null;

    public ActionNode(Func<IBTNode.NodeState> onUpdate)
    {
        this.onUpdate = onUpdate;
    }
    // ?? 연산자는 null일 경우 오른쪽을 반환함.
    // ?. 연산자 역시 마찬가지. null이 아닐 경우에 Invoke를 호출. 그게 아니라면 null을 리턴함.
    // onUpdate가 null이라면 ?? 왼쪽이 null이므로 Failure 아니라면 Invoke함.
    // onUpdate가 없다면 무조건 Failure지만 Invoke하게 되면 상태가 바뀌는
    public IBTNode.NodeState Evaluate() => onUpdate?.Invoke() ?? IBTNode.NodeState.Failure;
}

public sealed class SelectorNode : IBTNode
{
    List<IBTNode> actions;

    public SelectorNode(List<IBTNode> actions)
    {
        this.actions = actions;
    }

    public IBTNode.NodeState Evaluate()
    {
        if (actions == null) return IBTNode.NodeState.Failure;

        foreach (var action in actions)
        {
            // Selector의 경우 진행할 수 있다면 다른 노드는 체크하지 않기 떄문에
            // Failure만 아니라면 return해도 괜찮음.
            IBTNode.NodeState result = action.Evaluate();
            if (result != IBTNode.NodeState.Failure) return result;
        }

        return IBTNode.NodeState.Failure;
    }
}

public sealed class SequenceNode : IBTNode
{
    List<IBTNode> actions;

    public SequenceNode(List<IBTNode> actions)
    {
        this.actions = actions;
    }

    // Sequence의 경우에는 Success가 날 때 까지 Running을 계속 진행하고 있어야 함.
    // 
    public IBTNode.NodeState Evaluate()
    {
        if (actions == null) return IBTNode.NodeState.Failure;

        foreach (var action in actions)
        {
            IBTNode.NodeState result = action.Evaluate();
            switch (result)
            {
                case IBTNode.NodeState.Running: return IBTNode.NodeState.Running;
                case IBTNode.NodeState.Success: continue;
                case IBTNode.NodeState.Failure: return IBTNode.NodeState.Failure;
            }
        }

        return IBTNode.NodeState.Failure;
    }
}