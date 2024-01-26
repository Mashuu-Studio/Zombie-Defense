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

    public void StartGame()
    {
        foreach (var turret in turrets.Values)
        {
            turret.DestroyTurret();
        }
        turrets.Clear();
    }

    [SerializeField] private Transform turretPointer;

    void Update()
    {
        if (GameController.Instance.GameStarted == false || GameController.Instance.Pause) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        turretPointer.position = GetDirection(Player.Instance.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddTurret(turretPointer.position, "Barricade");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddTurret(turretPointer.position, "Turret");
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (turrets.ContainsKey(turretPointer.position))
            {
                var turret = (AttackTurretObject)turrets[turretPointer.position];
                if (turret)
                {
                    turret.Mount(WeaponController.Instance.CurWeapon);
                }
            }
        }
    }

    private Dictionary<Vector2, TurretObject> turrets = new Dictionary<Vector2, TurretObject>();

    public void AddTurret(Vector2 pos, string name)
    {
        Turret data = TurretManager.GetTurret(name);
        if (data == null) return;
        if (turrets.ContainsKey(pos)) return;

        Poolable obj = PoolController.Pop(name);
        TurretObject turret = obj.GetComponent<TurretObject>();
        if (turret == null)
        {
            PoolController.Push(name, obj);
            return;
        }
        turret.SetData(data, pos);
        turret.gameObject.name = name;
        turret.transform.parent = transform;
        turret.transform.position = pos;
        turrets.Add(pos, turret);
    }

    public void RemoveTurret(Vector2 pos)
    {
        if (turrets.ContainsKey(pos)) turrets.Remove(pos);
    }

    public static Vector2 GetDirection(Vector2 charPos, Vector3 mouseWorldPos)
    {
        /* [-1, 1] [0, 1] [1, 1]
         * [-1, 0] [0, 0] [1, 0]
         * [-1,-1] [0,-1] [1,-1]
         */

        /* 타일은 grid.cellSize만큼 나누어져 있음
         * 따라서 worldPos에서 내림을 통해 사각형의 왼쪽 아래 꼭지점을 얻음.
         * 이후 cellSize의 절반만큼 위치를 조정해주면 타일의 위치가 됨.
         */

        /* 캐릭터를 기준으로 타워를 지을 수 있는 범위를 본인 위치를 포함해 8방향 한 칸으로 하고 싶음.
         * 우선, 캐릭터를 기준으로 주변 타일을 체크함.
         * 이 후 현재 마우스의 위치와 거리를 비교해서 해당 위치에 포인터 리턴
         */
        charPos = MapGenerator.RoundToInt(charPos);

        Vector2 buildPos = PosToGrid(charPos + directions[0]);
        float minDistance = Vector2.Distance(buildPos, mouseWorldPos);
        for (int i = 1; i < directions.Length; i++)
        {
            Vector2 cmpPos = PosToGrid(charPos + directions[i]);
            float distance = Vector2.Distance(cmpPos, mouseWorldPos);

            if (minDistance > distance)
            {
                minDistance = distance;
                buildPos = cmpPos;
            }
        }

        return buildPos;
        /*
        Vector3 dir = mousePos - charPos;
        float deg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (deg > 180 - 22.5f) return directions[7];
        else if (deg > 180 - 22.5f * 3) return directions[0];
        else if (deg > 180 - 22.5f * 5) return directions[1];
        else if (deg > 180 - 22.5f * 7) return directions[2];
        else if (deg > 180 - 22.5f * 9) return directions[3];
        else if (deg > 180 - 22.5f * 11) return directions[4];
        else if (deg > 180 - 22.5f * 13) return directions[5];
        else if (deg > 180 - 22.5f * 15) return directions[6];
        else return directions[7];*/
    }

    private static Vector2 PosToGrid(Vector2 pos)
    {
        float x = Mathf.FloorToInt(pos.x);
        float y = Mathf.FloorToInt(pos.y);
        return new Vector2(x, y);
    }
}
