using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initializer : MonoBehaviour
{
    private void Awake()
    {
        SpriteManager.Init();
        EnemyManager.Init();
        WeaponManager.Init();
        BuildingManager.Init();
        CompanionManager.Init();
        ItemManager.Init();
        RoundManager.Init();
    }

    private void Start()
    {
        SoundController.Instance.Init();
        PoolController.Instance.Init();
        GameController.Instance.GoTo(SceneController.Scene.TITLE);
    }
}
