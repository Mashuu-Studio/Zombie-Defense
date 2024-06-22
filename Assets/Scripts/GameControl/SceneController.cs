using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get { return instance; } }
    private static SceneController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        isLoad = false;
    }

    public static string[] sceneName { get; private set; } = { "Loading", "Title", "Game" };

    public enum Scene { LOADING = 0, TITLE, GAME, }
    public static bool isLoad;

    public static void ChangeScene(Scene scene)
    {
        isLoad = false;
        Instance.StartCoroutine(LoadScene(scene));
    }

    private static IEnumerator LoadScene(Scene scene)
    {
        //UIController.Instance.ChangeScene((int)Scene.LOADING);
        var async = SceneManager.LoadSceneAsync(sceneName[(int)scene]);
        while (!async.isDone) yield return null;
        isLoad = true;
    }
}
