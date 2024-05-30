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
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Map Boundary"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Companion"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Companion"));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Companion"), LayerMask.NameToLayer("Companion"));

        difficulties = new DifficultRatio[]
        {
            new DifficultRatio(){hp = 1, dmg = 1, speed = 1 },
            new DifficultRatio(){hp = 1.5f, dmg = 1.5f, speed = 1.5f },
            new DifficultRatio(){hp = 2, dmg = 2, speed = 2 },
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

    public void GoTo(SceneController.Scene scene)
    {
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
                gameStarted = false;
                UIController.Instance.ChangeScene(1);
                break;

            case SceneController.Scene.GAME:
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
        public float speed;
    }
}
