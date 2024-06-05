using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class CustomToggle : MonoBehaviour
{
    [SerializeField] private GameObject offImage;
    private Toggle toggle;

    public bool isOn { 
        get { return toggle.isOn; }
        set
        {
            toggle.isOn = value;
            offImage.SetActive(!value);
        }
    }

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }
}
