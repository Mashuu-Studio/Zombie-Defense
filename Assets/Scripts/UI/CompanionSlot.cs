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
    [SerializeField] private TMP_Dropdown patrolDropdown;

    private static List<string> weaponKeys;
    private static List<string> patrolKeys;
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

        if (patrolKeys == null)
        {
            patrolKeys = new List<string>();
            var array = System.Enum.GetValues(typeof(CompanionObject.PatrolType));
            foreach (var type in array)
            {
                patrolKeys.Add("GAME.SHOP.COMPANION.PATROL." + type.ToString().ToUpper());
            }
        }

        weaponDropdown.AddOptions(weaponKeys);
        patrolDropdown.AddOptions(patrolKeys);
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
        if (b)
        {
            UpdateInfo();
            UpdateDropdowns();
        }
        else data = null;
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

    public void ChangePatrolType(int index)
    {
        data.SetPatrolType((CompanionObject.PatrolType)index);
    }

    private void Update()
    {
        UpdateInfo();
        if (language != GameSetting.Instance.CurrentLanguage)
        {
            UpdateDropdowns();
            language = GameSetting.Instance.CurrentLanguage;
        }
    }
    private void UpdateInfo()
    {
        hpText.text = $"HP: {Data.Hp} / {Data.MaxHp}";
        if (data.UsingWeapon != null) weaponDropdown.value = keyIndex;
    }

    public void UpdateDropdowns()
    {
        for (int i = 0; i < weaponKeys.Count; i++)
        {
            string key = weaponKeys[i];
            string amount = WeaponManager.GetWeapon(key).infAmount ? "INF" : Player.Instance.ItemAmount(key).ToString();
            LocalizedString localizedString = new LocalizedString("Item Name Table", key);
            weaponDropdown.options[i].text = $"{localizedString.GetLocalizedString()} : {amount}";
        }
        weaponDropdown.captionText.text = weaponDropdown.options[weaponDropdown.value].text;

        for (int i = 0; i < patrolKeys.Count; i++)
        {
            string key = patrolKeys[i];
            LocalizedString localizedString = new LocalizedString("UI String Table", key);
            patrolDropdown.options[i].text = localizedString.GetLocalizedString();
        }
        patrolDropdown.captionText.text = patrolDropdown.options[patrolDropdown.value].text;
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
