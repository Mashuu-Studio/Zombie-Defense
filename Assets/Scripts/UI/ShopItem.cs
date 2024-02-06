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

    public Weapon Item { get; private set; }
    public void Init(Weapon weapon)
    {
        localizeStringEvent.StringReference.TableEntryReference = weapon.key;    
        itemPrice.text = $"${weapon.price}";
        Item = weapon;
    }

    public void BuyItem()
    {
        UIController.Instance.BuyItem(this);
    }
}
