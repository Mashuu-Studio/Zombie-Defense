using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private LocalizeStringEvent localizeStringEvent;
    [SerializeField] private TextMeshProUGUI itemPrice;

    public BuyableData Item { get; private set; }
    public void Init(BuyableData data)
    {
        localizeStringEvent.SetEntry(data.key);
        itemImage.sprite = SpriteManager.GetSprite(data.key);
        itemPrice.text = $"${data.price}";
        Item = data;
    }

    public void BuyItem()
    {
        UIController.Instance.BuyItem(this);
    }
}
