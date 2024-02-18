using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private RectTransform weaponScrollRectTransform;
    [SerializeField] private RectTransform turretScrollRectTransform;
    [SerializeField] private RectTransform otherItemScrollRectTransform;
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

        int count = 0;
        foreach (var weapon in WeaponManager.Weapons)
        {
            if (weapon.consumable) continue;
            var item = Instantiate(itemPrefab, weaponScrollRectTransform);
            item.Init(weapon);
            item.gameObject.SetActive(true);
            items.Add(item);
            count++;
        }
        weaponScrollRectTransform.sizeDelta = new Vector2(150 * count, weaponScrollRectTransform.sizeDelta.y);

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
        otherItemScrollRectTransform.sizeDelta = new Vector2(150 * count, otherItemScrollRectTransform.sizeDelta.y);
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
                Player.Instance.AdjustItemAmount(weapon.key, 1);
            }
            // 소모품이 아닌데 무기가 없다면 새롭게 획득
            else
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
