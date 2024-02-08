using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class ActionController : MonoBehaviour
{
    public static ActionController Instance { get { return instance; } }
    private static ActionController instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(Flush());
    }

    private static Queue<Tuple<GameObject, Action>> actions = new Queue<Tuple<GameObject, Action>>();

    public static void AddAction(GameObject go, Action action)
    {
        Tuple<GameObject, Action> tuple = new Tuple<GameObject, Action>(go, action);
        actions.Enqueue(tuple);
    }

    IEnumerator Flush()
    {
        while (true)
        {
            int count = actions.Count;
            while (count > 0)
            {
                var action = actions.Dequeue();
                if (action.Item1.activeSelf) action.Item2.Invoke();
                count--;
            }
            yield return null;
        }
    }
}
