using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get { return instance; } }
    private static GameController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        Title();
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Map Boundary"));
    }

    public bool Pause { get { return pause || levelUpPause; } }
    private bool pause;
    private bool levelUpPause;

    public bool GameStarted { get { return gameStarted; } }
    private bool gameStarted;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame(!pause);
            UIController.Instance.OpenSetting(pause);
        }
    }

    public void PauseGame(bool b)
    {
        pause = b;
    }

    public void LevelUpPause(bool b)
    {
        levelUpPause = b;
    }
    public void Title()
    {
        gameStarted = false;
        UIController.Instance.ChangeScene(0);
        RoundController.Instance.EndGame();
    }

    public void StartGame()
    {
        gameStarted = true;
        UIController.Instance.ChangeScene(1);
        MapGenerator.Instance.StartGame();
        RoundController.Instance.EndGame();
        WeaponController.Instance.StartGame();
        TurretController.Instance.StartGame();

        Player.Instance.Init();
    }

    public void StartRound()
    {
        RoundController.Instance.StartRound();
        UIController.Instance.StartRound();
    }

    public void EndRound()
    {
        UIController.Instance.EndRound();
        EnemyController.Instance.EndRound();
    }
}
