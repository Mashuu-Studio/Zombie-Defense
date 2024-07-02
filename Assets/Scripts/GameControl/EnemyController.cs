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
            // �� �� Ȯ���� üũ�Ͽ� ��ȯ.
            if (!GameController.Instance.Pause) t += Time.deltaTime;
            yield return null;

            if (t >= 1f)
            {
                t--;
                if (MapGenerator.Instance.Map == null) yield return null;

                // Ȯ���� 1�̻��̶�� Ȯ�� ��ȯ.
                float p = prob;
                while (p >= 1)
                {
                    AddEnemy(enemy, MapGenerator.Instance.GetEnemySpawnPos());
                    p--;
                }

                // �� �� ���� Ȯ���� ���缭 �߰���ȯ.
                float rand = Random.Range(0, 1f);
                if (rand <= p) AddEnemy(enemy, MapGenerator.Instance.GetEnemySpawnPos());
            }
        }
    }

    public EnemyObject AddEnemy(Enemy enemy, Vector3 pos)
    {
        EnemyObject enemyObject = (EnemyObject)PoolController.Pop(enemy.key);
        enemyObject.SetData(enemy);
        enemyObject.transform.position = pos;
        //enemyObject.transform.SetParent(transform);
        spawnedEnemies.Add(enemyObject);

        return enemyObject;
    }

    public void DeadEnemy(EnemyObject enemy)
    {
        spawnedEnemies.Remove(enemy);
    }
}
