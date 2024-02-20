using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeUI : MonoBehaviour
{
    [SerializeField] private RectTransform weaponIconScrollRectTransform;
    [SerializeField] private RectTransform turretIconScrollRectTransform;
    [SerializeField] private BuildModeItemIcon weaponIconPrefab;
    [SerializeField] private BuildModeItemIcon turretIconPrefab;
    private List<BuildModeItemIcon> weaponIcons = new List<BuildModeItemIcon>();
    private List<BuildModeItemIcon> turretIcons = new List<BuildModeItemIcon>();
    [SerializeField] private RectTransform selectedWeaponPoint;

    private void Awake()
    {
        weaponIconPrefab.gameObject.SetActive(false);
        turretIconPrefab.gameObject.SetActive(false);
    }

    public void Init()
    {
        turretIcons.ForEach(turretIcon => Destroy(turretIcon.gameObject));
        turretIcons.Clear();

        int count = 0;
        foreach (var weapon in WeaponManager.Weapons)
        {
            if (weapon.consumable) continue;
            var weaponIcon = Instantiate(weaponIconPrefab, weaponIconScrollRectTransform);
            weaponIcon.Init(weapon.key);
            weaponIcon.gameObject.SetActive(true);
            weaponIcons.Add(weaponIcon);
            count++;
        }

        count = 0;
        foreach (var turret in TurretManager.Turrets)
        {
            var turretIcon = Instantiate(turretIconPrefab, turretIconScrollRectTransform);
            turretIcon.Init(turret.key);
            turretIcon.gameObject.SetActive(true);
            turretIcons.Add(turretIcon);
            count++;
        }
        turretIconScrollRectTransform.sizeDelta = new Vector2(150 * count, turretIconScrollRectTransform.sizeDelta.y);
    }

    private int weaponIndex = 0;

    public void BuildMode(bool b)
    {
        gameObject.SetActive(b);
        if (b) weaponIndex = 0;
        selectedWeaponPoint.anchoredPosition = new Vector2(150 * weaponIndex, 0);
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            weaponIndex += (scroll > 0) ? -1 : 1;
            if (weaponIndex < 0) weaponIndex = weaponIcons.Count - 1;
            if (weaponIndex >= weaponIcons.Count) weaponIndex = 0;
            TurretController.Instance.SelectWeapon(weaponIcons[weaponIndex].Key);
            selectedWeaponPoint.anchoredPosition = new Vector2(150 * weaponIndex, 0);
        }
    }
}
