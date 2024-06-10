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
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Enemy"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Map Boundary"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Companion"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Companion"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Companion"), LayerMask.NameToLayer("Companion"));

        difficulties = new DifficultRatio[]
        {
            new DifficultRatio(){ hp = 1, dmg = 1, reward = 1 },
            new DifficultRatio(){ hp = 1.5f, dmg = 1.5f, reward = 1.2f },
            new DifficultRatio(){ hp = 2, dmg = 2, reward = 1.5f },
        };
    }

    public bool Pause { get { return pause || levelUpPause; } }
    private bool pause;
    private bool levelUpPause;

    public bool GameStarted { get { return gameStarted; } }
    private bool gameStarted;

    public DifficultRatio Difficulty { get { return difficulties[difficultyIndex]; } }
    private static DifficultRatio[] difficulties;
    public string DifficultyKey
    {
        get
        {
            string key = "TITLE.DIFFICULT.";
            switch (difficultyIndex)
            {
                case 0: key += "EASY"; break;
                case 1: key += "NORMAL"; break;
                case 2: key += "HARD"; break;
            }

            return key;
        }
    }
    private int difficultyIndex;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UIController.Instance.UIisRemains)
            {
                UIController.Instance.OffOpenedUI();
            }
            else
            {
                PauseGame(!pause);
            }
        }

        if (RoundController.Instance != null && !RoundController.Instance.Progress)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                UIController.Instance.OnOffShop();
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                UIController.Instance.OnOffBuildMode();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartRound();
            }
        }
    }

    public void OnOffPause()
    {
        PauseGame(!pause);
    }

    public void PauseGame(bool b)
    {
        pause = b;
        UIController.Instance.OpenSetting(pause);
    }

    public void LevelUpPause(bool b)
    {
        levelUpPause = b;
    }

    public void GoTo(SceneController.Scene scene)
    {
        UIController.Instance.Title(scene == SceneController.Scene.TITLE);
        SceneController.ChangeScene(scene);
        StartCoroutine(ControlGame(scene));
    }

    public void SelectDifficulty(int index)
    {
        difficultyIndex = index;
    }

    IEnumerator ControlGame(SceneController.Scene scene)
    {
        while (!SceneController.isLoad) yield return null;

        switch (scene)
        {
            case SceneController.Scene.TITLE:
                CursorController.Instance.SetCursor(false);
                PauseGame(false);
                gameStarted = false;
                UIController.Instance.ChangeScene(1);
                break;

            case SceneController.Scene.GAME:
                CursorController.Instance.SetCursor(true);
                PauseGame(false);
                UIController.Instance.ChangeScene(2);
                MapGenerator.Instance.StartGame();
                RoundController.Instance.EndGame();
                WeaponController.Instance.StartGame();
                BuildingController.Instance.StartGame();
                UIController.Instance.StartGame();

                Player.Instance.StartGame();
                CameraController.Instance.SetCamera(Camera.main);
                gameStarted = true;
                break;
        }
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

    public class DifficultRatio
    {
        public float hp;
        public float dmg;
        public float reward;
    }
}
