using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingController : MonoBehaviour
{
    public static BuildingController Instance { get { return instance; } }
    private static BuildingController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    [SerializeField] private BuildPointer buildingPointer;

    private Dictionary<Vector2, BuildingObject> buildings = new Dictionary<Vector2, BuildingObject>();

    public void StartGame()
    {
        List<BuildingObject> list = buildings.Values.ToList();
        while (list.Count > 0)
        {
            list[0].DestroyBuilding();
            list.RemoveAt(0);
        }
        buildings.Clear();
    }

    #region BuildMode
    public bool BuildMode { get { return buildMode; } }
    private bool buildMode;
    private string selectedBuildingKey;

    public void SelectBuildingOnBuildMode(string key)
    {
        selectedBuildingKey = key;
        buildingPointer.ChangeSprite(SpriteManager.GetSprite(key));
    }

    public void ChangeBulidMode(bool b)
    {
        buildMode = b;
        buildingPointer.gameObject.SetActive(buildMode);
    }

    public void MoveBuildingPointer(Vector2 pos)
    {
        buildingPointer.transform.position = pos;
        buildingPointer.SetColor(Buildable(pos));
    }

    public Vector2 selectedBuildingPos;
    public bool SelectBuilding(Vector2 pos)
    {
        selectedBuildingPos = pos;
        return buildings.ContainsKey(pos);
    }

    public void Mount(string key)
    {
        var turret = buildings[selectedBuildingPos] as TurretObject;
        if (turret) turret.Mount(WeaponManager.GetWeapon(key), true);
    }
    #endregion

    public void Build(Vector2 pos)
    {
        Building data = BuildingManager.GetBuilding(selectedBuildingKey);
        if (data == null || !Buildable(pos)) return;
        if (Player.Instance.ItemAmount(data.key) <= 0)
        {
            // 없다면 구매. 구매할 수 없다면 짓지 않음.
            if (Player.Instance.BuyItem(data)) Player.Instance.AdjustItemAmount(data.key, 1);
            else return;
        }

        Poolable obj = PoolController.Pop(selectedBuildingKey);
        BuildingObject building = obj.GetComponent<BuildingObject>();
        if (building == null)
        {
            PoolController.Push(selectedBuildingKey, obj);
            return;
        }
        Player.Instance.AdjustItemAmount(data.key, -1);

        building.SetData(data, pos);
        building.gameObject.name = selectedBuildingKey;
        building.transform.SetParent(transform);
        building.transform.position = pos;
        buildings.Add(pos, building);

        MapGenerator.Instance.UpdateAstar();
    }

    public void Store(Vector2 pos)
    {
        if (buildings.ContainsKey(pos))
        {
            var building = buildings[pos];
            // 사용한 적 없는 경우에만 회수. 아니면 파괴
            if (!building.AlreadyUsed) Player.Instance.AdjustItemAmount(building.Data.key, 1);
            building.DestroyBuilding();
        }
    }

    public void RemoveBuilding(Vector2 pos)
    {
        if (buildings.ContainsKey(pos))
        {
            buildings.Remove(pos);
            MapGenerator.Instance.UpdateAstar();
        }
    }

    public bool Buildable(Vector2 pos)
    {
        return !buildings.ContainsKey(pos)
            && !MapGenerator.PosOnWall(pos);
    }
}
