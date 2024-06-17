using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController Instance { get { return instance; } }
    private static EnemyController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public List<EnemyObject> SpawnEnemies { get { return spawnedEnemies; } }
    private List<EnemyObject> spawnedEnemies = new List<EnemyObject>();

    public void EndRound()
    {
        while (spawnedEnemies.Count > 0) spawnedEnemies[0].Dead();
    }

    public IEnumerator SpawnEnemy(Enemy enemy, float prob)
    {
        float t = 0;
        while (true)
        {
            // 매 초 확률을 체크하여 소환.
            if (!GameController.Instance.Pause) t += Time.deltaTime;

            if (t >= 1f)
            {
                t--;
                if (MapGenerator.Instance.Map == null) yield return null;

                // 확률이 1이상이라면 확정 소환.
                float p = prob;
                while (p >= 1)
                {
                    AddEnemy(enemy, MapGenerator.Instance.GetEnemySpawnPos());
                    p--;
                }

                // 이 후 남은 확률에 맞춰서 추가소환.
                float rand = Random.Range(0, 1f);
                if (rand <= p) AddEnemy(enemy, MapGenerator.Instance.GetEnemySpawnPos());
            }
            yield return null;
        }
    }

    public EnemyObject AddEnemy(Enemy enemy, Vector3 pos)
    {
        EnemyObject enemyObject = (EnemyObject)PoolController.Pop(enemy.key);
        enemyObject.SetData(enemy);
        enemyObject.transform.position = pos;
        enemyObject.transform.SetParent(transform);
        spawnedEnemies.Add(enemyObject);

        return enemyObject;
    }

    public void DeadEnemy(EnemyObject enemy)
    {
        spawnedEnemies.Remove(enemy);
    }
}
