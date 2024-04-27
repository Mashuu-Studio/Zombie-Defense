using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Bullet (Poolable)")]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : Poolable
{
    private Rigidbody2D rigidbody;

    private Weapon weapon;

    private Vector2 destination;
    private Vector2 direction;

    private float distance;
    private float remainTime;
    private float speed;

    private float radius;

    private bool stop;
    private bool point;
    private float dmgDelay;

    public override void Init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    public void SetBullet(Vector2 start, Vector2 dest, Vector2 dir, Weapon w, float spd)
    {
        stop = false;

        transform.position = rigidbody.position = start;
        destination = dest;
        direction = dir.normalized;

        weapon = w;
        dmgDelay = w.dmgdelay;
        distance = weapon.range;
        speed = spd;

        radius = w.radius;

        point = w.point;
        if (remainTime == 0) remainTime = Time.fixedDeltaTime * 2;

        transform.localScale = Vector3.one * w.bulletSize;
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.Pause) return;

        // 오토타겟의 경우 특정 위치에 생겼다가 사라져야 함.
        if (weapon.autotarget)
            remainTime -= Time.fixedDeltaTime;
        // 그 외의 경우는 특정 방향으로 이동해야 함.
        else if (!stop)
        {
            rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime * speed);
            distance -= Time.fixedDeltaTime * speed;
        }

        // 포인트 공격의 경우에는 특정 위치에 도착했을 경우만 폭발해야함.
        if (point)
        {
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
        // 그 외의 경우에는 특정 상황이 되면 사라짐.
        else if (MapGenerator.Instance.MapBoundary.Contains(rigidbody.position) == false
                || distance < 0 || remainTime <= 0)
        {
            PoolController.Push("Bullet", this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if (point)
            {
                dmgDelay = 0;
                StartCoroutine(RangeDamage());
            }
            else Damage(collision);
        }
    }

    IEnumerator RangeDamage()
    {
        stop = true;

        while (dmgDelay > 0)
        {
            if (!GameController.Instance.Pause) dmgDelay -= Time.deltaTime;
            yield return null;
        }

        // 폭발 범위를 보기 위한 용도
        transform.localScale = Vector3.one * radius * 2;

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (var col in cols) Damage(col);

        yield return null;
        PoolController.Push("Bullet", this);
    }

    private void Damage(Collider2D collision)
    {
        var enemy = collision.transform.parent.GetComponent<EnemyObject>();

        ActionController.AddAction(gameObject, () =>
        {
            enemy.Damaged(weapon.dmg, weapon.attribute);
            if (weapon.pierce == false && !point)
            {
                PoolController.Push("Bullet", this);
                StopAllCoroutines();
            }
        });
    }
}
