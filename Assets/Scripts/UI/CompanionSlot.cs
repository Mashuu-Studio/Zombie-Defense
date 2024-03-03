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
    private int keyIndex;
    private int language;
    public CompanionObject Data { get { return data; } }
    private CompanionObject data;

    public void Init()
    {
        if (weaponKeys == null)
        {
            weaponKeys = new List<string>();
            WeaponManager.GetWeapons().ForEach(weapon => weaponKeys.Add(weapon.key));
        }
        weaponDropdown.AddOptions(weaponKeys);
        SetActive(false);
    }
    public void SetData(CompanionObject data)
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
        if (data.UsingWeapon != null) weaponDropdown.value = keyIndex;
    }

    public void ChangeWeapon(int index)
    {
        // 재고가 없다면 false 리턴. 원래대로 드랍다운 변경
        if (data.ChangeWeapon(weaponKeys[index]))
        {
            keyIndex = index;
            UIController.Instance.UpdateCompanions();
        }
        else weaponDropdown.value = keyIndex;
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
            string amount = WeaponManager.GetWeapon(key).infAmount ? "INF" : Player.Instance.ItemAmount(key).ToString();
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
