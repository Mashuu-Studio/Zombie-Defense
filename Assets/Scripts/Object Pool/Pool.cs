using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pool : MonoBehaviour
{
    public Poolable Prefab { get { return prefab; } }
    private Poolable prefab;
    private Stack<Poolable> pool;

    public void Init(Poolable prefab)
    {
        this.prefab = prefab;
        pool = new Stack<Poolable>();

        prefab.gameObject.SetActive(false);
        for (int i = 0; i < prefab.amount; i++)
        {
            CreateObject();
        }
    }

    public void CreateObject()
    {
        Poolable obj = Instantiate(prefab, transform);
        obj.name = prefab.name;
        obj.Init();
        pool.Push(obj);
        obj.gameObject.SetActive(false);
    }

    public Poolable Pop()
    {
        if (pool.Count == 0) CreateObject();
        Poolable obj = pool.Pop();
        obj.gameObject.SetActive(true);
        return obj;
    }

    public void Push(Poolable obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(transform);
        pool.Push(obj);
    }
}
