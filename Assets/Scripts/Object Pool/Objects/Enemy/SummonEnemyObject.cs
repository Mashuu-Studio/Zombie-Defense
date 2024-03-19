using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonEnemyObject : EnemyObject, ISummonObject
{
    public List<GameObject> SummonedUnits { get { return summonedUnits; } }

    public bool CanSummon { get { return !waitSummonCD && SummonedUnits.Count < summonAmount; } }

    private List<GameObject> summonedUnits;
    private string summonUnitName;
    private float summonCD;
    private int summonAmount;
    private bool waitSummonCD;

    public override void Update()
    {
        base.Update();
        int index = 0;
        while (index < SummonedUnits.Count)
        {
            if (!SummonedUnits[index].activeSelf) SummonedUnits.RemoveAt(index);
            else index++;
        }
    }
    public override void SetData(Enemy data, int remainSep = 0)
    {
        base.SetData(data);
        summonedUnits = new List<GameObject>();
        summonUnitName = data.summonUnit;
        summonCD = data.summonCD;
        summonAmount = data.summonAmount;
        waitSummonCD = false;
    }

    public void Summon()
    {
        waitSummonCD = true;
        Enemy enemy = EnemyManager.GetEnemy(summonUnitName);

        EnemyObject enemyObject = EnemyController.Instance.AddEnemy(enemy,
            transform.position + Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * Vector2.up);
        summonedUnits.Add(enemyObject.gameObject);

        StartCoroutine(SummonTimer());
    }

    public IEnumerator SummonTimer()
    {
        float time = 0;
        while (time < summonCD)
        {
            if (!GameController.Instance.Pause) time += Time.deltaTime;
            yield return null;
        }
        waitSummonCD = false;
    }
}
