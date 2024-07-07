using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BuildIcon : BuildModeItemIcon
{
    [SerializeField] protected DescriptionIcon itemIcon;
    [SerializeField] protected TextMeshProUGUI itemAmount;
    [SerializeField] protected TextMeshProUGUI priceText;

    public override void Init(string key)
    {
        base.Init(key);
        itemIcon.SetIcon(key);
        priceText.text = "$" + BuildingManager.GetBuilding(key).price.ToString();
    }

    private void Update()
    {
        if (Player.Instance != null)
            itemAmount.text = Player.Instance.ItemAmount(key).ToString();
    }

    public override void Select()
    {
        UIController.Instance.UnselectCompanion();
        BuildingController.Instance.SelectBuildingOnBuildMode(key);
    }
}
