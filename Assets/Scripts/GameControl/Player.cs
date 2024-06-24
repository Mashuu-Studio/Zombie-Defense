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
    public int MaxDef { get { return maxdef; } }
    public int Hp { get { return hp; } }
    public int Def { get { return def; } }
    public float Speed { get { return speed * (1 + ActivatedBuff.speed); } }
    public int ReloadTime { get { return reload; } }
    public int Reward { get { return reward; } }

    public int Money { get { return money; } }

    private Rigidbody2D rigidbody;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    private Dictionary<string, int> itemAmount = new Dictionary<string, int>();
    private Dictionary<string, int> magazines = new Dictionary<string, int>();

    public int KillCount { get { return killCount; } }
    private int killCount;
    #region Init & Update

    public void StartGame()
    {
        killCount = 0;

        invincible = false;

        hp = maxhp = 100;
        def = maxdef = 100;
        speed = 5;
        reload = 0;
        reward = 0;
        money = 500;

        itemAmount.Clear();
        magazines.Clear();
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
        if (GameController.Instance.GameProgress == false
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
        if (BuildingController.Instance.BuildMode) return;
        CameraController.Instance.MoveCamera(rigidbody.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (GameController.Instance.GameProgress == false
            || GameController.Instance.Pause) return;

        rigidbody.position += new Vector2(axisX, axisY) * Time.fixedDeltaTime * Speed;
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
    public bool BuyItem(BuyableData data, bool isMagazine = false)
    {
        int price = isMagazine ? data.magprice : data.price;
        if (Buy(price)) return true;
        return false;
    }

    public bool Buy(int price)
    {
        if (Money >= price)
        {
            money -= price;
            return true;
        }
        return false;
    }

    public bool BuyMagazine(Weapon data)
    {
        if (itemAmount.ContainsKey(data.key) && Buy(data.magprice)) return true;
        return false;
    }

    public void AdjustItemAmount(string key, int amount)
    {
        if (itemAmount.ContainsKey(key))
        {
            var w = WeaponManager.GetWeapon(key);
            if (w != null && w.infAmount) return; // 무기인데 무한무기면 스킵.
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
            if (w.singleBulletReload) refillAmmo = 1;
            else
            {
                refillAmmo = w.ammo - curammo;
                if (w.infAmount == false && refillAmmo > magazines[key]) refillAmmo = magazines[key];
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
        killCount++;
        float percentage = (100 + reward) / 100f;
        money += (int)(m * percentage * GameController.Instance.Difficulty.reward);
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
        if (buff.IsHeal)
        {
            if (buff.hp > 0) Heal(buff.hp);
            if (buff.def > 0) RefillArmor(buff.def);
        }
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
        int count = 1;
        while (time < buff.time)
        {
            if (!GameController.Instance.Pause) time += Time.deltaTime;
            if (buff.IsDotDamage && time >= count * buff.delay)
            {
                count++;
                Damaged(buff.hp);
            }
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

    public void RefillArmor(int amount)
    {
        def += amount;
        if (def > maxdef) def = maxdef;
    }

    public void Damaged(int dmg, ObjectData.Attribute attribute = ObjectData.Attribute.NONE)
    {
        if (invincible) return;

        dmg = IDamagedObject.Armoring(dmg, ref def);
        hp -= dmg;
        SoundController.Instance.PlaySFX(transform, "CHARACTER.DAMAGED");

        if (changeColorCoroutine != null) StopCoroutine(changeColorCoroutine);
        changeColorCoroutine = ChangeColor(Color.red);
        StartCoroutine(changeColorCoroutine);

        // 게임오버
        if (hp <= 0)
        {
            hp = 0;
            GameController.Instance.Lose();
        }
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
