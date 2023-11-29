using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy(1.5f));
    }

    IEnumerator SpawnEnemy(float time)
    {
        while (true)
        {
            if (MapGenerator.Instance.Map == null) yield return null;
            Poolable enemy = PoolController.Pop("Enemy");
            enemy.transform.position = MapGenerator.Instance.GetEnemySpawnPos();

            yield return new WaitForSeconds(time);
        }
    }
}
