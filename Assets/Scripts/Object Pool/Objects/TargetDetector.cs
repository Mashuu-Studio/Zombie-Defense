using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TargetDetector : MonoBehaviour
{
    private IAttackObject attackObject;
    protected int layerMask;
    private CircleCollider2D collider;
    public virtual void Init(IAttackObject obj, int layerMask)
    {
        collider = GetComponent<CircleCollider2D>();
        collider.isTrigger = true;

        attackObject = obj;
        this.layerMask = layerMask;
    }
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & layerMask) > 0)
        {
            attackObject.AddTarget(collision.gameObject, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & layerMask) > 0)
        {
            attackObject.AddTarget(collision.gameObject, false);
        }
    }*/
}
