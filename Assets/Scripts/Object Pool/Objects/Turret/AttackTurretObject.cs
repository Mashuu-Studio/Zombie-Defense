using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

[AddComponentMenu("Poolable/Attacking Turret (Poolable)")]
public class AttackTurretObject : TurretObject, IAttackObject
{
    [SerializeField] private LocalizeStringEvent mountedWeapon;
    private Collider2D targetCollider;
    private bool reloading;
    private IEnumerator reloadCoroutine;

    private Weapon weapon;

    public Collider2D TargetCollider { get { return targetCollider; } }
    public int Dmg { get { return weapon != null ? weapon.dmg : 0; } }
    public float Range { get { return weapon != null ? weapon.range : 0; } }
    public float ADelay { get { return weapon != null ? weapon.adelay : 0; } }
    public bool WaitAttack { get { return weapon != null ? weapon.Wait : false; } }

    public override void SetData(Turret data, Vector2 pos)
    {
        base.SetData(data, pos);
        weapon = null;
        mountedWeapon.SetEntry("WEAPON.NONE");
    }

    public void Mount(Weapon w, bool b)
    {
        // 마운트 하는 경우
        if (b)
        {
            if (w.infAmount == false && Player.Instance.ItemAmount(w.key) <= 0) return;

            // 이미 무기가 들어있을 경우
            if (weapon != null)
            {
                // 기존에 있던 무기는 소지품으로
                Player.Instance.AdjustItemAmount(weapon.key, 1);
            }
            weapon = new Weapon(w);
            mountedWeapon.SetEntry(w.key);
            Player.Instance.AdjustItemAmount(w.key, -1);
        }
        // 언마운트 하는 경우
        else
        {
            // 기존에 있던 무기는 소지품으로
            Player.Instance.AdjustItemAmount(weapon.key, 1);
            weapon = null;
            mountedWeapon.SetEntry("WEAPON.NONE");
        }
    }

    Collider2D[] targets;
    Transform target;
    public bool DetectTarget()
    {
        if (weapon == null || reloading) return false;

        targets = Physics2D.OverlapCircleAll(transform.position, Range / 2, 1 << LayerMask.NameToLayer("Enemy"));
        return targets.Length > 0;
    }

    public void Attack()
    {
        // 우선순위에 따라 적을 선택하는 코드가 들어갈 예정
        target = targets[0].transform;

        if (!WaitAttack)
        {
            if (weapon.curammo <= 0)
            {
                Reload();
                return;
            }
            weapon.Fire(transform.position, target.position, transform.rotation.eulerAngles.z);
            StartCoroutine(weapon.AttackDelay());
        }
    }

    public void Reload()
    {
        if (reloading || !Player.Instance.HasMagazine(weapon.key)) return;
        reloadCoroutine = weapon.Reloading();
        StartCoroutine(reloadCoroutine);
    }

    public override void DestroyTurret()
    {
        base.DestroyTurret();
        StopAllCoroutines();

        // 무기가 있었다면 언마운트
        if (weapon != null) Mount(weapon, false);
        weapon = null;
        reloading = false;
    }
}
