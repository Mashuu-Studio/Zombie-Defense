using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurretBuildIcon : BuildModeItemIcon
{
    [SerializeField] protected DescriptionIcon itemIcon;
    [SerializeField] protected TextMeshProUGUI itemAmount;

    public override void Init(string key)
    {
        base.Init(key);
        itemIcon.SetIcon(key);
    }

    private void Update()
    {
        if (Player.Instance != null)
            itemAmount.text = Player.Instance.ItemAmount(key).ToString();
    }

    public override void Select()
    {
        TurretController.Instance.SelectBuildingTurret(key);
    }
}
