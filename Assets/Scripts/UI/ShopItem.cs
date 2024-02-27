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
        // �̹����� ������ ũ�⸦ ����� ��� ũ�⸦ ������.
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

    private void Update()
    {
        if (GameController.Instance.GameStarted && Item != null)
        {
            bool b = WeaponController.Instance.HasWeapon(Item.key);
            itemAmount.gameObject.SetActive(b);
            string magazine = Player.Instance.GetMagazine(Item.key) >= 0 ? Player.Instance.GetMagazine(Item.key).ToString() : "inf";
            magazinePrice.gameObject.SetActive(b && magazine != "inf");
            buyMagazineButton.SetActive(b && magazine != "inf");
            if (b)
            {
                itemAmount.text = $"{Player.Instance.ItemAmount(Item.key)} : {magazine}";
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
