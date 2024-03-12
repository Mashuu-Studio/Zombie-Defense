using System.Collections.Generic;
using UnityEngine;

public class MountWeaponDropdown : MonoBehaviour
{
    [SerializeField] private MountWeaponIcon weaponIconPrefab;
    private List<MountWeaponIcon> weaponIcons = new List<MountWeaponIcon>();

    public static int ITEM_IMAGE_MAX_WIDTH = 150;
    public static int ITEM_IMAGE_MAX_HEIGHT = 50;
    // Start is called before the first frame update
    public void Init()
    {
        weaponIconPrefab.gameObject.SetActive(false);

        foreach (var weapon in WeaponManager.Weapons)
        {
            if (weapon.consumable) continue;
            var weaponIcon = Instantiate(weaponIconPrefab, transform);
            weaponIcon.Init(weapon.key);
            weaponIcon.gameObject.SetActive(false);
            weaponIcons.Add(weaponIcon);
        }
    }

    public void SetActive(bool b, Vector2 pos)
    {
        ((RectTransform)transform).anchoredPosition = CameraController.Instance.Cam.WorldToScreenPoint(pos);
        UpdateWeaponList();
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
}
