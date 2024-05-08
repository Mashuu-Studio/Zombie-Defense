using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemObject : Poolable
{
    public static void Drop(Vector2 position, Item item)
    {
        ItemObject dropItem = (ItemObject)PoolController.Pop(item.itemKey);
        dropItem.SetItem(item);
        dropItem.transform.position = position;
    }

    private string key;
    private BuffInfo buff;

    public void SetItem(Item item)
    {
        key = item.itemKey;
        buff = item.buff;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player.Instance.ActivateBuff(buff);
            PoolController.Push(key, this);
        }
    }
}

public class Item
{
    public string itemKey;
    public BuffInfo buff;
    public int prob;
}