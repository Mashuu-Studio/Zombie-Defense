using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemPrice;

    public Weapon Item { get; private set; }
    public void Init(Weapon weapon)
    {
        itemName.text = weapon.name;
        itemPrice.text = $"${weapon.price}";
        Item = weapon;
    }

    public void BuyItem()
    {
        UIController.Instance.BuyItem(this);
    }
}
