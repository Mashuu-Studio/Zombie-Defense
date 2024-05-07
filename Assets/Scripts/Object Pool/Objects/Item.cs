using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class Item : Poolable
{
    public static void Drop(Vector2 position)
    {
        BuffType buffType = (BuffType)Random.Range(0, System.Enum.GetValues(typeof(BuffType)).Length);
        int value = 0;
        if (buffType == BuffType.MONEY)
        {
            int rand = Random.Range(1, 5);
            value = rand * 50;
        }
        Item item = (Item)PoolController.Pop("Item");
        item.SetItem(buffType, value);
        item.transform.position = position;
    }

    public enum BuffType { HP = 0, MONEY, INVINCIBLE, RELOAD, SPEED }
    private BuffType buffType;
    private int value;

    private SpriteRenderer spriteRenderer;

    public override void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetItem(BuffType buffType, int value = 0)
    {
        // 스프라이트 세팅 임시
        switch (buffType)
        {
            case BuffType.HP: spriteRenderer.color = Color.red; break;
            case BuffType.MONEY: spriteRenderer.color = Color.green; break;
            case BuffType.INVINCIBLE: spriteRenderer.color = Color.yellow; break;
            case BuffType.RELOAD: spriteRenderer.color = Color.magenta; break;
            case BuffType.SPEED: spriteRenderer.color = Color.cyan; break;
        }

        this.buffType = buffType;
        this.value = value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Player.Instance.Buff(buffType, value);
            PoolController.Push("Item", this);
        }
    }
}
