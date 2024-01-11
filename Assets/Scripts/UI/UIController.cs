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
        shop.gameObject.SetActive(false);
    }

    [SerializeField] private ShopUI shop;

    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider ammoSlider;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private GameObject reloadingObj;

    public void OpenShop()
    {
        shop.Open();
    }

    void Update()
    {
        hpSlider.maxValue = Player.Instance.MaxHp;
        hpSlider.value = Player.Instance.Hp;
        ammoSlider.value = WeaponController.Instance.CurWeapon.curammo;
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
