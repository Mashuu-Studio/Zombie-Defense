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
    private float remainTime;
    private int speed;

    public override void Init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetBullet(Vector2 start, Vector2 dir, Weapon w, int spd)
    {
        transform.position = rigidbody.position = start;
        direction = dir.normalized;
        weapon = w;
        range = weapon.range;
        speed = spd;
        remainTime = weapon.point;
        if (remainTime == 0) remainTime = 1;

        if (weapon.splash == 0) transform.localScale = Vector3.one;
        else transform.localScale = Vector3.one * w.splash;
    }

    private void FixedUpdate()
    {
        if (weapon.point == 0)
        {
            rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime * speed);
            range -= Time.fixedDeltaTime * speed;
        }
        else remainTime -= Time.fixedDeltaTime;
        Debug.Log(remainTime);
        if (MapGenerator.Instance.mapBoundary.Contains(rigidbody.position) == false
            || range < 0 || remainTime <= 0) PoolController.Push("Bullet", this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.transform.parent.GetComponent<EnemyObject>().Damaged(weapon.dmg);
            if (weapon.pierce == false && weapon.point == 0) PoolController.Push("Bullet", this);
        }
    }
}