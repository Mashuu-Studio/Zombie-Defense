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
    protected Turret data;

    public virtual void SetData(Turret data, Vector2 pos)
    {
        this.data = data;

        hp = data.hp;
        hpBar.SetHpBar(this, hp);

        this.pos = pos;
    }

    public virtual void Damaged(int dmg, ObjectData.Attribute attribute = ObjectData.Attribute.NONE)
    {
        hp -= dmg;
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
