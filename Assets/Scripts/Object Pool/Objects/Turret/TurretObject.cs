using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Turret (Poolable)")]
public class TurretObject : BTPoolable, IDamagedObject
{
    private int hp;

    public int Hp { get { return hp; } }

    public virtual void Init(Turret data)
    {
        hp = data.hp;
    }

    public void Damaged(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
        {
            PoolController.Push(gameObject.name, this);
            StopAllCoroutines();
        }
    }
}
