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

        Application.targetFrameRate = 60;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Map Boundary"));
    }

    public bool Pause { get { return pause; } }
    private bool pause;

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
}
