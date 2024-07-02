using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyHitBox : MonoBehaviour
{
    private EnemyObject enemy;
    private BoxCollider2D hitbox;
    private int layerMask;
    private string attackSfxName;
    public void Init(EnemyObject enemy)
    {
        // 위치를 파악하는 용도로만 사용. 부하를 줄이기 위해 off로 둚.
        hitbox = GetComponent<BoxCollider2D>();
        hitbox.enabled = false;
        this.enemy = enemy;
        layerMask = 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Companion") | 1 << LayerMask.NameToLayer("Building");

        attackSfxName = enemy.Data.key + ".ATTACK";
        if (SoundController.Instance.ContainsSFX(attackSfxName) == false) attackSfxName = "ENEMY.ATTACK";
    }
    /*
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & layerMask) > 0)
        {
            enemy.Damaging(collision.gameObject);
        }
    }*/

    // 공격할 때 켜지게 되어있음.
    private void OnEnable()
    {
        SoundController.Instance.PlaySFX(enemy.transform, attackSfxName, true);
        // 켜지자마자 단발적으로 체크.
        var hits = Physics2D.OverlapBoxAll(transform.position + Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * hitbox.offset,
            hitbox.size, transform.rotation.eulerAngles.z, layerMask);
        foreach (var hit in hits)
        {
            enemy.Damaging(hit.gameObject);
        }
    }
}
