using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MountWeaponIcon : BuildModeItemIcon
{
    [SerializeField] private TextMeshProUGUI amountText;
    public override void Init(string key)
    {
        this.key = key;
        key = key.Replace("WEAPON.", "UI.");
        itemImage.sprite = SpriteManager.GetSprite(key);

        if (itemImage.sprite != null) itemImage.rectTransform.sizeDelta = new Vector2(itemImage.sprite.rect.width / itemImage.sprite.rect.height * 50, 50);
        float wRatio = itemImage.rectTransform.sizeDelta.x / MountWeaponDropdown.ITEM_IMAGE_MAX_WIDTH;
        float hRatio = itemImage.rectTransform.sizeDelta.y / MountWeaponDropdown.ITEM_IMAGE_MAX_HEIGHT;
        // 이미지가 지정한 크기를 벗어났을 경우 크기를 맞춰줌.
        if (wRatio > 1 || hRatio > 1)
        {
            if (wRatio > hRatio) itemImage.rectTransform.sizeDelta /= wRatio;
            else itemImage.rectTransform.sizeDelta /= hRatio;
        }
    }

    public void UpdateAmount()
    {
        int amount = Player.Instance.ItemAmount(key);
        string str = amount >= 0 ? amount.ToString() : "inf";

        amountText.text = str;
    }

    public override void Select()
    {
        TurretController.Instance.Mount(key);
        UIController.Instance.ShowMountWeaponUI(false, Vector2.zero);
    }
}
