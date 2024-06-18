using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using TMPro;

public class ShopItemStat : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent statusName;
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI statText;

    private void Awake()
    {
        slider.maxValue = 1;
        slider.minValue = 0;
        slider.wholeNumbers = false;
    }

    public void SetStatus(string nameEntry, float value, string stat)
    {
        statusName.SetEntry(nameEntry);
        slider.value = value;
        statText.text = stat;
    }
}
