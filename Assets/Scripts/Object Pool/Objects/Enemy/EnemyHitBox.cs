using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    private EnemyObject enemy;
    int layerMask;
    public void Init(EnemyObject enemy)
    {
        this.enemy = enemy;
        layerMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Building");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & layerMask) > 0)
        {
            enemy.Damaging(collision.gameObject);
        }
    }
}
