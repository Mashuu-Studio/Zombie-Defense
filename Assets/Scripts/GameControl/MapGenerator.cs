using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public static MapGenerator Instance { get { return instance; } }
    private static MapGenerator instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public const int WALL = 0;
    public const int GRASS = 1;

    public Rect mapBoundary { get; private set; }
    public int width, height;
    public string seed;
    public bool useSeed;
    public int smoothing;

    [Range(0, 100)]
    public int randomFillPercent;

    public int[,] Map { get { return map; } }
    private int[,] map;
    private Astar astar;
    private int squareSize = 1;

    private void Start()
    {
        astar = gameObject.AddComponent<Astar>();
        GenerateMap();

        Vector2 bottomLeft = ConvertToWorldPos(0, 0);
        mapBoundary = new Rect(bottomLeft.x, bottomLeft.y, width, height);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            GenerateMap();
        }
    }

    private void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();
        for (int i = 0; i < smoothing; i++)
            SmoothMap();
        /*
        int borderSize = 2;
        int[,] borderedMap = new int[width + borderSize * 2, height + borderSize * 2];

        for (int x = 0; x < borderedMap.GetLength(0); x++)
        {
            for (int y = 0; y < borderedMap.GetLength(1); y++)
            {
                if (x < borderSize || x >= width + borderSize || y < borderSize || y >= height + borderSize)
                    borderedMap[x, y] = 0;
                else borderedMap[x, y] = map[x - borderSize, y - borderSize];
            }
        }*/

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        //meshGen.GenerateMesh(borderedMap, 1);
        meshGen.GenerateMesh(map, squareSize);
        astar.SetMap(map);
    }

    public Vector2 GetEnemySpawnPos()
    {
        int x, y;
        do
        {
            x = UnityEngine.Random.Range(0, width);
            y = UnityEngine.Random.Range(0, height);
            if (map[x, y] != WALL) return ConvertToWorldPos(x, y);
        } while (true);
    }

    public void UpdateMapPath(Vector2 playerPos)
    {
        if (astar == null) return;
        astar.UpdateMapPath(playerPos);
    }

    public List<Vector2Int> FindPath(Vector2 start)
    {
        return astar.FindPath(start);
    }

    public static Vector2Int ConvertToWorldPos(int x, int y)
    {
        return new Vector2Int(x - Instance.map.GetLength(0) / 2, y - Instance.map.GetLength(1) / 2) * Instance.squareSize;
    }
    public static Vector2Int ConvertToWorldPos(Vector2Int mapPos)
    {
        return new Vector2Int(mapPos.x - Instance.map.GetLength(0) / 2, mapPos.y - Instance.map.GetLength(1) / 2) * Instance.squareSize;
    }
    public static Vector2Int ConvertToMapPos(Vector2Int worldPos)
    {
        return new Vector2Int(worldPos.x + Instance.map.GetLength(0) / 2, worldPos.y + Instance.map.GetLength(1) / 2) * Instance.squareSize;
    }

    private void RandomFillMap()
    {
        if (!useSeed) seed = Time.time.ToString();

        System.Random rand = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // terrain 0: wall, 1: grass
                int terrain = WALL;
                if (rand.Next() % 100 <= randomFillPercent) terrain = GRASS;
                map[x, y] = terrain;
            }
        }
    }

    private void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int surroundWallAmount = GetSurroundWallCount(x, y);

                if (surroundWallAmount > 4) map[x, y] = GRASS;
                else if (surroundWallAmount < 4) map[x, y] = WALL;
            }
        }
    }

    private int GetSurroundWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int x = gridX - 1; x <= gridX + 1; x++)
        {
            for (int y = gridY - 1; y <= gridY + 1; y++)
            {
                if (x < 0 || x >= width || y < 0 || y >= height) wallCount++;
                else if (!(x == gridX && y == gridY)) wallCount += map[x, y];
            }
        }

        return wallCount;
    }
}
