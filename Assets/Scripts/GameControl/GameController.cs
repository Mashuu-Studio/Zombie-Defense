using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject player;

    Stack<Poolable> test = new Stack<Poolable>();
    Stack<Poolable> test2 = new Stack<Poolable>();
    void Update()
    {
        float axisX = Input.GetAxis("Horizontal");
        float axisY = Input.GetAxis("Vertical");

        player.transform.position += new Vector3(axisX, axisY) * Time.deltaTime * 15;

        if (Input.GetKeyDown("a"))
        {
            test.Push(PoolController.Pop("Enemy"));
        }
        if (Input.GetKeyDown("s"))
        {
            test2.Push(PoolController.Pop("Enemy 1"));
        }

        if (Input.GetKeyDown("z") && test.Count > 0)
        {
            PoolController.Push("Enemy", test.Pop());
        }
        if (Input.GetKeyDown("x") && test2.Count > 0)
        {
            PoolController.Push("Enemy 1", test2.Pop());
        }
    }
}
