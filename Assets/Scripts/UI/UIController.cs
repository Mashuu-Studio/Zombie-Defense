using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
        errorLog.gameObject.SetActive(false);
        tutorialUI.Init();
        shop.Init();
        library.Init();
        buildModeUI.Init();
        mountWeaponDropdown.Init();
        setting.Init();
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
    [SerializeField] private ErrorLog errorLog;
    [SerializeField] private CanvasScaler scaler;
    [SerializeField] private FloatingDescription floatingDescription;
    [SerializeField] private LibraryUI library;
    public bool UIisRemains { get { return openedUIs.Count > 0; } }
    private List<GameObject> openedUIs = new List<GameObject>();

    public void OffOpenedUI()
    {
        if (!UIisRemains) return;
        var ui = openedUIs[openedUIs.Count - 1];
        if (ui == shop.gameObject) OpenShop(false);
        else if (ui == buildModeUI.gameObject) BuildMode(false);
        else if (ui == library.gameObject) Libary(false);
    }

    public void SetDescription(Vector3 pos, string key)
    {
        floatingDescription.SetDescription(ScalingPos(pos), key);
    }

    public void MoveDescription(Vector3 pos)
    {
        floatingDescription.MoveDescription(ScalingPos(pos));
    }

    public void ErrorLog(string log, string str)
    {
        errorLog.Log(log, str);
    }

    #region Library
    public void OnOffLibary()
    {
        Libary(!library.gameObject.activeSelf);
    }

    public void Libary(bool b)
    {
        library.SetActive(b);

        if (b)
        {
            tutorialUI.ShowTutorial(TutorialUI.TutorialStep.LIBRARY);
            openedUIs.Add(library.gameObject);
        }
        else openedUIs.Remove(library.gameObject);
    }

    public void UpdateLibraryDescription(string key)
    {
        library.UpdateDescription(key);
    }
    #endregion

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
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private LocalizeStringEvent loseStringEvent;

    [Space]
    [SerializeField] private ShopUI shop;
    [SerializeField] private GameObject shopButton;


    public void StartGame()
    {
        while (UIisRemains) OffOpenedUI();
        winUI.SetActive(false);
        loseUI.SetActive(false);

        difficultyText.SetEntry(GameController.Instance.DifficultyKey);
        foreach (var weapon in WeaponManager.Weapons)
        {
            if (weapon.consumable) continue;
            itemInfos[weapon.key].gameObject.SetActive(WeaponController.Instance.HasWeapon(weapon.key));
        }

        // Ʃ�丮���� �� �ôٸ� ����
        tutorialUI.ShowTutorial(TutorialUI.TutorialStep.GAME);
    }

    public void Endless()
    {
        winUI.SetActive(false);
    }

    public void GameOver(bool isWin)
    {
        if (!isWin)
        {
            int kill = Player.Instance.KillCount;
            string difficult = difficultyText.StringReference.GetLocalizedString();
            if (RoundController.Instance.Endless)
            {
                loseStringEvent.SetEntry("GAME.OVER.ENDLESS");
                loseStringEvent.StringReference.Arguments = new object[] { time, kill, difficult };
            }
            else
            {
                int round = RoundController.Instance.Round;
                loseStringEvent.SetEntry("GAME.OVER.LOSE");
                loseStringEvent.StringReference.Arguments = new object[] { round, kill, difficult };
            }
        }
        winUI.SetActive(isWin);
        loseUI.SetActive(!isWin);
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
        if (b)
        {
            tutorialUI.ShowTutorial(TutorialUI.TutorialStep.SHOP);
            openedUIs.Add(shop.gameObject);
        }
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
    [Space]
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
        if (b)
        {
            tutorialUI.ShowTutorial(TutorialUI.TutorialStep.BUILDMODE);
            openedUIs.Add(buildModeUI.gameObject);
        }
        else
        {
            openedUIs.Remove(buildModeUI.gameObject);
            mountWeaponDropdown.SetActive(false, ScalingPos(Camera.main.WorldToScreenPoint(Vector2.zero)));
        }
        BuildingController.Instance.ChangeBulidMode(b);
        MapGenerator.Instance.BuildMode(b);
    }

    public void UnselectCompanion()
    {
        buildModeUI.UnselectCompanion();
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

    #region Tutorial
    [SerializeField] private TutorialUI tutorialUI;

    // ���� �������� ���̺� ��� ����.
    public void ChangeTutorialProgress(bool b)
    {
        for (int i = 0; i < GameSetting.Instance.SettingInfo.readTutorial.Length; i++)
        {
            GameSetting.Instance.SettingInfo.readTutorial[i] = !b;
        }
        GameSetting.Instance.SaveSetting();
    }
    
    public void SetTutorialProgress()
    {
        bool isOn = false;
        for (int i = 0; i < GameSetting.Instance.SettingInfo.readTutorial.Length; i++)
        {
            if (!GameSetting.Instance.SettingInfo.readTutorial[i])
            {
                isOn = true;
                break;
            }
        }
        setting.SetTutorialProgress(isOn);
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
        if (GameController.Instance.GameProgress == false) return;

        UpdatePlayerInfo();
    }

    public void UpdatePlayerInfo()
    {
        hpText.text = Player.Instance.Hp.ToString();
        armorText.text = Player.Instance.Def.ToString();
        granadeAmount.text = Player.Instance.ItemAmount("WEAPON.GRENADE").ToString();
        moneyText.text = "$" + Player.Instance.Money.ToString();

        UpdateMagazine();
    }

    [Header("WEAPON")]
    [SerializeField] private Image[] weaponImages;
    [SerializeField] private TextMeshProUGUI granadeAmount;

    public void UpdateWeaponImage()
    {
        // UI �̹����� Ű ��ȯ
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

    private int time;
    public void UpdateRoundTime(int time)
    {
        this.time = time;
        TimeSpan res = TimeSpan.FromSeconds(time);
        roundTimeText.text = res.ToString("mm':'ss");
    }

    public void StartRound()
    {
        roundText.text = RoundController.Instance.Endless ? "ENDLESS" :$"ROUND {RoundController.Instance.Round}";
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
