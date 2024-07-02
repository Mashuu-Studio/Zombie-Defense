using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyProjectile : Projectile
{
    private BoxCollider2D hitbox;

    private bool isSiege;
    private BuffInfo debuff;

    private int dmg;
    private string summonUnit;
    private int summonProb;

    public override void Init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        hitbox = GetComponent<BoxCollider2D>();
    }

    public void SetProj(Vector2 start, Vector2 dest, float angle,
        bool isSiege, int dmg, float speed, BuffInfo debuff, string summonUnit, int summonProb)
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

        this.summonUnit = summonUnit;
        this.summonProb = summonProb;

        if (remainTime == 0) remainTime = Time.fixedDeltaTime * 2;
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.Pause) return;

        if (!stop)
        {
            Move();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ���������� ��� ������ ������ ���.
        if (isSiege) return;

        int layerMask = 1 << LayerMask.NameToLayer("Player")
            | 1 << LayerMask.NameToLayer("Building");

        if ((1 << collision.gameObject.layer & layerMask) > 0)
        {
            Damage(collision, true);
        }
    }

    protected override IEnumerator RangeDamaging()
    {
        stop = true;

        int layerMask = 1 << LayerMask.NameToLayer("Building") | 1 << LayerMask.NameToLayer("Trap") | 1 << LayerMask.NameToLayer("Player");
        Collider2D[] cols = Physics2D.OverlapBoxAll(
            transform.position, hitbox.size * transform.lossyScale.x,
            transform.rotation.eulerAngles.z, layerMask);
        foreach (var col in cols) Damage(col);

        yield return null;
        Push();
    }

    private void Damage(Collider2D collision, bool singleTarget = false)
    {
        ActionController.AddAction(gameObject, () =>
        {
            int dmg = this.dmg;
            // Player�� �ƴ϶� �ͷ��̶�� 1.5���
            if (isSiege && collision.transform.parent.gameObject != Player.Instance.gameObject) dmg = (int)(this.dmg * 1.5f);

            if (debuff != null)
            {
                IBuffTargetObject buffTargetObject = collision.transform.parent.GetComponent<IBuffTargetObject>();
                if (buffTargetObject != null) buffTargetObject.ActivateBuff(debuff);
            }

            var target = collision.transform.parent.GetComponent<IDamagedObject>();
            target.Damaged(dmg);

            if (singleTarget) Push();
        });
    }

    protected override void Push()
    {
        base.Push();
        if (summonUnit != "" && Random.Range(0, 100) < summonProb)
        {
            Enemy enemy = EnemyManager.GetEnemy(summonUnit);
            EnemyController.Instance.AddEnemy(enemy, transform.position);
        }
    }
}
