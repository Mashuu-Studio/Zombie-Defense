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
    [SerializeField] private ShopItemStat[] stats;

    public void UpdateStatus(string key)
    {
        string uiKey = key.Replace("WEAPON.", "UI.");
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

        foreach (var stat in stats)
            stat.gameObject.SetActive(false);
        Weapon weapon = WeaponManager.GetWeapon(key);
        if (weapon != null)
        {
            /* 슬라이더는 기본적으로 0 to 1
             * 데미지 최대치 = 250. slider = dmg / 200
             * 공속 1 / adelay. 제일 높은 공속 = 0.033. 30 으로 두면 될 듯. sldier = aspeed / 30.
             * 관통력 or 범위
             *    제일 높은 관통력 = 6, 예외로 25 slider = pierce / 10.
             *    제일 높은 범위 = 3. slider = radius / 3;
             * 정확도 100 - spread 0 to 45 slider = accurancy - 50 / 50
             * 장탄 수 ammo 제일 많은 장탄 = 100 sldier = ammo / 100
             * 장전 속도 reload 제일빠른 장전 속도 = 0.5f slider = 0.5f / reload;
             * 무기타입은 Description 부분에 표기. 혹은 이미지?
             */
            int dmg = weapon.autotarget ? weapon.dmg : weapon.dmg * weapon.bullets;
            float aspeed = 1 / weapon.adelay;
            int pierce = weapon.pierce;
            float radius = weapon.radius;
            int accuracy = 100 - weapon.bulletspreadangle;
            int ammo = weapon.ammo;
            float reload = weapon.reload;

            string[] names = new string[6]
            {
                "GAME.SHOP.STAT.DMG",
                "GAME.SHOP.STAT.ASPEED",
                "GAME.SHOP.STAT.PIERCE",
                "GAME.SHOP.STAT.ACCURACY",
                "GAME.SHOP.STAT.AMMO",
                "GAME.SHOP.STAT.RELOAD",
            };

            float[] values = new float[6]
            {
                dmg / 250f,
                aspeed / 30,
                pierce / 10f,
                (accuracy - 50) / 50f,
                ammo / 100f,
                0.5f / reload,
            };

            string[] status = new string[6]
            {
                weapon.bullets > 1 && !weapon.autotarget ? $"{weapon.dmg}x{weapon.bullets}" : $"{weapon.dmg}",
                string.Format("{0:0.0}/s", aspeed),
                pierce.ToString(),
                accuracy.ToString(),
                ammo.ToString(),
                string.Format("{0:0.0}s", reload),
            };

            // 범위를 표기해야하는 경우
            if (radius > 0)
            {
                values[2] = radius / 3;
                names[2] = "GAME.SHOP.STAT.RADIUS";
                status[2] = radius.ToString();
            }

            // 오토타겟의 경우에도 범위로 표기
            if (weapon.autotarget)
            {
                values[2] = weapon.range / 3;
                names[2] = "GAME.SHOP.STAT.RADIUS";
                status[2] = weapon.range.ToString();
            }

            for (int i = 0; i < 6; i++)
            {
                // 수류탄의 경우에는 데미지와 범위만 표기.
                if (weapon.consumable && (i != 0 && i != 2)) continue;

                stats[i].SetStatus(names[i], values[i], status[i]);
                stats[i].gameObject.SetActive(true);
            }
        }

        Companion companion = CompanionManager.GetCompanion(key);
        if (companion != null)
        {
            /* 동료는 체력과 방어력 두 종류
             * 체력의 최대치는 100
             * 방어력의 최대치는 100
             */
            string[] names = new string[2]
            {
                "GAME.SHOP.STAT.HP",
                "GAME.SHOP.STAT.ARMOR",
            };

            float[] values = new float[2]
            {
                companion.hp / 100f,
                companion.def / 100f,
            };

            string[] status = new string[2]
            {
                companion.hp.ToString(),
                companion.def.ToString()
            };

            for (int i = 0; i < 2; i++)
            {
                stats[i].SetStatus(names[i], values[i], status[i]);
                stats[i].gameObject.SetActive(true);
            }
        }
    }
}
