using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Bullet (Poolable)")]
[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : Projectile
{
    [SerializeField] TrailRenderer trail;
    [SerializeField] ParticleSystem particle;

    private CircleCollider2D hitbox;

    private Weapon weapon;

    private float distance;

    private float originRadius;
    private float radius;
    private float bulletSize;

    private bool point;
    private float dmgDelay;

    ParticleSystem.MainModule pmain;

    public override void Init()
    {
        base.Init();
        hitbox = GetComponent<CircleCollider2D>();
    }

    public virtual void SetBullet(Vector2 start, Vector2 dest, Vector2 dir, float angle,
        Weapon w, float spd)
    {
        stop = false;

        transform.position = rigidbody.position = start;
        destination = dest;
        direction = dir.normalized;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        bulletSize = w.bulletSize;
        transform.localScale = Vector3.one * bulletSize;
        if (particle != null)
        {
            pmain = particle.main;
            float lifetime = Vector2.Distance(start, dest) / (Time.fixedDeltaTime * speed);
            pmain.startSize = new ParticleSystem.MinMaxCurve(pmain.startSize.constant * bulletSize);
            particle.Play();
        }

        weapon = w;
        dmgDelay = w.dmgdelay;
        distance = weapon.range;
        speed = spd;

        originRadius = hitbox.radius;
        radius = w.radius / 2;

        point = w.point;
        if (remainTime == 0) remainTime = Time.fixedDeltaTime * 2;
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
            Move();
        }
        // 그 외의 경우에는 특정 상황이 되면 사라짐.
        else if (MapGenerator.Instance.MapBoundary.Contains(rigidbody.position) == false
                || distance < 0 || remainTime <= 0)
        {
            Push();
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

    protected override IEnumerator RangeDamage()
    {
        stop = true;

        while (dmgDelay > 0)
        {
            if (!GameController.Instance.Pause) dmgDelay -= Time.deltaTime;
            yield return null;
        }

        hitbox.radius = radius;

        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerMask.NameToLayer("Enemy"));
        foreach (var col in cols) Damage(col);

        yield return null;
        Push();
    }

    private void Damage(Collider2D collision)
    {
        var enemy = collision.transform.parent.GetComponent<EnemyObject>();

        ActionController.AddAction(gameObject, () =>
        {
            if (weapon.attribute == ObjectData.Attribute.BULLET) 
                SoundController.Instance.PlaySFX(collision.transform, "BULLET.DAMAGED", true);
            
            
            enemy.Damaged(weapon.dmg, weapon.attribute);
            if (weapon.pierce == false && !point)
            {
                Push();
                StopAllCoroutines();
            }
        });
    }

    protected override void Push()
    {
        trail.Clear();
        if (particle != null)
            pmain.startSize = new ParticleSystem.MinMaxCurve(pmain.startSize.constant / bulletSize);

        PoolController.Push(gameObject.name, this);

        string particleName = gameObject.name.Replace("WEAPON", "PARTICLE");
        if (particleName != gameObject.name)
        {
            var particle = PoolController.Pop(particleName);
            if (particle != null)
            {
                particle.transform.position = transform.position;
                ((ParticleObject)particle).Play(0, hitbox.radius * 2);
            }
        }
        hitbox.radius = originRadius;
    }
}
