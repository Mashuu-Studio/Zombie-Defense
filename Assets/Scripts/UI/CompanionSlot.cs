using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using TMPro;

public class CompanionSlot : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TMP_Dropdown weaponDropdown;

    private static List<string> weaponKeys;
    private int language;
    private void Start()
    {
        if (weaponKeys == null)
        {
            weaponKeys = new List<string>();
            WeaponManager.GetWeapons().ForEach(weapon => weaponKeys.Add(weapon.key));
        }
        weaponDropdown.AddOptions(weaponKeys);
    }
    public CompanionObject Data { get { return data; } }
    private CompanionObject data;
    public void Init(CompanionObject data)
    {
        this.data = data;        
        SetActive(true);
    }

    public void SetActive(bool b)
    {
        gameObject.SetActive(b);
        if (b)
        {
            UpdateInfo();
            UpdateDropdown();
        }
        else data = null;
    }

    private void UpdateInfo()
    {
        hpText.text = $"HP: {Data.Hp} / {Data.MaxHp}";
    }

    public void ChangeWeapon(int index)
    {
        string key = weaponKeys[index];
        data.ChangeWeapon(key);
    }

    private void Update()
    {
        UpdateInfo();
        if (language != GameSetting.Instance.CurrentLanguage)
        {
            UpdateDropdown();
            language = GameSetting.Instance.CurrentLanguage;
        }
    }

    public void UpdateDropdown()
    {
        for (int i = 0; i < weaponKeys.Count; i++)
        {
            string key = weaponKeys[i];
            int amount = Player.Instance.ItemAmount(key);
            LocalizedString localizedString = new LocalizedString("Item Name Table", key);
            weaponDropdown.options[i].text = $"{localizedString.GetLocalizedString()} : {amount}";
        }
        weaponDropdown.captionText.text = weaponDropdown.options[weaponDropdown.value].text;
    }

    public void Heal()
    {
        data.Heal();
        UpdateInfo();
    }
}
