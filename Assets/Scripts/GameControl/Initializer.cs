using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    private void Awake()
    {
        EnemyManager.Init();
        WeaponManager.Init();
        TurretManager.Init();
        SpriteManager.Init();
    }
}
