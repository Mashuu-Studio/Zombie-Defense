using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

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
    public bool UIisRemains { get { return openedUIs.Count > 0; } }
    private List<GameObject> openedUIs = new List<GameObject>();

    public void OffOpenedUI()
    {
        if (!UIisRemains) return;
        var ui = openedUIs[openedUIs.Count - 1];
        if (ui == shop.gameObject) OpenShop(false);
        else if (ui == buildModeUI.gameObject) BuildMode(false);
    }

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

    public void Title(bool b)
    {
        setting.Title(b);
    }
    #endregion

    #region Game
    [Header("Game")]
    [SerializeField] private SettingUI setting;
    [SerializeField] private ShopUI shop;
    [SerializeField] private GameObject shopButton;

    public void StartGame()
    {
        difficultyText.SetEntry(GameController.Instance.DifficultyKey);
        foreach (var weapon in WeaponManager.Weapons)
        {
            if (weapon.consumable) continue;
            itemInfos[weapon.key].gameObject.SetActive(WeaponController.Instance.HasWeapon(weapon.key));
        }
    }

    public void SelectDifficulty(int index)
    {
        GameController.Instance.SelectDifficulty(index);
    }

    #region Shop

    public void OnOffShop()
    {
        OpenShop(!shop.gameObject.activeSelf);
    }

    public void OpenShop(bool b)
    {
        shop.Open(b);
        if (b) openedUIs.Add(shop.gameObject);
        else openedUIs.Remove(shop.gameObject);
    }

    public void BuyItem(ShopItem shopItem, bool isMagazine = false)
    {
        shop.BuyItem(shopItem, isMagazine);
    }

    public void UpdateCompanionInfo()
    {
        shop.UpdateCompanionInfo();
    }

    public void RemoveCompanion(CompanionObject companion)
    {
        shop.RemoveCompanion(companion);
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
        if (b) openedUIs.Add(buildModeUI.gameObject);
        else openedUIs.Remove(buildModeUI.gameObject);
        if (b == false) mountWeaponDropdown.SetActive(false, UIController.ScalingPos(Camera.main.WorldToScreenPoint(Vector2.zero)));
        BuildingController.Instance.ChangeBulidMode(b);
        MapGenerator.Instance.BuildMode(b);
    }

    public void SelectCompanion(BuildModeItemIcon icon)
    {
        buildModeUI.SelectCompanion(icon);
    }

    public void UpdateBuildmodeCompanions()
    {
        buildModeUI.UpdateCompanions();
    }

    public void ShowMountWeaponUI(bool b, Vector2 pos)
    {
        mountWeaponDropdown.SetActive(b, ScalingPos(Camera.main.WorldToScreenPoint(pos)));
    }
    #endregion

    #region Setting
    public void OpenSetting(bool b)
    {
        setting.gameObject.SetActive(b);
    }
    #endregion
    #endregion

    #region Status
    [Header("Status")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI armorText;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI magazineText;
    [SerializeField] private TextMeshProUGUI moneyText;

    void Update()
    {
        if (GameController.Instance.GameStarted == false) return;

        hpText.text = $"{Player.Instance.Hp}";
        armorText.text = $"{Player.Instance.Def}";
        granadeAmount.text = Player.Instance.ItemAmount("WEAPON.GRENADE").ToString();
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
    [SerializeField] private Transform buildingInfoParent;
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

        foreach (var building in BuildingManager.Buildings)
        {
            var buildingInfoUI = Instantiate(itemInfoPrefab, buildingInfoParent);
            buildingInfoUI.SetInfo(SpriteManager.GetSprite(building.key));
            itemInfos.Add(building.key, buildingInfoUI);
            buildingInfoUI.gameObject.SetActive(true);
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
    [SerializeField] private LocalizeStringEvent difficultyText;
    [SerializeField] private GameObject startRoundObject;

    public void UpdateRoundTime(int time)
    {
        TimeSpan res = TimeSpan.FromSeconds(time);
        roundTimeText.text = res.ToString("mm':'ss");
    }

    public void StartRound()
    {
        roundText.text = $"ROUND {RoundController.Instance.Round}";
        roundTimeText.gameObject.SetActive(true);
        startRoundObject.SetActive(false);
        shopButton.SetActive(false);
        buildButton.SetActive(false);

        while (UIisRemains) OffOpenedUI();
    }

    public void EndRound()
    {
        roundText.text = $"ROUND {RoundController.Instance.Round}";
        roundTimeText.gameObject.SetActive(false);
        startRoundObject.SetActive(true);
        shopButton.SetActive(true);
        buildButton.SetActive(true);
    }
    #endregion
}
