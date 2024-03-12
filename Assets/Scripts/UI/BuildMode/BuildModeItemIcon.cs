using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BuildModeItemIcon : MonoBehaviour
{
    [SerializeField] protected Image itemImage;

    public string Key { get { return key; } }
    protected string key;
    public virtual void Init(string key)
    {
        this.key = key;
        itemImage.sprite = SpriteManager.GetSprite(key);
    }

    public abstract void Select();
}
