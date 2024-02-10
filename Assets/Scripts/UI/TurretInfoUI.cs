using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurretInfoUI : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI amountText;

    public void SetInfo(Sprite sprite)
    {
        image.sprite = sprite;
        amountText.text = "0";
    }

    public void UpdateInfo(int amount)
    {
        amountText.text = amount.ToString();
    }
}
