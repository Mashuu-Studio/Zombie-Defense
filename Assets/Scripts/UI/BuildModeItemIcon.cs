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
        itemIcon.SetIcon(key);
    }

    private void Update()
    {
        if (Player.Instance != null)
            itemAmount.text = Player.Instance.ItemAmount(key).ToString();
    }

    public void Select()
    {
        TurretController.Instance.SelectBuildingTurret(key);
    }
}
