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


    [SerializeField] private ShopUI shop;
    [SerializeField] private SettingUI setting;
    private void Start()
    {
        setting.Init();
        OpenSetting(false);
        OpenShop(false);
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
}
