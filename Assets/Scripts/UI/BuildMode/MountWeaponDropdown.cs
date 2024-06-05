using System.Collections.Generic;
using UnityEngine;

public class MountWeaponDropdown : MonoBehaviour
{
    [SerializeField] private MountWeaponIcon weaponIconPrefab;
    private List<MountWeaponIcon> weaponIcons = new List<MountWeaponIcon>();

    public static int ITEM_IMAGE_MAX_WIDTH = 150;
    public static int ITEM_IMAGE_MAX_HEIGHT = 50;

    private CompanionSlot companionSlot;
    public void Init(CompanionSlot slot = null)
    {
        companionSlot = slot;
        weaponIconPrefab.gameObject.SetActive(false);

        foreach (var weapon in WeaponManager.Weapons)
        {
            if (weapon.consumable) continue;
            var weaponIcon = Instantiate(weaponIconPrefab, transform);
            weaponIcon.Init(weapon.key);
            weaponIcon.SetDropdown(this);
            weaponIcon.gameObject.SetActive(false);
            weaponIcons.Add(weaponIcon);
        }
    }

    public void SetActive(bool b, Vector2 pos)
    {
        ((RectTransform)transform).anchoredPosition = pos;
        if (b) UpdateWeaponList();
        transform.SetAsLastSibling();
        gameObject.SetActive(b);
    }

    private void UpdateWeaponList()
    {
        foreach (var icon in weaponIcons)
            icon.gameObject.SetActive(false);

        List<int> indexes = WeaponController.Instance.UsingWeaponIndexes;
        ((RectTransform)transform).sizeDelta = new Vector2(200, 50 * indexes.Count);
        foreach (var index in indexes)
        {
            weaponIcons[index].UpdateAmount();
            weaponIcons[index].gameObject.SetActive(true);
        }
    }

    public void Select(string key)
    {
        if (companionSlot == null)
        {
            BuildingController.Instance.Mount(key);
            UIController.Instance.ShowMountWeaponUI(false, Vector2.zero);
        }
        else
        {
            companionSlot.ChangeWeapon(key);
            SetActive(false, ((RectTransform)transform).anchoredPosition);
        }
    }
}
