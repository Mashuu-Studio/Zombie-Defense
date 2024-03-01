using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionObject : BTPoolable, IDamagedObject, IAttackObject
{
    [SerializeField] private SpriteRenderer gunSpriteRenderer;

    private int maxhp;
    private int hp;
    private int def;

    private Weapon weapon;
    private bool reloading;

    public override void Init()
    {
        base.Init();
        hp = maxhp = 5;
        def = 0;
        reloading = false;
        WaitAttack = false;
        SetBasicWeapon();
    }

    public void SetBasicWeapon()
    {
        ChangeWeapon("WEAPON.PISTOL");
    }

    public void ChangeWeapon(string key)
    {
        Weapon w = WeaponManager.GetWeapon(key);
        weapon = new Weapon(w);
        gunSpriteRenderer.sprite = SpriteManager.GetSprite(w.key);
    }

    /*
    public List<Collider2D> DetectEnemyTargets(float range)
    {
        // 위치를 캐릭터 앞으로 세팅
        // 크기는 range로 세팅
        autoTargetCollider.offset = new Vector2(range / 2 + .5f, 0);
        autoTargetCollider.size = Vector2.one * range;

        List<Collider2D> cols = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = 1 << LayerMask.NameToLayer("Enemy");
        // 전방 사각형 모양
        autoTargetCollider.OverlapCollider(filter, cols);
        return cols;
    }
    */
    #region IDamagedObject
    public int Hp { get { return hp; } }
    public int MaxHp { get { return maxhp; } }
    public int Def { get { return def; } }

    public void Heal()
    {
        hp = maxhp;
    }

    public void Damaged(int dmg)
    {
        hp -= dmg;
        if (hp < 0)
        {
            StopAllCoroutines();
            CompanionController.Instance.RemoveCompanion(this);
        }
    }
    #endregion

    #region IAttackObject
    public int Dmg { get { return (weapon != null) ? weapon.dmg : 0; } }
    public float Range { get { return (weapon != null) ? weapon.range : 0; } }
    public float ADelay { get { return (weapon != null) ? weapon.adelay : 0; } }
    public bool WaitAttack { get; set; }

    private Collider2D targetCollider;
    public Collider2D TargetCollider { get { return targetCollider; } }
    Collider2D[] targets;
    Transform target;
    public bool DetectTarget()
    {
        if (weapon == null || reloading) return false;

        targets = Physics2D.OverlapCircleAll(transform.position, Range / 2, 1 << LayerMask.NameToLayer("Enemy"));
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
                ((Bullet)PoolController.Pop("Bullet")).SetBullet(transform.position, target.position, newDir, weapon, weapon.bulletSpeed);
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

    #endregion

    private IEnumerator reloadCoroutine;

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
}

