using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using TMPro;

public class CompanionSlot : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private Image weaponImage;
    [SerializeField] private MountWeaponDropdown weaponDropdown;
    [SerializeField] private List<CustomToggle> patrolTypes;

    private static List<string> weaponKeys;
    private static List<string> patrolKeys;
    private string weaponKey;
    public CompanionObject Data { get { return data; } }
    private CompanionObject data;

    public void Init()
    {
        if (weaponKeys == null)
        {
            weaponKeys = new List<string>();
            WeaponManager.GetWeapons().ForEach(weapon => weaponKeys.Add(weapon.key));
        }

        if (patrolKeys == null)
        {
            patrolKeys = new List<string>();
            var array = System.Enum.GetValues(typeof(CompanionObject.PatrolType));
            foreach (var type in array)
            {
                patrolKeys.Add("GAME.SHOP.COMPANION.PATROL." + type.ToString().ToUpper());
            }
        }
        weaponDropdown.Init(this);
        SetActive(false);
    }
    public void SetData(CompanionObject data)
    {
        this.data = data;
        image.sprite = SpriteManager.GetSprite(data.Key);
        SetActive(true);
    }

    public void SetActive(bool b)
    {
        gameObject.SetActive(b);
        weaponDropdown.gameObject.SetActive(false);
        if (b) UpdateInfo();
        else data = null;
    }

    public void OpenDropdown()
    {
        transform.SetAsLastSibling();
        weaponDropdown.SetActive(!weaponDropdown.gameObject.activeSelf, Vector2.zero);
    }

    public void ChangeWeapon(string key)
    {
        if (data.ChangeWeapon(key))
        {
            ChangeWeaponKey(key);
        }
    }

    public void ChangeWeaponKey(string key)
    {
        weaponKey = key;
        weaponImage.sprite = SpriteManager.GetSprite(key.Replace("WEAPON.", "UI.")); if (weaponImage.sprite != null) weaponImage.rectTransform.sizeDelta = new Vector2(weaponImage.sprite.rect.width / weaponImage.sprite.rect.height * 50, 50);
        float wRatio = weaponImage.rectTransform.sizeDelta.x / MountWeaponDropdown.ITEM_IMAGE_MAX_WIDTH;
        float hRatio = weaponImage.rectTransform.sizeDelta.y / MountWeaponDropdown.ITEM_IMAGE_MAX_HEIGHT;
        // 이미지가 지정한 크기를 벗어났을 경우 크기를 맞춰줌.
        if (wRatio > 1 || hRatio > 1)
        {
            if (wRatio > hRatio) weaponImage.rectTransform.sizeDelta /= wRatio;
            else weaponImage.rectTransform.sizeDelta /= hRatio;
        }
    }

    public void ChangePatrolType(int index)
    {
        if (index != 4 && patrolTypes[index].isOn) data.SetPatrolType((CompanionObject.PatrolType)index);
    }

    public void UpdateInfo()
    {
        armorText.text = $"{data.Def}";
        hpText.text = $"{data.Hp}";
        if (data.UsingWeapon != null
            && weaponKey != data.UsingWeapon.key)
        {
            weaponKey = data.UsingWeapon.key;
            ChangeWeaponKey(data.UsingWeapon.key);
        }
        patrolTypes[(int)data.PType].isOn = true;
    }

    public void FillArmor()
    {
        data.FillArmor();
        UpdateInfo();
    }

    public void Heal()
    {
        data.Heal();
        UpdateInfo();
    }

    public void Fire()
    {
        CompanionController.Instance.RemoveCompanion(data);
    }
}
