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
        spawnedEnemies = new List<EnemyObject>();
    }

    private List<EnemyObject> spawnedEnemies;

    public void DeadEnemy(EnemyObject enemy)
    {
        spawnedEnemies.Remove(enemy);
    }

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
                EnemyObject enemyObject = (EnemyObject)PoolController.Pop("Invisible Enemy");
                enemyObject.SetData(enemy);
                enemyObject.transform.position = MapGenerator.Instance.GetEnemySpawnPos();
                spawnedEnemies.Add(enemyObject);
            }
            yield return null;
        }
    }
}
