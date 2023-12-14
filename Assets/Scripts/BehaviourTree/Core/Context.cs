using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Context
{
    public GameObject gameObject;
    public Transform transform;
    public IAttackObject attackObject;
    public IDamagedObject damagedObject;
    public IMovingObject movingObject;

    public static Context CreateContextFromObject(GameObject go)
    {
        Context context = new Context();
        context.gameObject = go;
        context.transform = go.transform;
        context.attackObject = go.GetComponent<IAttackObject>();
        context.damagedObject = go.GetComponent<IDamagedObject>();
        context.movingObject = go.GetComponent<IMovingObject>();

        return context;
    }
}
