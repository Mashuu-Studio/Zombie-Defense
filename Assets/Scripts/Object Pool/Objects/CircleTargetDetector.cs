using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class CircleTargetDetector : TargetDetector
{
    private CircleCollider2D collider;
    public override void Init(IAttackObject obj, int layerMask)
    {
        base.Init(obj, layerMask);
        collider = GetComponent<CircleCollider2D>();
    }
    public void SetRadius(float radius)
    {
        collider.radius = radius;
    }
}
