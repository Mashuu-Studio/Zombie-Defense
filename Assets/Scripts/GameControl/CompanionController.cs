using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionController : MonoBehaviour
{
    public static CompanionController Instance { get { return instance; } }
    private static CompanionController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public bool Hirable { get { return companions.Count < MAX_COMPANION; } }

    public static int MAX_COMPANION { get; private set; } = 4;
    public List<CompanionObject> Companions { get { return companions; } }
    private List<CompanionObject> companions = new List<CompanionObject>();

    public CompanionObject AddCompanion(string key)
    {
        CompanionObject companion = (CompanionObject)PoolController.Pop(key);
        companion.transform.position = Player.Instance.transform.position;
        companion.Summon(key);
        companions.Add(companion);

        return companion;
    }

    public void RemoveCompanion(CompanionObject companion)
    {
        UIController.Instance.RemoveCompanion(companion);
        PoolController.Push(companion.name, companion);
        companions.Remove(companion);
    }

    public void SetCompanionPatrol(int index, List<Vector2> list)
    {
        companions[index].SetHoldPatrol(list);
    }
}
