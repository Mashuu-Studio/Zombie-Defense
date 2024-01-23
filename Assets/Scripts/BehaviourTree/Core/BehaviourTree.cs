using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public BTNode rootNode;
    public BTNode.State treeState = BTNode.State.Running;
    public List<BTNode> nodes = new List<BTNode>();

    public BTNode.State Update()
    {
        if (rootNode.state == BTNode.State.Running)
        {
            treeState = rootNode.Update();
        }
        return treeState;
    }

    #region Editor
#if UNITY_EDITOR
    public BTNode CreateNode(Type type)
    {
        BTNode node = CreateInstance(type) as BTNode;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();
        nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();

        return node;
    }

    public void DeleteNode(BTNode node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }

    public void AddChild(BTNode parent, BTNode child)
    {
        BTRootNode root = parent as BTRootNode;
        if (root)
        {
            root.child = child;
        }

        BTDecoratorNode decorator = parent as BTDecoratorNode;
        if (decorator)
        {
            decorator.child = child;
        }

        BTCompositeNode composite = parent as BTCompositeNode;
        if (composite)
        {
            composite.children.Add(child);
        }
    }

    public void RemoveChild(BTNode parent, BTNode child)
    {
        BTRootNode root = parent as BTRootNode;
        if (root)
        {
            root.child = null;
        }

        BTDecoratorNode decorator = parent as BTDecoratorNode;
        if (decorator)
        {
            decorator.child = null;
        }

        BTCompositeNode composite = parent as BTCompositeNode;
        if (composite)
        {
            composite.children.Remove(child);
        }
    }
#endif
#endregion

    public static List<BTNode> GetChildren(BTNode parent)
    {
        List<BTNode> children = new List<BTNode>();

        BTRootNode root = parent as BTRootNode;
        if (root && root.child != null)
        {
            children.Add(root.child);
        }

        BTDecoratorNode decorator = parent as BTDecoratorNode;
        if (decorator && decorator.child != null)
        {
            children.Add(decorator.child);
        }

        BTCompositeNode composite = parent as BTCompositeNode;
        if (composite && composite.children != null)
        {
            return composite.children;
        }
        return children;
    }

    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        return tree;
    }

    public static void Traverse(BTNode node, Action<BTNode> action)
    {
        action.Invoke(node);
        var children = GetChildren(node);
        children.ForEach(n => Traverse(n, action));
    }

    public void Bind(Context context)
    {
        Traverse(rootNode, (n) => n.context = context);
    }
}