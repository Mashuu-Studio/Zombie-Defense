using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagedObject
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

    public enum StatType { HP = 0, SPEED, RELOAD, REWARD, }

    private int lv;
    private int exp;
    private int maxexp;
    private int bonusStat;

    private bool invincible;

    private int hp;
    private int maxhp;
    private int speed;
    private int reload;
    private int reward;

    private int money;

    public int Lv { get { return lv; } }
    public int MaxExp { get { return maxexp; } }
    public int Exp { get { return exp; } }
    public int BonusStat { get { return bonusStat; } }

    public int MaxHp { get { return maxhp; } }
    public int Hp { get { return hp; } }
    public int Def { get; }
    public int Speed { get { return speed; } }
    public int ReloadTime { get { return reload; } }
    public int Reward { get { return reward; } }

    public int Money { get { return money; } }

    private Rigidbody2D rigidbody;

    [SerializeField] private BoxCollider2D autoTargetCollider;
    [SerializeField] private SpriteRenderer gunSpriteRenderer;

    private Dictionary<string, int> itemAmount = new Dictionary<string, int>();
    private Dictionary<string, int> magazines = new Dictionary<string, int>();
    #region Init & Update

    public void StartGame()
    {
        lv = 1;
        exp = 0;
        maxexp = 10;
        bonusStat = 0;

        invincible = false;

        hp = maxhp = 10;
        speed = 5;
        reload = 0;
        reward = 0;
        money = 0;

        itemAmount.Clear();
        foreach (var weapon in WeaponManager.Weapons)
        {
            itemAmount.Add(weapon.key, 0);
            magazines.Add(weapon.key, 0);
        }

        foreach (var turret in TurretManager.Turrets)
        {
            itemAmount.Add(turret.key, 0);
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

    private void Update()
    {
        if (GameController.Instance.GameStarted == false
            || GameController.Instance.Pause
            || TurretController.Instance.BuildMode)
        {
            axisX = 0;
            axisY = 0;
            return;
        }

        axisX = Input.GetAxis("Horizontal");
        axisY = Input.GetAxis("Vertical");

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
            || TurretController.Instance.BuildMode) return;

        rigidbody.position += new Vector2(axisX, axisY) * Time.fixedDeltaTime * speed;
        CameraController.Instance.MoveCamera(rigidbody.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void LookAt(Vector3 target)
    {
        Vector2 dir = target - transform.position;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }
    #endregion

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
        if (itemAmount.ContainsKey(key))
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
            if (WeaponManager.GetWeapon(key).infAmount) mag = -1;
            else mag = magazines[key];
        }
        return mag;
    }

    public void AddMagazine(string key)
    {
        if (magazines.ContainsKey(key)) magazines[key]++;
    }

    public bool HasMagazine(string key)
    {
        if (WeaponManager.GetWeapon(key).infAmount) return true;
        if (magazines.ContainsKey(key) && magazines[key] > 0)
        { 
            magazines[key]--;
            return true;
        }
        return false;
    }

    public void ReturnMagazine(string key)
    {
        if (magazines.ContainsKey(key))
            magazines[key]++;
    }

    #region Reward
    public void GetReward(int xp, int m)
    {
        float percentage = (100 + reward) / 100f;
        exp += (int)(xp * percentage);
        money += (int)(m * percentage);

        if (exp >= maxexp) StartCoroutine(LevelUp());
    }

    IEnumerator LevelUp()
    {
        // 한 번에 여러번 레벨업 할 수 있으므로 반복문 활용
        while (exp >= maxexp)
        {
            if (!GameController.Instance.Pause)
            {
                lv += 1;
                exp -= maxexp;
                maxexp += 10;
                bonusStat++;
                //UIController.Instance.LevelUp();
            }
            yield return null;
        }
    }

    public void Upgrade(StatType type)
    {
        if (bonusStat <= 0) return;

        bonusStat--;
        switch (type)
        {
            case StatType.HP:
                hp += 5;
                maxhp += 5;
                break;
            case StatType.SPEED:
                speed += 1;
                break;
            case StatType.RELOAD:
                reload += 25;
                break;
            case StatType.REWARD:
                reward += 25;
                break;
        }
    }
    #endregion

    public void Buff(Item.BuffType type, int value = 0)
    {
        if (type == Item.BuffType.HP) hp = maxhp;
        else if (type == Item.BuffType.MONEY) money += value;
        else StartCoroutine(ActiveBuff(type));
    }

    IEnumerator ActiveBuff(Item.BuffType type)
    {
        float time = 3;
        switch (type)
        {
            case Item.BuffType.INVINCIBLE: invincible = true; time = 3; break;
            case Item.BuffType.RELOAD: reload += 100; time = 10; break;
            case Item.BuffType.SPEED: speed += 5; time = 10; break;
        }

        while (time > 0)
        {
            if (!GameController.Instance.Pause) time -= Time.deltaTime;
            yield return null;
        }

        switch (type)
        {
            case Item.BuffType.INVINCIBLE: invincible = false; break;
            case Item.BuffType.RELOAD: reload -= 100; break;
            case Item.BuffType.SPEED: speed -= 5; break;
        }
    }

    public void Damaged(int dmg)
    {
        if (invincible) return;
        hp -= dmg;

        if (hp <= 0) GameController.Instance.GoTo(SceneController.Scene.TITLE);
    }

    #region Weapon
    public void SwitchWeapon(string name)
    {
        gunSpriteRenderer.sprite = SpriteManager.GetSprite(name);
    }

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
    #endregion
}
