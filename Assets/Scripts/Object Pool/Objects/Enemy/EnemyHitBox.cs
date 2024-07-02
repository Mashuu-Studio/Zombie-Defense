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
        // ��ġ�� �ľ��ϴ� �뵵�θ� ���. ���ϸ� ���̱� ���� off�� �R.
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

    // ������ �� ������ �Ǿ�����.
    private void OnEnable()
    {
        SoundController.Instance.PlaySFX(enemy.transform, attackSfxName, true);
        // �����ڸ��� �ܹ������� üũ.
        var hits = Physics2D.OverlapBoxAll(transform.position + Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z) * hitbox.offset,
            hitbox.size, transform.rotation.eulerAngles.z, layerMask);
        foreach (var hit in hits)
        {
            enemy.Damaging(hit.gameObject);
        }
    }
}
