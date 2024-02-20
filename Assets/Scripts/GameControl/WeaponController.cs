using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance { get { return instance; } }
    private static WeaponController instance;

    private List<Weapon> weapons;
    private int curIndex;
    private bool wait;
    public Weapon CurWeapon { get { return weapons[curIndex]; } }

    private int WeaponAmount
    {
        get
        {
            int amount = 0;
            foreach (var weapon in weapons)
            {
                if (weapon.usable) amount++;
            }
            return amount;
        }
    }

    public string[] PrevCurNextWeaponKeys
    {
        get
        {
            string[] str = new string[3] { "", "", "" };

            // 0부터 2까지 순서대로 prev, cur, next
            str[1] = weapons[curIndex].key;
            // 무기가 하나라면 prev와 next는 비움.
            if (WeaponAmount > 1)
            {
                // prev부터 채워줌.
                // 무기가 둘이라면 prev와 next가 같으나 next를 비우기 위함.
                int index = curIndex;
                do
                {
                    index--;
                    if (index < 0) index = weapons.Count - 1;
                } while (!weapons[index].usable);
                str[0] = weapons[index].key;

                if (WeaponAmount > 2)
                {
                    index = curIndex;
                    do
                    {
                        index++;
                        if (index >= weapons.Count) index = 0;
                    } while (!weapons[index].usable);
                    str[2] = weapons[index].key;
                }
            }
            return str;
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void StartGame()
    {
        weapons = WeaponManager.GetWeapons();
        weapons.ForEach(weapon => weapon.usable = false);
        weapons[0].usable = true;
        curIndex = 0;

        weapons.ForEach(w => w.Reload());
        wait = false;
        Player.Instance.SwitchWeapon(CurWeapon.key);
        UIController.Instance.SwitchWeapon();
    }


    public void AddWeapon(string key)
    {
        Weapon weapon = FindWeapon(key);
        if (weapon == null || weapon.consumable || HasWeapon(key)) return;

        weapon.Reload();
        weapon.usable = true;
        UIController.Instance.UpdateWeaponImage();
    }

    public bool HasWeapon(string key)
    {
        Weapon weapon = FindWeapon(key);
        return (weapon != null) ? weapon.usable : false;
    }

    private Weapon FindWeapon(string key)
    {
        return weapons.Find(weapon => weapon.key == key);
    }

    IEnumerator adelayCoroutine;
    IEnumerator reloadCoroutine;

    private void Update()
    {
        if (GameController.Instance.GameStarted == false 
            || GameController.Instance.Pause
            || TurretController.Instance.BuildMode) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int move = (scroll > 0) ? 1 : -1;
        if (scroll == 0) move = 0;
        Switch(move);

        if (UIController.PointOverUI()) return;
        if (Input.GetMouseButton(0))
            Fire(Player.Instance.transform.position, mouseWorldPos);

        if (Input.GetKeyDown(KeyCode.R))
            Reload();

        if (Input.GetKeyDown(KeyCode.G))
            Granade(Player.Instance.transform.position, mouseWorldPos);
    }

    IEnumerator AttackDelay()
    {
        wait = true;

        float time = CurWeapon.adelay;
        while (time > 0)
        {
            if (!GameController.Instance.Pause) time -= Time.deltaTime;
            yield return null;
        }
        wait = false;
    }

    public void Granade(Vector2 start, Vector2 dest)
    {
        if (Player.Instance.ItemAmount("WEAPON.GRANADE") <= 0) return;

        Weapon granade = WeaponManager.GetWeapon("WEAPON.GRANADE");

        Vector2 dir = (dest - start).normalized;
        ((Bullet)PoolController.Pop("Bullet")).SetBullet(start, dest, dir, granade, granade.bulletSpeed);
        Player.Instance.AdjustItemAmount("WEAPON.GRANADE", -1);
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

        float pReload = (100 + Player.Instance.Reload) / 100f;
        float time = CurWeapon.reload / pReload;
        SoundController.Instance.PlaySFX(Player.Instance.gameObject, "RELOAD");
        while (time > 0)
        {
            if (!GameController.Instance.Pause) time -= Time.deltaTime;
            yield return null;
        }
        CurWeapon.Reload();
        UIController.Instance.Reloading(false);
        wait = false;
    }

    public void Switch(int move)
    {
        do
        {
            if (move > 0) curIndex--;
            if (move < 0) curIndex++;

            if (curIndex < 0) curIndex = weapons.Count - 1;
            if (curIndex >= weapons.Count) curIndex = 0;
        } while (!CurWeapon.usable);

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
            Player.Instance.SwitchWeapon(CurWeapon.key);
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
            // 줌인 상태라면 흔들림 보정
            // 추후 샷건, 화염방사 등 흔들림 보정을 할 필요가 없는 값들에 대해 추가 조정
            int angle = Random.Range(-spread / 2, spread / 2 + 1) / (Player.Instance.ZoomIn ? 2 : 1);
            Vector3 newDir = Quaternion.Euler(0, 0, angle) * dir;
            // 오토타겟이면 적의 수만큼 자동타겟팅하여 공격.
            if (CurWeapon.autotarget)
            {
                if (autoTargets.Count > i) pos = autoTargets[i].transform.position;
                // 적의 수가 타겟팅 수보다 적다면 스킵
                else break;
            }
            ((Bullet)PoolController.Pop("Bullet")).SetBullet(pos, dest, newDir, CurWeapon, CurWeapon.bulletSpeed);
        }
        SoundController.Instance.PlaySFX(Player.Instance.gameObject, CurWeapon.key);
        CurWeapon.curammo--;
        UIController.Instance.UpdateAmmo(CurWeapon.curammo);
        adelayCoroutine = AttackDelay();
        StartCoroutine(adelayCoroutine);
    }
}
