using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public void Open()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}