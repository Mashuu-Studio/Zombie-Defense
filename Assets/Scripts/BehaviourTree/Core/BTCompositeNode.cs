using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BTCompositeNode : BTNode
{    public List<BTNode> children = new List<BTNode>();

    public override BTNode Clone()
    {
        BTCompositeNode node = Instantiate(this);
        node.children = children.ConvertAll(c => c.Clone());
        return node;
    }}
