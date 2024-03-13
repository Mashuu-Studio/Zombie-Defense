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
        // 후에는 몬스터 이름이 하나의 프리팹이 될 예정. 
        // 현재는 테스트 용도로 프리팹을 분류해줌.
        EnemyObject enemyObject = (EnemyObject)PoolController.Pop(GetEnemyPrefabName(enemy));
        enemyObject.SetData(enemy);
        enemyObject.transform.position = pos;
        spawnedEnemies.Add(enemyObject);

        return enemyObject;
    }

    public void DeadEnemy(EnemyObject enemy)
    {
        spawnedEnemies.Remove(enemy);
    }

    public static string GetEnemyPrefabName(Enemy enemy)
    {
        string prefabname = "Enemy";
        if (enemy.fly) prefabname = "Flight " + prefabname;
        if (enemy.inv) prefabname = "Invisible " + prefabname;
        if (enemy.buff != null) prefabname = "Buff Enemy";
        if (enemy.summonAmount != 0) prefabname = "Summon Enemy";

        return prefabname;
    }
}
