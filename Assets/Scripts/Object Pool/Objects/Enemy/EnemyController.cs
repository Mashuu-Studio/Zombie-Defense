using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    List<Enemy> enemies;
    private void Awake()
    {
        enemies = new List<Enemy>()
        {
            new Enemy()
            {
                hp = 3,
                speed = 5,
                dmg = 1,
                range = 3f,
                adelay = 1,
            },
            new Enemy()
            {
                hp = 5,
                speed = 2,
                dmg = 3,
                range = 0.8f,
                adelay = 3,
            },
            new Enemy()
            {
                hp = 1,
                speed = 4,
                dmg = 2,
                range = 1.2f,
                adelay = .5f,
            },
        };
    }
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
            EnemyObject enemy = (EnemyObject)PoolController.Pop("Enemy");
            int rand = Random.Range(0, enemies.Count);
            enemy.Init(enemies[rand]);
            enemy.transform.position = MapGenerator.Instance.GetEnemySpawnPos();

            yield return new WaitForSeconds(time);
        }
    }
}
