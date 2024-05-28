using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBullet : Bullet
{
    [SerializeField] private DigitalRuby.LightningBolt.LightningBoltScript lightning;

    public override void Init()
    {
        base.Init();
        lightning.Init();
    }

    public override void SetBullet(Vector2 start, Vector2 dest, Vector2 dir, float angle, Weapon w, float spd)
    {
        base.SetBullet(start, dest, dir, angle, w, spd);
        transform.position = rigidbody.position = dest;
        lightning.StartPosition = start;
        lightning.EndPosition = dest;
        lightning.Activate(true);
    }

    protected override void Push()
    {
        StartCoroutine(Lightning());
    }

    IEnumerator Lightning()
    {
        float time = 0.1f;

        while (time > 0)
        {
            if (!GameController.Instance.Pause) time -= Time.deltaTime;
            yield return null;
        }
        lightning.Activate(false);
        base.Push();
    }
}
