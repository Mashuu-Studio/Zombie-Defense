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

    IEnumerator changeColorCoroutine;
    public virtual void Damaged(int dmg, ObjectData.Attribute attribute = ObjectData.Attribute.NONE)
    {
        hp -= dmg;

        if (changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);
        changeColorCoroutine = ChangeColor(Color.red);
        StartCoroutine(changeColorCoroutine);

        if (hp <= 0)
        {
            DestroyBuilding();
        }
    }
    IEnumerator ChangeColor(Color color)
    {
        Color reverse = Color.white - color;
        float time = 0.2f;
        while (time > 0)
        {
            if (!GameController.Instance.Pause)
            {
                spriteRenderer.material.SetColor("_Color", color);
                time -= Time.deltaTime;
                color += reverse * Time.deltaTime * 5;
            }
            yield return null;
        }
        spriteRenderer.material.SetColor("_Color", Color.white);
    }

    public virtual void DestroyBuilding()
    {
        BuildingController.Instance.RemoveBuilding(pos);
        PoolController.Push(gameObject.name, this);
        StopAllCoroutines();
    }
}
