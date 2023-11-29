using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Bullet (Poolable)")]
public class Bullet : Poolable 
{
    private Rigidbody2D rigidbody;
    private Vector3 direction;
    private int dmg;
    private int speed;

    public override void Init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetBullet(Vector2 start, Vector2 dir, int d, int spd)
    {
        transform.position = start;
        direction = dir;
        dmg = d;
        speed = spd;
    }

    private void Update()
    {
        rigidbody.MovePosition(transform.position + direction * Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.GetComponent<TestEnemy>().Damaged(dmg);
            PoolController.Push("Bullet", this);
        }
    }
}
