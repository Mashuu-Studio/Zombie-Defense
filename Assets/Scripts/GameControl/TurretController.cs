using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public static TurretController Instance { get { return instance; } }
    private static TurretController instance;

    private static Vector2[] directions;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        directions = new Vector2[]
        {
            new Vector2(-1,1),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0),
            new Vector2(0,0),
            new Vector2(-1,0),
            new Vector2(1,-1),
            new Vector2(0,-1),
            new Vector2(-1,-1),
        };
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

    public bool BuildMode { get { return buildMode; } }
    private bool buildMode;
    private string selectedTurretKey;
    private string selectedWeaponKey;

    public void SelectBuildingTurret(string key)
    {
        selectedTurretKey = key;
        turretPointer.ChangeSprite(SpriteManager.GetSprite(key));
    }

    public void SelectWeapon(string key)
    {
        selectedWeaponKey = key;
    }

    public void ChangeBulidMode(bool b)
    {
        buildMode = b;
    }

    void Update()
    {
        if (GameController.Instance.GameStarted == false 
            || GameController.Instance.Pause
            || RoundController.Instance.Progress) return;

        turretPointer.gameObject.SetActive(buildMode);
        if (BuildMode)
        {
            Vector3 mousePos = CameraController.Instance.Cam.ScreenToWorldPoint(Input.mousePosition);
            Vector2 pos = PosToGrid(MapGenerator.RoundToInt(mousePos));
            turretPointer.transform.position = pos;
            turretPointer.SetColor(Buildable(pos));
            
            float axisX = Input.GetAxis("Horizontal");
            float axisY = Input.GetAxis("Vertical");
            Vector3 movePos = CameraController.Instance.Cam.transform.position + new Vector3(axisX, axisY) * Time.deltaTime * 10;
            CameraController.Instance.MoveCamera(movePos, movePos);

            // ÅÍ·¿ ±¸¸Å ¹× ºôµå
            if (Input.GetMouseButton(0) && !UIController.PointOverUI())
            {
                BuildTurret(turretPointer.transform.position, selectedTurretKey);
            }

            // ÅÍ·¿ º¸°ü
            if (Input.GetMouseButton(1))
            {
                StoreTurret(turretPointer.transform.position);
            }

            // ¸¶¿îÆ®
            if (Input.GetKeyDown(KeyCode.Q) && turrets.ContainsKey(turretPointer.transform.position))
            {
                var turret = turrets[turretPointer.transform.position] as AttackTurretObject;
                if (turret) turret.Mount(WeaponManager.GetWeapon(selectedWeaponKey), true);
            }
        }
    }

    public void BuildTurret(Vector2 pos, string name)
    {
        Turret data = TurretManager.GetTurret(name);
        if (data == null || !Buildable(pos)) return;
        if (Player.Instance.ItemAmount(data.key) <= 0
            && !Player.Instance.BuyItem(data)) return;

        Poolable obj = PoolController.Pop(name);
        TurretObject turret = obj.GetComponent<TurretObject>();
        if (turret == null)
        {
            PoolController.Push(name, obj);
            return;
        }
        Player.Instance.AdjustItemAmount(data.key, -1);

        turret.SetData(data, pos);
        turret.gameObject.name = name;
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

    private bool Buildable(Vector2 pos)
    {
        Vector2Int mapPos = MapGenerator.ConvertToMapPos(MapGenerator.RoundToInt(pos));
        
        return !turrets.ContainsKey(pos)  
            && MapGenerator.PosOnMap(mapPos)
            && !(MapGenerator.Instance.Map[mapPos.x, mapPos.y] == MapGenerator.WALL);
    }

    private static Vector2Int PosToGrid(Vector2 pos)
    {
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        return new Vector2Int(x, y);
    }
}
