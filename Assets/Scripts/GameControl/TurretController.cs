using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public static TurretController Instance { get { return instance; } }
    private static TurretController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    [SerializeField] private TurretBuildPointer turretPointer;

    private Dictionary<Vector2, TurretObject> turrets = new Dictionary<Vector2, TurretObject>();

    public void StartGame()
    {
        foreach (var turret in turrets.Values)
        {
            turret.DestroyTurret();
        }
        turrets.Clear();
    }

    #region BuildMode
    public bool BuildMode { get { return buildMode; } }
    private bool buildMode;
    private string selectedTurretKey;

    public void SelectBuildingTurret(string key)
    {
        selectedTurretKey = key;
        turretPointer.ChangeSprite(SpriteManager.GetSprite(key));
    }

    public void ChangeBulidMode(bool b)
    {
        buildMode = b;
        turretPointer.gameObject.SetActive(buildMode);
    }

    public void MoveTurretPointer(Vector2 pos)
    {
        turretPointer.transform.position = pos;
        turretPointer.SetColor(Buildable(pos));
    }

    public Vector2 selectedTurretPos;
    public bool SelectTurret(Vector2 pos)
    {
        selectedTurretPos = pos;
        return turrets.ContainsKey(pos);
    }

    public void Mount(string key)
    {
        var turret = turrets[selectedTurretPos] as AttackTurretObject;
        if (turret) turret.Mount(WeaponManager.GetWeapon(key), true);
    }
    #endregion

    public void BuildTurret(Vector2 pos)
    {
        Turret data = TurretManager.GetTurret(selectedTurretKey);
        if (data == null || !Buildable(pos)) return;
        if (Player.Instance.ItemAmount(data.key) <= 0
            && !Player.Instance.BuyItem(data)) return;

        Poolable obj = PoolController.Pop(selectedTurretKey);
        TurretObject turret = obj.GetComponent<TurretObject>();
        if (turret == null)
        {
            PoolController.Push(selectedTurretKey, obj);
            return;
        }
        Player.Instance.AdjustItemAmount(data.key, -1);

        turret.SetData(data, pos);
        turret.gameObject.name = selectedTurretKey;
        turret.transform.parent = transform;
        turret.transform.position = pos;
        turrets.Add(pos, turret);
    }

    public void StoreTurret(Vector2 pos)
    {
        if (turrets.ContainsKey(pos))
        {
            var turret = turrets[pos];
            Turret data = turret.Data;
            Player.Instance.AdjustItemAmount(data.key, 1);
            turret.DestroyTurret();
        }
    }

    public void RemoveTurret(Vector2 pos)
    {
        if (turrets.ContainsKey(pos)) turrets.Remove(pos);
    }

    public bool Buildable(Vector2 pos)
    {
        return !turrets.ContainsKey(pos)
            && !MapGenerator.PosOnWall(pos);
    }
}
