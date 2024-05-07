using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : Poolable
{
    private Rigidbody2D rigidbody;
    private BoxCollider2D hitbox;

    private Vector2 destination;
    private Vector2 direction;

    private float remainTime;
    private float speed;
    private bool isSiege;
    private BuffInfo debuff;

    private int dmg;

    private bool stop;

    public override void Init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<BoxCollider2D>();
    }

    public void SetProj(Vector2 start, Vector2 dest, float angle, 
        bool isSiege, int dmg, float speed, BuffInfo debuff)
    {
        stop = false;

        transform.position = rigidbody.position = start;
        transform.rotation = Quaternion.Euler(0, 0, angle);
        destination = dest;
        direction = (dest - start).normalized;

        this.dmg = dmg;
        this.speed = speed;
        this.isSiege = isSiege;
        this.debuff = debuff;

        if (remainTime == 0) remainTime = Time.fixedDeltaTime * 2;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 공성유닛의 경우 도착할 때까지 대기.
        if (isSiege) return;

        int layerMask = 1 << LayerMask.NameToLayer("Player") 
            | 1 << LayerMask.NameToLayer("Turret")
            | 1 << LayerMask.NameToLayer("Trap");

        if ((1 << collision.gameObject.layer & layerMask) > 0)
        {
            Damage(collision, true);
        }
    }

    IEnumerator RangeDamage()
    {
        stop = true;

        int layerMask = 1 << LayerMask.NameToLayer("Turret") | 1 << LayerMask.NameToLayer("Trap") | 1 << LayerMask.NameToLayer("Player");
        Collider2D[] cols = Physics2D.OverlapBoxAll(
            transform.position, hitbox.size * transform.lossyScale.x,
            transform.rotation.eulerAngles.z, layerMask);
        foreach (var col in cols) Damage(col);

        yield return null;
        PoolController.Push(gameObject.name, this);
    }

    private void Damage(Collider2D collision, bool singleTarget = false)
    {
        ActionController.AddAction(gameObject, () =>
        {
            int dmg = this.dmg;
            // Player가 아니라 터렛이라면 1.5배수
            if (isSiege && collision.transform.parent.gameObject != Player.Instance.gameObject) dmg = (int)(this.dmg * 1.5f);

            if (debuff != null)
            {
                IBuffTargetObject buffTargetObject = collision.transform.parent.GetComponent<IBuffTargetObject>();
                if (buffTargetObject != null) buffTargetObject.ActivateBuff(debuff);
            }
            var target = collision.transform.parent.GetComponent<IDamagedObject>();
            target.Damaged(dmg);

            if (singleTarget) PoolController.Push(gameObject.name, this);
        });
    }
}
