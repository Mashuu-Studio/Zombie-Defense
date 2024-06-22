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

    public bool Progress { get { return progress; } }
    private bool progress;
    public bool Endless { get { return RoundManager.GetRound(round - 1) == null; } }
    public int Round { get { return round; } }
    private int round;
    private List<IEnumerator> spawnEnemyCoroutines;

    public void EndGame()
    {
        GameOver();
        GameController.Instance.EndRound();
    }

    public void GameOver()
    {
        StopAllCoroutines();
        spawnEnemyCoroutines.ForEach(coroutine => StopCoroutine(coroutine));
        spawnEnemyCoroutines.Clear();
    }

    public void StartGame()
    {
        round = 0;
        progress = false;
        EndGame();
    }

    public void StartRound()
    {
        var roundInfo = RoundManager.GetRound(round++);
        if (roundInfo != null)
            StartCoroutine(ProgressRound(roundInfo, 60));
        // 엔드리스 시작
        else
            StartCoroutine(ProgressRound(RoundManager.EndRound, 60, true));
    }
    private const float AmountRatio = 1.25f;
    private IEnumerator ProgressRound(Round roundInfo, float time, bool endless = false)
    {
        progress = true;
        foreach (var info in roundInfo.enemyInfo)
        {
            spawnEnemyCoroutines.Add(EnemyController.Instance.SpawnEnemy(EnemyManager.GetEnemy(info.Key), info.Value * AmountRatio / time));
        }
        spawnEnemyCoroutines.ForEach(coroutine => StartCoroutine(coroutine));

        if (endless) time = 0;
        while (time > 0 || endless)
        {
            if (!GameController.Instance.Pause)
            {
                if (endless) time += Time.deltaTime;
                else time -= Time.deltaTime;
                UIController.Instance.UpdateRoundTime(Mathf.CeilToInt(time));
            }
            yield return null;
        }

        GameController.Instance.EndRound();
        spawnEnemyCoroutines.ForEach(coroutine => StopCoroutine(coroutine));
        spawnEnemyCoroutines.Clear();

        // 모든 라운드를 진행했다면 승리
        if (RoundManager.GetRound(round) == null)
        {
            GameController.Instance.Win();
        }
        progress = false;
    }
}
