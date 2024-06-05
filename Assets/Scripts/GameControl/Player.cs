using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagedObject, IBuffTargetObject
{
    #region Instance
    public static Player Instance { get { return instance; } }
    private static Player instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        rigidbody = GetComponent<Rigidbody2D>();
    }
    #endregion

    [SerializeField] private AudioListener lisnter;
    [SerializeField] private GameObject shootingPoint;
    public Vector3 FirePoint { get { return shootingPoint.transform.position; } }
    public enum StatType { HP = 0, SPEED, RELOAD, REWARD, }

    private bool invincible;

    private int hp;
    private int def;
    private int maxhp;
    private int maxdef;
    private int speed;
    private int reload;
    private int reward;

    private int money;

    public int MaxHp { get { return maxhp; } }
    public int Hp { get { return hp; } }
    public int Def { get { return def + ActivatedBuff.def; } }
    public float Speed { get { return speed * (1 + ActivatedBuff.speed); } }
    public int ReloadTime { get { return reload; } }
    public int Reward { get { return reward; } }

    public int Money { get { return money; } }

    private Rigidbody2D rigidbody;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private Dictionary<string, int> itemAmount = new Dictionary<string, int>();
    private Dictionary<string, int> magazines = new Dictionary<string, int>();
    #region Init & Update

    public void StartGame()
    {
        invincible = false;

        hp = maxhp = 100;
        def = maxdef = 0;
        speed = 5;
        reload = 0;
        reward = 0;
        money = 0;

        itemAmount.Clear();
        foreach (var weapon in WeaponManager.Weapons)
        {
            int amount = weapon.infAmount ? -1 : 0;
            itemAmount.Add(weapon.key, amount);
            magazines.Add(weapon.key, amount);
        }

        foreach (var building in BuildingManager.Buildings)
        {
            itemAmount.Add(building.key, 0);
        }
    }

    // 사격 시 흔들림 보정을 위한 값
    public bool ZoomIn { get { return zoomin; } }
    private bool zoomin;
    public void Zoom(bool b)
    {
        zoomin = b;
    }

    float axisX;
    float axisY;

    public const float MOVE_SOUND_TIME = 0.5f;
    private float moveSoundTime = MOVE_SOUND_TIME;

    private void Update()
    {
        moveSoundTime += Time.deltaTime;
        if (GameController.Instance.GameStarted == false
            || GameController.Instance.Pause
            || BuildingController.Instance.BuildMode)
        {
            axisX = 0;
            axisY = 0;
            return;
        }

        axisX = Input.GetAxis("Horizontal");
        axisY = Input.GetAxis("Vertical");

        if (axisX != 0 && axisY != 0 && moveSoundTime > MOVE_SOUND_TIME)
        {
            SoundController.Instance.PlaySFX(transform, "CHARACTER.MOVE");
            moveSoundTime = 0;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        LookAt(mouseWorldPos);

        bool zoomin = Input.GetMouseButton(1);
        CameraController.Instance.ZoomCamera(zoomin);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameController.Instance.GameStarted == false
            || GameController.Instance.Pause
            || BuildingController.Instance.BuildMode) return;

        rigidbody.position += new Vector2(axisX, axisY) * Time.fixedDeltaTime * Speed;
        CameraController.Instance.MoveCamera(rigidbody.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void LookAt(Vector3 target)
    {
        Vector2 dir = target - transform.position;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(0, 0, degree + 90);
        lisnter.transform.localEulerAngles = new Vector3(0, 0, -(degree + 90));
    }

    #endregion

    #region Item Management
    public bool BuyItem(BuyableData data)
    {
        // 우선 true로 세팅.
        if (itemAmount.ContainsKey(data.key))// Money >= data.price )
        {
            //money -= data.price;
            itemAmount[data.key]++;
            return true;
        }
        return false;
    }

    public void AdjustItemAmount(string key, int amount)
    {
        if (itemAmount.ContainsKey(key) && WeaponManager.GetWeapon(key).infAmount == false)
        {
            itemAmount[key] += amount;
            UIController.Instance.UpdateItemAmount(key, itemAmount[key]);
        }
    }

    public int ItemAmount(string key)
    {
        if (itemAmount.ContainsKey(key)) return itemAmount[key];
        return 0;
    }

    public int GetMagazine(string key)
    {
        int mag = 0;
        if (magazines.ContainsKey(key))
        {
            mag = magazines[key];
        }
        return mag;
    }

    public void AddMagazine(string key)
    {
        if (magazines.ContainsKey(key))
        {
            var w = WeaponManager.GetWeapon(key);
            magazines[key] += w.ammo;
        }
    }
    public int UseMagazine(string key, int curammo)
    {
        int refillAmmo = 0;
        var w = WeaponManager.GetWeapon(key);
        if (HasMagazine(key))
        {
            if (w.infAmount) refillAmmo = w.ammo;
            else if (w.singleBulletReload) refillAmmo = 1;
            else
            {
                refillAmmo = w.ammo - curammo;
                if (refillAmmo > magazines[key]) refillAmmo = magazines[key];
            }
        }
        if (w.infAmount == false) magazines[key] -= refillAmmo;
        return refillAmmo;
    }

    public bool HasMagazine(string key)
    {
        if (WeaponManager.GetWeapon(key).infAmount) return true;
        if (magazines.ContainsKey(key) && magazines[key] > 0) return true;
        return false;
    }
    #endregion

    #region Reward
    public void GetReward(int m)
    {
        float percentage = (100 + reward) / 100f;
        money += (int)(m * percentage);
    }
    #endregion

    #region Buff
    public List<BuffInfo> Buffs { get { return buffs.Keys.ToList(); } }
    private Dictionary<BuffInfo, IEnumerator> buffs = new Dictionary<BuffInfo, IEnumerator>();
    public BuffInfo ActivatedBuff
    {
        get
        {
            BuffInfo buff = new BuffInfo();
            if (Buffs != null) Buffs.ForEach(b => buff += b);
            return buff;
        }
    }

    public void ActivateBuff(BuffInfo buff)
    {
        // 단순 회복일 경우 즉시 발동
        if (buff.IsHeal) Heal(buff.hp);
        else
        {
            if (buffs.ContainsKey(buff))
            {
                StopCoroutine(buffs[buff]);
                buffs[buff] = BuffTimer(buff);
            }
            else buffs.Add(buff, BuffTimer(buff));

            StartCoroutine(buffs[buff]);
        }
    }

    public IEnumerator BuffTimer(BuffInfo buff)
    {
        float time = 0;
        while (time < buff.time)
        {
            if (!GameController.Instance.Pause) time += Time.deltaTime;
            yield return null;
        }
        buffs.Remove(buff);
    }
    #endregion

    IEnumerator changeColorCoroutine;
    public void Heal(int amount)
    {
        hp += amount;
        if (hp > maxhp) hp = maxhp;

        if (changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);
        changeColorCoroutine = ChangeColor(Color.green);
        StartCoroutine(changeColorCoroutine);
    }

    public void Damaged(int dmg, ObjectData.Attribute attribute = ObjectData.Attribute.NONE)
    {
        if (invincible) return;
        hp -= dmg;
        SoundController.Instance.PlaySFX(transform, "CHARACTER.DAMAGED");

        if (changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);
        changeColorCoroutine = ChangeColor(Color.red);
        StartCoroutine(changeColorCoroutine);

        if (hp <= 0) GameController.Instance.GoTo(SceneController.Scene.TITLE);
    }

    IEnumerator ChangeColor(Color color)
    {
        Color reverse = Color.white - color;
        float time = 0.2f;
        while (time > 0)
        {
            if (!GameController.Instance.Pause)
            {
                spriteRenderer.material.SetColor("_Color", color);
                time -= Time.deltaTime;
                color += reverse * Time.deltaTime * 5;
            }
            yield return null;
        }
        spriteRenderer.material.SetColor("_Color", Color.white);
    }
    #region Weapon
    public void SwitchWeapon(float weapon)
    {
        animator.SetFloat("weapon", weapon);
    }

    public void Fire()
    {
        animator.SetTrigger("fire");
    }
    #endregion
}
