using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private DescriptionIcon itemIcon;
    [SerializeField] private TextMeshProUGUI itemAmount;
    [SerializeField] private TextMeshProUGUI itemPrice;

    public BuyableData Item { get; private set; }
    public bool IsMagazine { get { return magazine; } }
    private bool magazine;
    public void Init(BuyableData data, bool magazine = false)
    {
        itemImage.sprite = SpriteManager.GetSprite(data.key);
        itemPrice.text = $"${data.price}";
        Item = data;
        itemIcon.SetIcon(Item.key);

        this.magazine = magazine;
    }

    private void Update()
    {
        if (Player.Instance != null && Item != null)
        {
            itemAmount.text = Player.Instance.ItemAmount(Item.key).ToString();
            // 무기의 경우에는 가지고 있는지 아닌지도 체크해주어야 함.
            if (Item as Weapon != null && !WeaponController.Instance.HasWeapon(Item.key))
                itemAmount.text = "NONE";
        }
    }

    public void BuyItem()
    {
        UIController.Instance.BuyItem(this);
    }
}
