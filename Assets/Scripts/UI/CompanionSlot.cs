using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CompanionSlot : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI curretWeapon;

    public CompanionObject Data { get { return data; } }
    private CompanionObject data;
    public void Init(CompanionObject data)
    {
        this.data = data;
        SetActive(true);
    }

    public void SetActive(bool b)
    {
        gameObject.SetActive(b);
        if (b == false) data = null;
    }
}
