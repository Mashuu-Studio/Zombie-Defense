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
                adelay = 0.5f,
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
                adelay = 0.1f,
                range = 15,
                bulletspreadangle = 10,

                ammo = 30,
                reload = 1,
            },
            new Weapon()
            {
                name = "ASSULT RIFLE",
                price = 1000,

                dmg = 3,
                adelay = 0.2f,
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
        {
            Vector2 dir = mouseWorldPos - Player.Instance.transform.position;
            Fire(Player.Instance.transform.position, dir);
        }

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

    public void Fire(Vector3 pos, Vector3 dir)
    {
        if (wait) return;
        if (CurWeapon.curammo == 0)
        {
            Reload();
            return;
        }

        int spread = CurWeapon.bulletspreadangle;
        for (int i = 0; i < CurWeapon.bullets; i++)
        {
            int angle = Random.Range(-spread / 2, spread / 2 + 1);
            Vector3 newDir = Quaternion.Euler(0, 0, angle) * dir;
            ((Bullet)PoolController.Pop("Bullet")).SetBullet(pos, newDir, CurWeapon.dmg, CurWeapon.range, 50);
        }
        CurWeapon.curammo--;
        adelayCoroutine = AttackDelay();
        StartCoroutine(adelayCoroutine);
    }
}
