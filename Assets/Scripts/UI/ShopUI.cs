using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private RectTransform weaponScrollRectTransform;
    [SerializeField] private RectTransform turretScrollRectTransform;
    [SerializeField] private RectTransform otherItemScrollRectTransform;
    [SerializeField] private RectTransform magazineScrollRectTransform;
    [SerializeField] private ShopItem itemPrefab;
    private List<ShopItem> items = new List<ShopItem>();

    private void Awake()
    {
        itemPrefab.gameObject.SetActive(false);
    }

    public void Init()
    {
        items.ForEach(item => Destroy(item.gameObject));
        items.Clear();

        // 우선은 Magazine을 Shop에 간단하게 추가
        // 이 후에 UI를 변경할 것이기 떄문에 우선 작동을 확인할 수 있도록만 함.
        int count = 0;
        foreach (var weapon in WeaponManager.Weapons)
        {
            if (weapon.consumable) continue;
            var item = Instantiate(itemPrefab, weaponScrollRectTransform);
            item.Init(weapon);
            item.gameObject.SetActive(true);
            items.Add(item);
            count++;

            if (weapon.infmagazine) continue;
            var magazine = Instantiate(itemPrefab, magazineScrollRectTransform);
            magazine.Init(weapon, true);
            magazine.gameObject.SetActive(true);
            items.Add(magazine);
        }
        weaponScrollRectTransform.sizeDelta = new Vector2(150 * count, weaponScrollRectTransform.sizeDelta.y);
        magazineScrollRectTransform.sizeDelta = new Vector2(150 * count, magazineScrollRectTransform.sizeDelta.y);

        count = 0;
        foreach (var turret in TurretManager.Turrets)
        {
            var item = Instantiate(itemPrefab, turretScrollRectTransform);
            item.Init(turret);
            item.gameObject.SetActive(true);
            items.Add(item);
            count++;
        }
        turretScrollRectTransform.sizeDelta = new Vector2(150 * count, turretScrollRectTransform.sizeDelta.y);

        count = 0;
        foreach (var other in ItemManager.Items)
        {
            var item = Instantiate(itemPrefab, otherItemScrollRectTransform);
            item.Init(other);
            item.gameObject.SetActive(true);
            items.Add(item);
            count++;
        }
        // otherItemScrollRectTransform.sizeDelta = new Vector2(150 * count, otherItemScrollRectTransform.sizeDelta.y);
    }

    public void Open(bool b)
    {
        gameObject.SetActive(b);
    }

    public void BuyItem(ShopItem shopItem)
    {
        Weapon weapon = shopItem.Item as Weapon;
        if (weapon != null)
        {
            // 무기가 있거나 소모품이면 소지품에 추가
            if (weapon.consumable || WeaponController.Instance.HasWeapon(weapon.key))
            {
                if (shopItem.IsMagazine) Player.Instance.AddMagazine(weapon.key);
                else Player.Instance.AdjustItemAmount(weapon.key, 1);
            }
            // 소모품이 아닌데 무기가 없고 탄창이 아니라면 새롭게 획득
            else if (!shopItem.IsMagazine)
            {
                WeaponController.Instance.AddWeapon(weapon.key);
                UIController.Instance.AddItem(weapon.key);
            }
        }
        else
        {
            Player.Instance.AdjustItemAmount(shopItem.Item.key, 1);
        }
    }
}
