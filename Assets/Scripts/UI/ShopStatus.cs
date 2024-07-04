using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using TMPro;

public class ShopStatus : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    // ����, ���� �� ��Ȳ�� ���� �ٸ� �ɷ��� ǥ��� �� �ֵ��� ����
    // �켱�� ���⿡�� ���������� ���
    [SerializeField] private ShopItemStat[] stats;

    public void UpdateStatus(string key)
    {
        string uiKey = key.Replace("WEAPON.", "UI.");
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

        foreach (var stat in stats)
            stat.gameObject.SetActive(false);
        Weapon weapon = WeaponManager.GetWeapon(key);
        if (weapon != null)
        {
            /* �����̴��� �⺻������ 0 to 1
             * ������ �ִ�ġ = 250. slider = dmg / 200
             * ���� 1 / adelay. ���� ���� ���� = 0.033. 30 ���� �θ� �� ��. sldier = aspeed / 30.
             * ����� or ����
             *    ���� ���� ����� = 6, ���ܷ� 25 slider = pierce / 10.
             *    ���� ���� ���� = 3. slider = radius / 3;
             * ��Ȯ�� 100 - spread 0 to 45 slider = accurancy - 50 / 50
             * ��ź �� ammo ���� ���� ��ź = 100 sldier = ammo / 100
             * ���� �ӵ� reload ���Ϻ��� ���� �ӵ� = 0.5f slider = 0.5f / reload;
             * ����Ÿ���� Description �κп� ǥ��. Ȥ�� �̹���?
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

            // ������ ǥ���ؾ��ϴ� ���
            if (radius > 0)
            {
                values[2] = radius / 3;
                names[2] = "GAME.SHOP.STAT.RADIUS";
                status[2] = radius.ToString();
            }

            // ����Ÿ���� ��쿡�� ������ ǥ��
            if (weapon.autotarget)
            {
                values[2] = weapon.range / 3;
                names[2] = "GAME.SHOP.STAT.RADIUS";
                status[2] = weapon.range.ToString();
            }

            for (int i = 0; i < 6; i++)
            {
                // ����ź�� ��쿡�� �������� ������ ǥ��.
                if (weapon.consumable && (i != 0 && i != 2)) continue;

                stats[i].SetStatus(names[i], values[i], status[i]);
                stats[i].gameObject.SetActive(true);
            }
        }

        Companion companion = CompanionManager.GetCompanion(key);
        if (companion != null)
        {
            /* ����� ü�°� ���� �� ����
             * ü���� �ִ�ġ�� 100
             * ������ �ִ�ġ�� 100
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
