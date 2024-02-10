using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private RectTransform weaponScrollRectTransform;
    [SerializeField] private RectTransform turretScrollRectTransform;
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

        foreach (var weapon in WeaponManager.Weapons)
        {
            if (WeaponController.Instance.HasWeapon(weapon.key) == false)
            {
                var item = Instantiate(itemPrefab, weaponScrollRectTransform);
                item.Init(weapon);
                item.gameObject.SetActive(true);
                items.Add(item);
            }
        }
        weaponScrollRectTransform.sizeDelta = new Vector2(weaponScrollRectTransform.sizeDelta.x, 150 * items.Count);


        foreach (var turret in TurretManager.Turrets)
        {
            var item = Instantiate(itemPrefab, turretScrollRectTransform);
            item.Init(turret);
            item.gameObject.SetActive(true);
            items.Add(item);
        }
        turretScrollRectTransform.sizeDelta = new Vector2(turretScrollRectTransform.sizeDelta.x, 150 * items.Count);
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
            items.Remove(shopItem);
            shopItem.gameObject.SetActive(false);
            WeaponController.Instance.GetWeapon(weapon);
        }

        Turret turret = shopItem.Item as Turret;
        if (turret != null)
        {
            TurretController.Instance.AddTurret(turret.key);
        }
    }
}
