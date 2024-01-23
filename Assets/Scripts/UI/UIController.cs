using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get { return instance; } }
    private static UIController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }


    [Header("Game")]
    [SerializeField] private SettingUI setting;
    [SerializeField] private ShopUI shop;
    private void Start()
    {
        setting.Init();
        OpenSetting(false);
        OpenShop(false);
        levelUpView.gameObject.SetActive(false);
    }

    public void OpenShop(bool b)
    {
        shop.Open(b);
    }

    public void BuyItem(ShopItem shopItem)
    {
        shop.BuyItem(shopItem);
    }

    public void OpenSetting(bool b)
    {
        setting.gameObject.SetActive(b);
    }

    #region Status
    [Header("Status")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider ammoSlider;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI lvText;
    [SerializeField] private GameObject reloadingObj;
    [SerializeField] private TextMeshProUGUI moneyText;

    void Update()
    {
        hpSlider.maxValue = Player.Instance.MaxHp;
        hpSlider.value = Player.Instance.Hp;
        expSlider.maxValue = Player.Instance.MaxExp;
        expSlider.value = Player.Instance.Exp;
        lvText.text = Player.Instance.Lv.ToString();
        ammoSlider.value = WeaponController.Instance.CurWeapon.curammo;
        moneyText.text = $"$ {Player.Instance.Money}";
    }

    public void SwitchWeapon()
    {
        ammoSlider.maxValue = WeaponController.Instance.CurWeapon.ammo;
        ammoSlider.value = WeaponController.Instance.CurWeapon.curammo;
        weaponNameText.text = WeaponController.Instance.CurWeapon.name;
        Reloading(false);
    }

    public void Reloading(bool b)
    {
        reloadingObj.SetActive(b);
    }
    #endregion

    #region Level
    [Header("Level Up")]
    [SerializeField] private GameObject levelUpView;
    [SerializeField] private TextMeshProUGUI[] upgradeInfos;

    public void LevelUp()
    {
        upgradeInfos[0].text = $"{Player.Instance.MaxHp} ¡æ {Player.Instance.MaxHp + 5}";
        upgradeInfos[1].text = $"{Player.Instance.Speed} ¡æ {Player.Instance.Speed + 1}";
        upgradeInfos[2].text = $"{Player.Instance.Reload}% ¡æ {Player.Instance.Reload + 25}%";
        upgradeInfos[3].text = $"{Player.Instance.Reward}% ¡æ {Player.Instance.Reward + 25}%";

        levelUpView.gameObject.SetActive(true);
        GameController.Instance.LevelUpPause(true);
    }

    public void UpgradeStat(int index)
    {
        Player.Instance.Upgrade((Player.StatType)index);
        levelUpView.gameObject.SetActive(false);
        GameController.Instance.LevelUpPause(false);
    }
    #endregion

    #region Round
    [Space]
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI roundTimeText;
    [SerializeField] private GameObject startRoundButton;

    public void UpdateRoundTime(int time)
    {
        roundTimeText.text = time.ToString();
    }

    public void StartRound()
    {
        roundTimeText.gameObject.SetActive(true);
        startRoundButton.SetActive(false);
    }

    public void EndRound()
    {
        roundTimeText.gameObject.SetActive(false);
        startRoundButton.SetActive(true);
    }
    #endregion
}
