using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Bullet (Poolable)")]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : Poolable 
{
    private Rigidbody2D rigidbody;
    private Vector2 direction;
    private int dmg;
    private float range;
    private int speed;

    public override void Init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetBullet(Vector2 start, Vector2 dir, int d, float r, int spd)
    {
        transform.position = start;
        direction = dir.normalized;
        dmg = d;
        range = r;
        speed = spd;
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime * speed);
        range -= Time.fixedDeltaTime * speed;
        if (MapGenerator.Instance.mapBoundary.Contains(rigidbody.position) == false
            || range < 0) PoolController.Push("Bullet", this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            PoolController.Push("Bullet", this);
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.transform.parent.GetComponent<EnemyObject>().Damaged(dmg);
            PoolController.Push("Bullet", this);
        }
    }
}
