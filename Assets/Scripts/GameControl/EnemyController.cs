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

    public IEnumerator SpawnEnemy(Enemy enemy, float time)
    {
        float t = 0;
        while (true)
        {
            if (!GameController.Instance.Pause) t += Time.deltaTime;

            if (t >= time)
            {
                t -= time;
                if (MapGenerator.Instance.Map == null) yield return null;

                AddEnemy(enemy, MapGenerator.Instance.GetEnemySpawnPos());
            }
            yield return null;
        }
    }

    public EnemyObject AddEnemy(Enemy enemy, Vector3 pos)
    {
        EnemyObject enemyObject = (EnemyObject)PoolController.Pop(enemy.key);
        enemyObject.SetData(enemy);
        enemyObject.transform.position = pos;
        spawnedEnemies.Add(enemyObject);

        return enemyObject;
    }

    public void DeadEnemy(EnemyObject enemy)
    {
        spawnedEnemies.Remove(enemy);
    }
}
