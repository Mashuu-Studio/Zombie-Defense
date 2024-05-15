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

    [Header("Units")]
    [SerializeField] private List<CompanionSlot> companionSlots;

    [Header("Status")]
    [SerializeField] private TextMeshProUGUI bonusStat;
    [SerializeField] private TextMeshProUGUI[] statUpgradeInfos;

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
    }

    public void ChangePanel(int index)
    {
        for (int i = 0; i < panels.Count; i++)
        {
            panels[i].SetActive(i == index);
        }
        if (index == 2) UpdateStatus();
    }

    public void BuyItem(ShopItem shopItem, bool isMagazine)
    {
        Weapon weapon = shopItem.Item as Weapon;
        if (weapon != null)
        {
            // 무기가 있거나 소모품이면 소지품에 추가
            if (weapon.consumable || WeaponController.Instance.HasWeapon(weapon.key))
            {
                if (isMagazine) Player.Instance.AddMagazine(weapon.key);
                else if (weapon.consumable
                    || Player.Instance.ItemAmount(weapon.key) < CompanionController.MAX_COMPANION)
                {
                    Player.Instance.AdjustItemAmount(weapon.key, 1);
                    UpdateCompanionSlots();
                }
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
            }
        }
        else
        {
            Player.Instance.AdjustItemAmount(shopItem.Item.key, 1);
        }
    }

    public void UpdateCompanionSlots()
    {
        foreach (var slot in companionSlots)
        {
            slot.UpdateDropdowns();
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
    }

    #region Status
    public void UpdateStatus()
    {
        bonusStat.text = $"BONUS: {Player.Instance.BonusStat}";
        statUpgradeInfos[0].text = $"{Player.Instance.MaxHp} → {Player.Instance.MaxHp + 5}";
        statUpgradeInfos[1].text = $"{Player.Instance.Speed} → {Player.Instance.Speed + 1}";
        statUpgradeInfos[2].text = $"{Player.Instance.ReloadTime}% → {Player.Instance.ReloadTime + 25}%";
        statUpgradeInfos[3].text = $"{Player.Instance.Reward}% → {Player.Instance.Reward + 25}%";
    }

    public void UpgradeStat(int index)
    {
        Player.Instance.Upgrade((Player.StatType)index);
        UpdateStatus();
    }
    #endregion

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
