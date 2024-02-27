using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using TMPro;

public class ShopStatus : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    // 무기, 동료 등 상황에 따라 다른 능력이 표기될 수 있도록 세팅
    // 우선은 무기에만 한정적으로 사용
    [SerializeField] private LocalizeStringEvent[] statusNames;

    // 능력치 수치도 후에 슬라이더로 직관적으로 표기
    [SerializeField] private Slider[] sliders;
    [SerializeField] private TextMeshProUGUI[] stats;

    public void UpdateStatus(Weapon weapon)
    {
        string uiKey = weapon.key.Replace("WEAPON.", "UI.");
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

        // 먼저 가장 기초적인 능력치만
        string dmg = weapon.bullets > 1 ? $"{weapon.dmg}x{weapon.bullets}" : $"{weapon.dmg}";
        string aspeed = $"{string.Format("{0:0.0}", 1 / weapon.adelay)}/s";
        string ammo = $"{weapon.ammo}";

        statusNames[0].SetEntry("GAME.SHOP.STAT.DMG");
        statusNames[1].SetEntry("GAME.SHOP.STAT.ASPEED");
        statusNames[2].SetEntry("GAME.SHOP.STAT.AMMO");

        // 추후 특수능력 추가 표기
        stats[0].text = dmg;
        stats[1].text = aspeed;
        stats[2].text = ammo;
    }
}
