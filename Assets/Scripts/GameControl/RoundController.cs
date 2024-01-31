using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundController : MonoBehaviour
{
    public static RoundController Instance { get { return instance; } }
    private static RoundController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        spawnEnemyCoroutines = new List<IEnumerator>();
    }

    private int round;
    private List<IEnumerator> spawnEnemyCoroutines;

    public void EndGame()
    {
        round = 0;
        StopAllCoroutines();
        GameController.Instance.EndRound();
        spawnEnemyCoroutines.ForEach(coroutine => StopCoroutine(coroutine));
        spawnEnemyCoroutines.Clear();
    }

    public void StartRound()
    {
        var roundInfo = RoundManager.GetRound(round);
        round++;
        // 라운드에 따라 라운드 시간 부여
        if (roundInfo != null)
            StartCoroutine(ProgressRound(roundInfo, 60));
        else GameController.Instance.GoTo(SceneController.Scene.TITLE);
    }

    private IEnumerator ProgressRound(Round roundInfo, float time)
    {
        foreach (var info in roundInfo.enemyInfo)
        {
            spawnEnemyCoroutines.Add(EnemyController.Instance.SpawnEnemy(EnemyManager.GetEnemy(info.Key), info.Value));
        }
        spawnEnemyCoroutines.ForEach(coroutine => StartCoroutine(coroutine));

        while (time > 0)
        {
            if (!GameController.Instance.Pause)
            {
                time -= Time.deltaTime;
                UIController.Instance.UpdateRoundTime(Mathf.CeilToInt(time));
            }
            yield return null;
        }

        GameController.Instance.EndRound();
        spawnEnemyCoroutines.ForEach(coroutine => StopCoroutine(coroutine));
        spawnEnemyCoroutines.Clear();
    }
}
