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
        weapons = new List<Weapon>();
        weapons.Add(WeaponManager.GetWeapon("PISTOL"));
        curIndex = 0;

        weapons.ForEach(w => w.Reload());
        wait = false;
        Player.Instance.SwitchWeapon(CurWeapon.name);
        UIController.Instance.SwitchWeapon();
    }

    public void GetWeapon(Weapon weapon)
    {
        if (HasWeapon(weapon.name)) return;

        weapon.Reload();
        weapons.Add(weapon);
    }

    public bool HasWeapon(string name)
    {
        return weapons.Find(weapon => weapon.name == name) != null;
    }

    IEnumerator adelayCoroutine;
    IEnumerator reloadCoroutine;

    private void Update()
    {
        if (GameController.Instance.GameStarted == false || GameController.Instance.Pause) return;

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
            if (!GameController.Instance.Pause) time -= Time.deltaTime;
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
            Player.Instance.SwitchWeapon(CurWeapon.name);
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
