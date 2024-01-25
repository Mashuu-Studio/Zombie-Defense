using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Turret (Poolable)")]
public class TurretObject : BTPoolable, IDamagedObject
{
    private Vector2 pos;
    private int hp;

    public int Hp { get { return hp; } }

    public virtual void Init(Turret data, Vector2 pos)
    {
        hp = data.hp;
        this.pos = pos;
    }

    public void Damaged(int dmg)
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
