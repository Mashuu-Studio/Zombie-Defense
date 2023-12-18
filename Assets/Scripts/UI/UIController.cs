using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        shop.gameObject.SetActive(false);
    }

    [SerializeField] private ShopUI shop;

    public void OpenShop()
    {
        shop.Open();
    }
}
