using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> panels;
    [SerializeField] private RectTransform weaponScrollRectTransform;
    [SerializeField] private RectTransform otherItemScrollRectTransform;
    [SerializeField] private ShopItem itemPrefab;
    [SerializeField] private Sprite buyItemSprite;
    [SerializeField] private Sprite hireSprite;

    [Header("Units")]
    [SerializeField] private List<CompanionSlot> companionSlots;

    [Header("Info")]
    [SerializeField] private LocalizeStringEvent description;
    [SerializeField] private ShopStatus itemStatus;

    private List<ShopItem> items = new List<ShopItem>();

    public static int ITEM_IMAGE_MAX_WIDTH = 250;
    public static int ITEM_IMAGE_MAX_HEIGHT = 100;

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
            if (weapon.infAmount) continue;
            if (weapon.consumable) continue;
            var item = Instantiate(itemPrefab, weaponScrollRectTransform);
            item.Init(weapon);
            item.gameObject.SetActive(true);
            items.Add(item);
            count++;
        }
        weaponScrollRectTransform.sizeDelta = new Vector2(weaponScrollRectTransform.sizeDelta.x, 150 * count);

        count = 0;
        foreach (var other in ItemManager.Items)
        {
            var item = Instantiate(itemPrefab, otherItemScrollRectTransform);
            item.Init(other);
            if (other.key.Contains("COMPANION")) item.ChangeBuyButtonImage(hireSprite);
            else item.ChangeBuyButtonImage(buyItemSprite);
            item.gameObject.SetActive(true);
            items.Add(item);
            count++;
        }
        otherItemScrollRectTransform.sizeDelta = new Vector2(otherItemScrollRectTransform.sizeDelta.x, 150 * count);

        UpdateInfo(items[0]);
        ChangePanel(0);

        foreach (var slot in companionSlots)
        {
            slot.Init();
        }
    }

    public void Open(bool b)
    {
        gameObject.SetActive(b);
        if (b) UpdateCompanionInfo();
    }

    public void UpdateCompanionInfo()
    {
        companionSlots.ForEach(slot =>
        {
            if (slot.gameObject.activeSelf) slot.UpdateInfo();
        });
    }

    public void ChangePanel(int index)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == index);
        }
    }

    public void BuyItem(ShopItem shopItem, bool isMagazine)
    {
        if (Player.Instance.BuyItem(shopItem.Item, isMagazine) == false) return;

        Weapon weapon = shopItem.Item as Weapon;
        if (weapon != null)
        {
            // 무기가 있거나 소모품이면 소지품에 추가
            if (weapon.consumable || WeaponController.Instance.HasWeapon(weapon.key))
            {
                if (isMagazine) Player.Instance.AddMagazine(weapon.key);
                else if (!isMagazine) Player.Instance.AdjustItemAmount(weapon.key, 1);
            }
            // 소모품이 아닌데 무기가 없고 탄창이 아니라면 새롭게 획득
            else if (!isMagazine)
            {
                WeaponController.Instance.AddWeapon(weapon.key);
                UIController.Instance.AddItem(weapon.key);
            }
        }
        else if (shopItem.Item.key.Contains("COMPANION"))
        {
            if (CompanionController.Instance.Hirable)
            {
                CompanionObject companion = CompanionController.Instance.AddCompanion(shopItem.Item.key);
                foreach (var slot in companionSlots)
                {
                    if (slot.Data == null)
                    {
                        slot.SetData(companion);
                        slot.transform.SetAsLastSibling();
                        break;
                    }
                }
                UIController.Instance.UpdateBuildmodeCompanions();
            }
        }
        else if (shopItem.Item.key.Contains("HEAL"))
        {
            if (shopItem.Item.key.Contains("HP"))
            {
                Player.Instance.Heal(100);
            }
            else if (shopItem.Item.key.Contains("ARMOR"))
            {
                Player.Instance.RefillArmor(100);
            }
        }
        else
        {
            Player.Instance.AdjustItemAmount(shopItem.Item.key, 1);
        }
    }

    public void RemoveCompanion(CompanionObject companion)
    {
        foreach (var slot in companionSlots)
        {
            if (slot.Data == companion)
            {
                slot.SetActive(false);
                return;
            }
        }
        UIController.Instance.UpdateBuildmodeCompanions();
    }

    #region Info
    public void UpdateInfo(ShopItem shopItem)
    {
        description.SetEntry(shopItem.Item.key);
        Weapon weapon = shopItem.Item as Weapon;
        if (weapon != null) itemStatus.UpdateStatus(weapon);
        itemStatus.gameObject.SetActive(weapon != null);
    }
    #endregion
}
