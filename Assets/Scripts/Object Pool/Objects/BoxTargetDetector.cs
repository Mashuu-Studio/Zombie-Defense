using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BoxTargetDetector : TargetDetector
{
    private BoxCollider2D collider;
    public override void Init(IAttackObject obj, int layerMask)
    {
        base.Init(obj, layerMask);
        collider = GetComponent<BoxCollider2D>();
    }
    public void SetBox(Vector2 size)
    {
        collider.size = size;
    }
}