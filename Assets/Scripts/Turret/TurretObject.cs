using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Turret (Poolable)")]
public class TurretObject : Poolable, IDamagedObject
{
    int hp;

    public void Init(Turret data)
    {
        hp = data.hp;
    }

    public void Damaged(int dmg)
    {
        hp -= dmg;
        if (hp <= 0)
            PoolController.Push(gameObject.name, this);
    }
}
