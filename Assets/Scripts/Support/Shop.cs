using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : SupportObject
{
    public override void Interact()
    {
        UIController.Instance.OpenShop();
    }
}
