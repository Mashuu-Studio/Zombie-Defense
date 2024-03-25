using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Poolable
{
    private Rigidbody2D rigidbody;

    private Vector2 destination;
    private Vector2 direction;

    private float remainTime;
    private float speed;

    private int dmg;
    private float radius;

    private bool stop;

    public override void Init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetProj(Vector2 start, Vector2 dest, int dmg, float radius, float speed)
    {
        stop = false;

        transform.position = rigidbody.position = start;
        destination = dest;
        direction = (dest - start).normalized;

        this.dmg = dmg;
        this.radius = radius;
        this.speed = speed;

        if (remainTime == 0) remainTime = Time.fixedDeltaTime * 2;
        transform.localScale = Vector3.one * radius * 2;
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.Pause) return;

        if (!stop)
        {
            rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime * speed);

            // 도착한 것은 특정한 방법을 활용하기로 함.
            // 현재 위치로부터 dest, dest + moveAmount 3가지 경우와의 거리를 책정함.
            // dest와 가깝다면 아직 도착하지 않은 것이고 dest + moveAmount와 가깝다면 도착한 것임.
            // 도착했을 때 위치를 dest로 세팅해주고 폭발시켜줌.
            float dist1 = Vector2.Distance(transform.position, destination);
            float dist2 = Vector2.Distance(transform.position, destination + direction * Time.fixedDeltaTime);
            if (dist1 == 0 || dist1 >= dist2)
            {
                transform.position = destination;
                StartCoroutine(RangeDamage());
            }
        }
    }

    IEnumerator RangeDamage()
    {
        stop = true;

        int layerMask = 1 << LayerMask.NameToLayer("Turret") | 1 << LayerMask.NameToLayer("Trap") | 1 << LayerMask.NameToLayer("Player");
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius, layerMask);
        foreach (var col in cols) Damage(col);

        yield return null;
        PoolController.Push("Projectile", this);
    }

    private void Damage(Collider2D collision)
    {
        ActionController.AddAction(gameObject, () =>
        {
            int dmg = this.dmg;
            // Player가 아니라 터렛이라면 1.5배수
            if (collision.transform.parent.gameObject != Player.Instance.gameObject) dmg = (int)(this.dmg * 1.5f);

            var target = collision.transform.parent.GetComponent<IDamagedObject>();
            target.Damaged(dmg);
        });
    }
}
