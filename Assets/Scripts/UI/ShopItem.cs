using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private LocalizeStringEvent itemName;
    [SerializeField] private TextMeshProUGUI itemAmount;
    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private TextMeshProUGUI magazinePrice;
    [SerializeField] private Image buyButtomImage;
    [SerializeField] private GameObject buyMagazineButton;

    public BuyableData Item { get; private set; }
    public void Init(BuyableData data)
    {
        string uiKey = data.key.Replace("WEAPON.", "UI.");
        itemImage.sprite = SpriteManager.GetSprite(uiKey);
        if (itemImage.sprite != null)
            itemImage.rectTransform.sizeDelta = new Vector2(itemImage.sprite.rect.width / itemImage.sprite.rect.height * 100, 100);

        float wRatio = itemImage.rectTransform.sizeDelta.x / ShopUI.ITEM_IMAGE_MAX_WIDTH;
        float hRatio = itemImage.rectTransform.sizeDelta.y / ShopUI.ITEM_IMAGE_MAX_HEIGHT;
        // 이미지가 지정한 크기를 벗어났을 경우 크기를 맞춰줌.
        if (wRatio > 1 || hRatio > 1)
        {
            if (wRatio > hRatio) itemImage.rectTransform.sizeDelta /= wRatio;
            else itemImage.rectTransform.sizeDelta /= hRatio;
        }

        itemName.SetEntry(data.key);
        itemPrice.text = $"${data.price}";
        magazinePrice.text = $"${0}";

        Item = data;
    }

    public void ChangeBuyButtonImage(Sprite sprite)
    {
        buyButtomImage.sprite = sprite;
    }

    private void Update()
    {
        if (!GameController.Instance.GameStarted) return;

        if (Item != null)
        {
            bool b = WeaponController.Instance.HasWeapon(Item.key);
            itemAmount.gameObject.SetActive(b);
            if (Item.key == "HEAL.HP")
            {
                int price = (Player.Instance.MaxHp - Player.Instance.Hp) * Item.price / Player.Instance.MaxHp;
                itemPrice.text = price.ToString();
            }
            else if (Item.key == "HEAL.ARMOR")
            {
                int price = (Player.Instance.MaxDef - Player.Instance.Def) * Item.price / Player.Instance.MaxDef;
                itemPrice.text = price.ToString();
            }
            else if (Item.key.Contains("WEAPON"))
            {
                int magazine = Player.Instance.GetMagazine(Item.key);
                string str = magazine >= 0 ? magazine.ToString() : "inf";
                magazinePrice.gameObject.SetActive(b && str != "inf");
                buyMagazineButton.SetActive(b && str != "inf");
                if (b)
                {
                    itemAmount.text = $"{Player.Instance.ItemAmount(Item.key)} : {str}";
                }
            }
        }
    }

    public void BuyItem()
    {
        UIController.Instance.BuyItem(this);
    }

    public void BuyMagazine()
    {
        UIController.Instance.BuyItem(this, true);
    }
}
