using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

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
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        setting.Init();
        shop.Init();
        buildModeUI.Init();
        mountWeaponDropdown.Init();
        InitItemInfo();
        OpenSetting(false);
        OpenShop(false);
        buildModeUI.gameObject.SetActive(false);
    }

    [Header("Scene")]
    [SerializeField] private GameObject[] scenes;
    [SerializeField] private Canvas canvas;

    public void ChangeScene(int index)
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i].SetActive(i == index);
        }

        if (index == 2) canvas.worldCamera = CameraController.Instance.Cam;
    }

    #region UI
    [Header("UI")]
    [SerializeField] private CanvasScaler scaler;
    [SerializeField] private FloatingDescription floatingDescription;

    public void SetDescription(Vector3 pos, string key)
    {
        floatingDescription.SetDescription(ScalingPos(pos), key);
    }

    public void MoveDescription(Vector3 pos)
    {
        floatingDescription.MoveDescription(ScalingPos(pos));
    }

    public static Vector2 ScalingPos(Vector2 pos)
    {
        float ratio = Instance.scaler.referenceResolution.y / Screen.height;
        return pos * ratio;
    }

    public static bool PointOverUI()
    {
        if (EventSystem.current == null) return false;
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.layer == LayerMask.NameToLayer("UI")) return true;
        }
        return false;
    }

    public static bool PointOverUI(GameObject go)
    {
        if (EventSystem.current == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == go) return true;
        }
        return false;
    }
    #endregion

    #region Game
    [Header("Game")]
    [SerializeField] private SettingUI setting;
    [SerializeField] private ShopUI shop;
    [SerializeField] private GameObject shopButton;

    public void StartGame()
    {
        foreach (var weapon in WeaponManager.Weapons)
        {
            if (weapon.consumable) continue;
            itemInfos[weapon.key].gameObject.SetActive(WeaponController.Instance.HasWeapon(weapon.key));
        }
    }

    #region Shop

    public void OnOffShop()
    {
        OpenShop(!shop.gameObject.activeSelf);
    }

    public void OpenShop(bool b)
    {
        shop.Open(b);
    }

    public void BuyItem(ShopItem shopItem, bool isMagazine = false)
    {
        shop.BuyItem(shopItem, isMagazine);
    }

    public void RemoveCompanion(CompanionObject companion)
    {
        shop.RemoveCompanion(companion);
    }

    public void UpdateCompanions()
    {
        shop.UpdateCompanionSlots();
    }
    #endregion

    #region BuildMode
    [SerializeField] private BuildModeUI buildModeUI;
    [SerializeField] private GameObject buildButton;
    [SerializeField] private MountWeaponDropdown mountWeaponDropdown;
    public void OnOffBuildMode()
    {
        bool isOn = !buildModeUI.gameObject.activeSelf;
        BuildMode(isOn);
    }

    public void BuildMode(bool b)
    {
        buildModeUI.BuildMode(b);
        if (b == false) mountWeaponDropdown.SetActive(false, Vector2.zero);
        TurretController.Instance.ChangeBulidMode(b);
        MapGenerator.Instance.BuildMode(b);
    }

    public void SelectCompanion(BuildModeItemIcon icon)
    {
        buildModeUI.SelectCompanion(icon);
    }

    public void ShowMountWeaponUI(bool b, Vector2 pos)
    {
        mountWeaponDropdown.SetActive(b, pos);
    }
    #endregion

    #region Setting
    public void OpenSetting(bool b)
    {
        setting.gameObject.SetActive(b);
    }

    public void LoadResolutionInfo()
    {
        setting.LoadResolutionInfo();
    }
    #endregion
    #endregion

    #region Status
    [Header("Status")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI maxAmmoText;
    [SerializeField] private TextMeshProUGUI magazineText;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI lvText;
    [SerializeField] private TextMeshProUGUI moneyText;

    void Update()
    {
        if (GameController.Instance.GameStarted == false) return;

        hpText.text = $"{Player.Instance.Hp}/{Player.Instance.MaxHp}";
        expSlider.maxValue = Player.Instance.MaxExp;
        expSlider.value = Player.Instance.Exp;
        lvText.text = Player.Instance.Lv.ToString();
        granadeAmount.text = Player.Instance.ItemAmount("WEAPON.GRANADE").ToString();
        moneyText.text = $"$ {Player.Instance.Money}";

        UpdateMagazine();
    }

    [Header("WEAPON")]
    [SerializeField] private Image[] weaponImages;
    [SerializeField] private TextMeshProUGUI granadeAmount;

    public void UpdateWeaponImage()
    {
        // UI 이미지로 키 변환
        string[] weaponKeys = WeaponController.Instance.PrevCurNextWeaponKeys;
        for (int i = 0; i < weaponKeys.Length; i++)
        {
            bool empty = string.IsNullOrEmpty(weaponKeys[i]);
            weaponImages[i].gameObject.SetActive(!empty);
            if (empty) continue;

            string uiKey = weaponKeys[i].Replace("WEAPON.", "UI.");
            Sprite sprite = SpriteManager.GetSprite(uiKey);
            Vector2 size = GetWeaponImageSize(sprite);

            weaponImages[i].sprite = sprite;
            ((RectTransform)weaponImages[i].transform).sizeDelta = size;
        }
    }

    private Vector2 GetWeaponImageSize(Sprite sprite)
    {
        if (sprite == null) return Vector2.zero;
        Vector2 size = new Vector2(sprite.rect.width, sprite.rect.height);
        size = size / sprite.pixelsPerUnit * 80;
        return size;
    }

    public void SwitchWeapon()
    {
        Weapon weapon = WeaponController.Instance.CurWeapon;
        ammoText.text = weapon.curammo.ToString();
        maxAmmoText.text = weapon.ammo.ToString();
        UpdateWeaponImage();
        Reloading(false);
    }

    public void UpdateAmmo(int ammo)
    {
        ammoText.text = ammo.ToString();
    }

    private void UpdateMagazine()
    {
        int mag = Player.Instance.GetMagazine(WeaponController.Instance.CurWeapon.key);
        magazineText.text = (mag == -1) ? "INF" : mag.ToString();
    }

    public void Reloading(bool b)
    {
        if (b) ammoText.text = "RELOAD";
        else ammoText.text = WeaponController.Instance.CurWeapon.curammo.ToString();

    }

    [Header("Item")]
    [SerializeField] private Transform weaponInfoParent;
    [SerializeField] private Transform turretInfoParent;
    [SerializeField] private ItemInfoUI itemInfoPrefab;
    private Dictionary<string, ItemInfoUI> itemInfos;

    private void InitItemInfo()
    {
        itemInfoPrefab.gameObject.SetActive(false);

        itemInfos = new Dictionary<string, ItemInfoUI>();
        foreach (var weapon in WeaponManager.Weapons)
        {
            if (weapon.consumable) continue;
            var weaponInfoUI = Instantiate(itemInfoPrefab, weaponInfoParent);
            weaponInfoUI.SetInfo(SpriteManager.GetSprite(weapon.key));
            itemInfos.Add(weapon.key, weaponInfoUI);
            weaponInfoUI.gameObject.SetActive(false);
        }

        foreach (var turret in TurretManager.Turrets)
        {
            var turretInfoUI = Instantiate(itemInfoPrefab, turretInfoParent);
            turretInfoUI.SetInfo(SpriteManager.GetSprite(turret.key));
            itemInfos.Add(turret.key, turretInfoUI);
            turretInfoUI.gameObject.SetActive(true);
        }
    }

    public void AddItem(string key)
    {
        if (itemInfos.ContainsKey(key))
            itemInfos[key].gameObject.SetActive(true);
    }

    public void UpdateItemAmount(string key, int amount)
    {
        if (itemInfos.ContainsKey(key))
            itemInfos[key].UpdateInfo(amount);
    }
    #endregion

    #region Round
    [Space]
    [SerializeField] private TextMeshProUGUI roundText;
    [SerializeField] private TextMeshProUGUI roundTimeText;
    [SerializeField] private GameObject startRoundButton;

    public void UpdateRoundTime(int time)
    {
        TimeSpan res = TimeSpan.FromSeconds(time);
        roundTimeText.text = res.ToString("mm':'ss");
    }

    public void StartRound()
    {
        roundText.text = $"ROUND {RoundController.Instance.Round}";
        roundTimeText.gameObject.SetActive(true);
        startRoundButton.SetActive(false);
        shopButton.SetActive(false);
        OpenShop(false);
        BuildMode(false);
        buildButton.SetActive(false);
    }

    public void EndRound()
    {
        roundText.text = $"ROUND {RoundController.Instance.Round}";
        roundTimeText.gameObject.SetActive(false);
        startRoundButton.SetActive(true);
        shopButton.SetActive(true);
        buildButton.SetActive(true);
    }
    #endregion
}
