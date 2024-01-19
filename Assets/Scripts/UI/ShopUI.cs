using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private RectTransform scrollRectTransform;
    [SerializeField] private ShopItem itemPrefab;
    private List<ShopItem> items = new List<ShopItem>();

    private void Awake()
    {
        itemPrefab.gameObject.SetActive(false);
    }
    public void Open(bool b)
    {
        gameObject.SetActive(b);

        if (b)
        {
            items.ForEach(item => Destroy(item.gameObject));
            items.Clear();

            foreach (var weapon in WeaponManager.Weapons)
            {
                if (WeaponController.Instance.HasWeapon(weapon.name) == false)
                {
                    var item = Instantiate(itemPrefab, scrollRectTransform);
                    item.Init(weapon);
                    item.gameObject.SetActive(true);
                    items.Add(item);
                }
            }

            scrollRectTransform.sizeDelta = new Vector2(scrollRectTransform.sizeDelta.x, 150 * items.Count);
        }
    }

    public void BuyItem(ShopItem shopItem)
    {
        items.Remove(shopItem);
        Destroy(shopItem.gameObject);
        WeaponController.Instance.GetWeapon(shopItem.Item);
    }
}
