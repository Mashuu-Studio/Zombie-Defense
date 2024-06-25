using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Projectile : Poolable
{
    protected Rigidbody2D rigidbody;

    protected Vector2 destination;
    protected Vector2 direction;

    protected float remainTime;
    protected float speed;

    protected bool stop;

    protected IEnumerator rangeDamageCoroutine;
    public override void Init()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (GameController.Instance.Pause) return;

        if (!stop)
        {
            Move();
        }
    }

    protected void Move()
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
            RangeDamage();
        }
    }

    protected void RangeDamage()
    {
        // 이미 진행중이라면 진행하지 않음.
        if (rangeDamageCoroutine == null)
        {
            rangeDamageCoroutine = RangeDamaging();
            StartCoroutine(rangeDamageCoroutine);
        }
    }

    protected abstract IEnumerator RangeDamaging();

    protected virtual void Push()
    {
        rangeDamageCoroutine = null;
        PoolController.Push(gameObject.name, this);

        string particleName = gameObject.name.Replace("PROJECTILE", "PARTICLE");
        if (particleName != gameObject.name)
        {
            var particle = PoolController.Pop(particleName);
            if (particle != null)
            {
                particle.transform.position = transform.position;
                ((ParticleObject)particle).Play(transform.rotation.eulerAngles.z);
            }
        }
    }
}
