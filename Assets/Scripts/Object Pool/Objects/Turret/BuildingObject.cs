using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Poolable/Building (Poolable)")]
public class BuildingObject : BTPoolable, IDamagedObject
{
    [Space]
    [SerializeField] private ObjectHpBar hpBar;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 pos;
    private int hp;

    public int Hp { get { return hp; } }
    public int Def { get; }

    public Building Data { get { return data; } }
    protected Building data;

    public virtual void SetData(Building data, Vector2 pos)
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
            DestroyBuilding();
        }
    }

    public virtual void DestroyBuilding()
    {
        BuildingController.Instance.RemoveBuilding(pos);
        PoolController.Push(gameObject.name, this);
        StopAllCoroutines();
    }
}
