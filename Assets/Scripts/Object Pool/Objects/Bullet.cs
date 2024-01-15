using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Bullet (Poolable)")]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : Poolable 
{
    private Rigidbody2D rigidbody;
    private Vector2 direction;
    private Weapon weapon;
    private float range;
    private int speed;

    public override void Init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetBullet(Vector2 start, Vector2 dir, Weapon w, int spd)
    {
        rigidbody.position = start;
        direction = dir.normalized;
        weapon = w;
        range = weapon.range;
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
            collision.transform.parent.GetComponent<EnemyObject>().Damaged(weapon.dmg);
            if (weapon.pierce == false) PoolController.Push("Bullet", this);
        }
    }
}
