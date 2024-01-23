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

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Map Boundary"));
    }

    public bool Pause { get { return pause || levelUpPause; } }
    private bool pause;
    private bool levelUpPause;

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
