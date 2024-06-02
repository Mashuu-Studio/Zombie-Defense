using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitBox : MonoBehaviour
{
    private EnemyObject enemy;
    private int layerMask;
    private string attackSfxName;
    public void Init(EnemyObject enemy)
    {
        this.enemy = enemy;
        layerMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Building");

        attackSfxName = enemy.Data.key + ".ATTACK";
        if (SoundController.Instance.ContainsSFX(attackSfxName) == false) attackSfxName = "ENEMY.ATTACK";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & layerMask) > 0)
        {
            enemy.Damaging(collision.gameObject);
        }
    }

    // 공격할 때 켜지게 되어있음.
    private void OnEnable()
    {
        SoundController.Instance.PlaySFX(enemy.transform.position, attackSfxName);
    }
}
