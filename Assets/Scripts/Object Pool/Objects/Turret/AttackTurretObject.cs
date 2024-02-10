using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

[AddComponentMenu("Poolable/Attacking Turret (Poolable)")]
public class AttackTurretObject : TurretObject, IAttackObject
{
    [SerializeField] private LocalizeStringEvent mountedWeapon;
    private Collider2D targetCollider;
    private int dmg;
    private float range;
    private float aDelay;
    private int speed;
    private bool reloading;
    private IEnumerator reloadCoroutine;

    private Weapon weapon;

    public Collider2D TargetCollider { get { return targetCollider; } }
    public int Dmg { get { return dmg; } }
    public float Range { get { return range; } }
    public float ADelay { get { return aDelay; } }
    public bool WaitAttack { get; set; }

    public override void SetData(Turret data, Vector2 pos)
    {
        base.SetData(data, pos);
        dmg = data.dmg;
        range = data.range;
        aDelay = data.adelay;
        speed = data.speed;
        weapon = null;
        mountedWeapon.SetEntry("WEAPON.NONE");
    }

    public void Mount(Weapon w, bool b)
    {
        // 마운트 하는 경우
        if (b)
        {
            if (Player.Instance.ItemAmount(w.key) <= 0) return;

            // 터렛에 들어있는 무기와 마운트하려는 무기가 다를 경우
            if (weapon != null && weapon.key != w.key)
            {
                // 기존에 있던 무기는 소지품으로
                Player.Instance.AdjustItemAmount(weapon.key, 1);
            }
            weapon = new Weapon(w);
            dmg = weapon.dmg;
            range = weapon.range;
            aDelay = weapon.adelay;
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

        targets = Physics2D.OverlapCircleAll(transform.position, range / 2, 1 << LayerMask.NameToLayer("Enemy"));
        return targets != null && targets.Length > 0;
    }

    public void Attack()
    {
        // 우선순위에 따라 적을 선택하는 코드가 들어갈 예정
        target = targets[0].transform;

        if (!WaitAttack && !reloading)
        {
            if (weapon.curammo <= 0)
            {
                Reload();
                return;
            }
            Vector2 dir = target.position - transform.position;
            int spread = weapon.bulletspreadangle;
            for (int i = 0; i < weapon.bullets; i++)
            {
                int angle = Random.Range(-spread / 2, spread / 2 + 1);
                Vector3 newDir = Quaternion.Euler(0, 0, angle) * dir;
                ((Bullet)PoolController.Pop("Bullet")).SetBullet(transform.position, newDir, weapon, 50);
            }
            SoundController.Instance.PlaySFX(Player.Instance.gameObject, weapon.key);
            weapon.curammo--;

            StartCoroutine(AttackTimer());
        }
    }

    public IEnumerator AttackTimer()
    {
        WaitAttack = true;
        float time = 0;
        while (time < ADelay)
        {
            if (!GameController.Instance.Pause) time += Time.deltaTime;
            yield return null;
        }
        WaitAttack = false;
    }

    public void Reload()
    {
        if (reloading) return;
        reloadCoroutine = Reloading();
        StartCoroutine(reloadCoroutine);
    }

    IEnumerator Reloading()
    {
        reloading = true;
        UIController.Instance.Reloading(true);

        float time = weapon.reload;
        while (time > 0)
        {
            if (!GameController.Instance.Pause) time -= Time.deltaTime;
            yield return null;
        }
        weapon.Reload();
        reloading = false;
    }

    public override void DestroyTurret()
    {
        base.DestroyTurret();
        StopAllCoroutines();

        // 무기가 있었다면 언마운트
        if (weapon != null) Mount(weapon, false);
        weapon = null;
        WaitAttack = false;
        reloading = false;
    }
}
