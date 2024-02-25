using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Turret (Poolable)")]
public class TurretObject : BTPoolable, IDamagedObject
{
    [Space]
    [SerializeField] private ObjectHpBar hpBar;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 pos;
    private int hp;

    public int Hp { get { return hp; } }
    public int Def { get; }

    public Turret Data { get { return data; } }
    private Turret data;

    public virtual void SetData(Turret data, Vector2 pos)
    {
        this.data = data;

        hp = data.hp;
        hpBar.SetHpBar(hp, new Vector2(0.75f, 0.1f), 0.6f);

        this.pos = pos;
    }

    public void Damaged(int dmg)
    {
        hp -= dmg;
        hpBar.UpdateHpBar(hp);
        if (hp <= 0)
        {
            DestroyTurret();
        }
    }

    public virtual void DestroyTurret()
    {
        TurretController.Instance.RemoveTurret(pos);
        PoolController.Push(gameObject.name, this);
        StopAllCoroutines();
    }
}
