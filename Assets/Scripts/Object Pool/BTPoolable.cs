using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTPoolable : Poolable
{
    public BehaviourTree tree;

    public override void Init()
    {
        if (tree == null) return;
        Context context = Context.CreateContextFromObject(gameObject);
        tree = tree.Clone();
        tree.Bind(context);
    }

    public virtual void Update()
    {
        if (tree == null) return;
        if (gameObject.activeSelf) tree.Update();
    }
}
