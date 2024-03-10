using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildModeItemIcon : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private DescriptionIcon itemIcon;
    [SerializeField] private TextMeshProUGUI itemAmount;

    public string Key { get { return key; } }
    private string key;
    public void Init(string key)
    {
        this.key = key;
        itemImage.sprite = SpriteManager.GetSprite(key);
        if (!key.Contains("COMPANION.")) itemIcon.SetIcon(key);
        else itemAmount.text = "";
    }

    private void Update()
    {
        if (Player.Instance != null && !key.Contains("COMPANION."))
            itemAmount.text = Player.Instance.ItemAmount(key).ToString();
    }

    public void Select()
    {
        if (key.Contains("TURRET.")) TurretController.Instance.SelectBuildingTurret(key);
        else if (key.Contains("COMPANION.")) UIController.Instance.SelectCompanion(this); 
    }
}
