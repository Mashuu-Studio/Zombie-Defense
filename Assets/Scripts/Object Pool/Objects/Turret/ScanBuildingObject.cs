using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ScanBuildingObject : BuildingObject
{
    private CircleCollider2D scanCollider;
    public override void Init()
    {
        base.Init();
        scanCollider = GetComponent<CircleCollider2D>();
        scanCollider.isTrigger = true;
    }

    public override void SetData(Building data, Vector2 pos)
    {
        base.SetData(data, pos);
        scanCollider.radius = data.range;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var enemy = collision.transform.parent.GetComponent<EnemyObject>();
            enemy.SetVisible(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            var enemy = collision.transform.parent.GetComponent<EnemyObject>();
            enemy.SetVisible(false);
        }
    }
}
