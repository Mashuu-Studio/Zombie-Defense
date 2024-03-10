using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeUI : MonoBehaviour
{
    [SerializeField] private RectTransform weaponIconScrollRectTransform;
    [SerializeField] private BuildModeItemIcon weaponIconPrefab;
    [SerializeField] private RectTransform selectedWeaponPoint;
    private List<BuildModeItemIcon> weaponIcons = new List<BuildModeItemIcon>();

    [Space]
    [SerializeField] private RectTransform turretIconScrollRectTransform;
    [SerializeField] private BuildModeItemIcon turretIconPrefab;
    private List<BuildModeItemIcon> turretIcons = new List<BuildModeItemIcon>();

    [Space]
    [SerializeField] private BuildModeItemIcon[] companionIcons;

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
    }

    private int weaponIndex = 0;

    public void BuildMode(bool b)
    {
        gameObject.SetActive(b);
        if (b)
        {
            weaponIndex = 0;
            TurretController.Instance.SelectWeapon(weaponIcons[weaponIndex].Key);
            selectedWeaponPoint.anchoredPosition = new Vector2(150 * weaponIndex, 0);

            selectedCompanionIndex = -1;
            UpdateCompanions();
        }
    }

    private int selectedCompanionIndex;
    private Vector2 companionPatrolStartPos;
    private Vector2 companionPatrolEndPos;

    public void SelectCompanion(BuildModeItemIcon icon)
    {
        selectedCompanionIndex = -1;
        for (int i = 0; i < companionIcons.Length; i++)
        {
            if (companionIcons[i] == icon)
            {
                selectedCompanionIndex = i;
                break;
            }
        }
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

        // ÅÍ·¿ÀÌ¶û °ãÄ¡Áö ¾Êµµ·Ï ÇØ¾ßÇÔ.
        if (selectedCompanionIndex != -1)
        {
            Vector3 mousePos = CameraController.Instance.Cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pos = TurretController.PosToGrid(MapGenerator.RoundToInt(mousePos));

            if (Input.GetMouseButtonDown(0))
            {
                companionPatrolStartPos = pos;
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                companionPatrolEndPos = pos;
                CompanionController.Instance.SetCompanionPatrol(selectedCompanionIndex, new List<Vector2>() { companionPatrolStartPos, companionPatrolEndPos });
            }
        }
    }

    private void UpdateCompanions()
    {
        foreach (var icon in companionIcons) icon.gameObject.SetActive(false);

        for (int i = 0; i < CompanionController.Instance.Companions.Count; i++)
        {
            var data = CompanionController.Instance.Companions[i];
            companionIcons[i].gameObject.SetActive(true);
            companionIcons[i].Init("COMPANION.COMPANION");
        }
    }
}
