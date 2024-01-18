using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance { get { return instance; } }
    private static WeaponController instance;

    private List<Weapon> weapons;
    private int curIndex = 0;
    private bool wait;
    public Weapon CurWeapon { get { return weapons[curIndex]; } }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        weapons = new List<Weapon>()
        {
            new Weapon()
            {
                name = "PISTOL",
                price = 0,

                dmg = 1,
                adelay = 0.4f,
                range = 15,
                bulletspreadangle = 10,

                ammo = 7,
                reload = 1,
            },
            new Weapon()
            {
                name = "SMG",
                price = 300,

                dmg = 1,
                adelay = 0.05f,
                range = 15,
                bulletspreadangle = 10,

                ammo = 30,
                reload = 1,
            },
            new Weapon()
            {
                name = "ASSULT RIFLE",
                price = 1000,

                dmg = 2,
                adelay = 0.1f,
                range = 25,
                bulletspreadangle = 5,

                ammo = 30,
                reload = 2,
            },
            new Weapon()
            {
                name = "SHOTGUN",
                price = 1000,

                dmg = 3,
                adelay = 2f,
                range = 8,
                bulletspreadangle = 45,
                bullets = 10,

                ammo = 5,
                reload = 3,
            },
            new Weapon()
            {
                name = "SNIPER RIFLE",
                price = 1000,

                dmg = 5,
                adelay = 2f,
                range = 40,
                bulletspreadangle = 2,
                bullets = 1,

                ammo = 7,
                pierce = true,
                reload = 2,
            },
            new Weapon()
            {
                name = "BAZUKA",
                price = 1000,

                dmg = 5,
                adelay = 3f,
                range = 40,
                bulletspreadangle = 2,
                bullets = 1,

                ammo = 7,
                point = 1,
                splash = 10,
                reload = 2,
            },
            new Weapon()
            {
                name = "TESLA",
                price = 1000,

                dmg = 1,
                adelay = .5f,
                range = 5,
                bulletspreadangle = 2,
                bullets = 5,

                ammo = 30,
                reload = 2,
                autotarget = true,
            },
        };

        weapons.ForEach(w => w.Reload());
        wait = false;
        UIController.Instance.SwitchWeapon();
    }

    IEnumerator adelayCoroutine;
    IEnumerator reloadCoroutine;

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int move = (scroll > 0) ? 1 : -1;
        if (scroll == 0) move = 0;
        Switch(move);

        if (Input.GetMouseButton(0))
            Fire(Player.Instance.transform.position, mouseWorldPos);

        if (Input.GetKeyDown(KeyCode.R))
            Reload();
    }

    IEnumerator AttackDelay()
    {
        wait = true;

        float time = CurWeapon.adelay;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        wait = false;
    }

    public void Reload()
    {
        if (wait) return;
        if (reloadCoroutine != null) StopCoroutine(reloadCoroutine);
        reloadCoroutine = Reloading();
        StartCoroutine(reloadCoroutine);
    }

    IEnumerator Reloading()
    {
        wait = true;
        UIController.Instance.Reloading(true);

        float time = CurWeapon.reload;
        SoundController.Instance.PlaySFX(Player.Instance.gameObject, "RELOAD");
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }
        CurWeapon.Reload();
        UIController.Instance.Reloading(false);
        wait = false;
    }

    public void Switch(int move)
    {
        if (move > 0) curIndex--;
        if (move < 0) curIndex++;

        if (curIndex < 0) curIndex = weapons.Count - 1;
        if (curIndex >= weapons.Count) curIndex = 0;

        if (move != 0)
        {
            if (adelayCoroutine != null)
            {
                StopCoroutine(adelayCoroutine);
                adelayCoroutine = null;
            }
            if (reloadCoroutine != null)
            {
                StopCoroutine(reloadCoroutine);
                reloadCoroutine = null;
            }
            wait = false;
            UIController.Instance.SwitchWeapon();
        }
    }

    public void Fire(Vector3 pos, Vector3 dest)
    {
        if (wait) return;
        if (CurWeapon.curammo == 0)
        {
            Reload();
            return;
        }

        List<Collider2D> autoTargets = CurWeapon.autotarget ? Player.Instance.DetectEnemyTargets(CurWeapon.range) : new List<Collider2D>();
        Vector3 dir = dest - pos;
        int spread = CurWeapon.bulletspreadangle;
        for (int i = 0; i < CurWeapon.bullets; i++)
        {
            int angle = Random.Range(-spread / 2, spread / 2 + 1);
            Vector3 newDir = Quaternion.Euler(0, 0, angle) * dir;
            // 지점 공격이면 위치 지정
            if (CurWeapon.point != 0) pos = dest;
            // 오토타겟이면 적의 수만큼 자동타겟팅하여 공격.
            if (CurWeapon.autotarget)
            {
                if (autoTargets.Count > i) pos = autoTargets[i].transform.position;
                // 적의 수가 타겟팅 수보다 적다면 스킵
                else break;
            }
            ((Bullet)PoolController.Pop("Bullet")).SetBullet(pos, newDir, CurWeapon, 50);
        }
        SoundController.Instance.PlaySFX(Player.Instance.gameObject, CurWeapon.name);
        CurWeapon.curammo--;
        adelayCoroutine = AttackDelay();
        StartCoroutine(adelayCoroutine);
    }
}
