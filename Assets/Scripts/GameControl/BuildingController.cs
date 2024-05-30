using System.Collections;
using System.Collections.Generic;
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
        foreach (var building in buildings.Values)
        {
            building.DestroyBuilding();
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
        var building = buildings[selectedBuildingPos] as TurretObject;
        if (building) building.Mount(WeaponManager.GetWeapon(key), true);
    }
    #endregion

    public void Build(Vector2 pos)
    {
        Building data = BuildingManager.GetBuilding(selectedBuildingKey);
        if (data == null || !Buildable(pos)) return;
        if (Player.Instance.ItemAmount(data.key) <= 0
            && !Player.Instance.BuyItem(data)) return;

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
        building.transform.parent = transform;
        building.transform.position = pos;
        buildings.Add(pos, building);
    }

    public void Store(Vector2 pos)
    {
        if (buildings.ContainsKey(pos))
        {
            var building = buildings[pos];
            Building data = building.Data;
            Player.Instance.AdjustItemAmount(data.key, 1);
            building.DestroyBuilding();
        }
    }

    public void RemoveBuilding(Vector2 pos)
    {
        if (buildings.ContainsKey(pos)) buildings.Remove(pos);
    }

    public bool Buildable(Vector2 pos)
    {
        return !buildings.ContainsKey(pos)
            && !MapGenerator.PosOnWall(pos);
    }
}
