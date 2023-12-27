using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

    [SerializeField] private AstarPath astar;
    private bool updateCol;

    [Space]
    //public MeshGenerator meshGenerator;
    public Tilemap boundaryTilemap;
    public Tilemap wallTilemap;
    public Tilemap wallBottomTilemap;
    public Tilemap grassTilemap;
    [SerializeField] Tile[] tiles;
    [SerializeField] Tile wallBottomTile;

    public Rect mapBoundary { get; private set; }
    public int width, height;
    public string seed;
    public bool useSeed;
    public int smoothing;

    [Range(0, 100)]
    public int randomFillPercent;

    public Bounds MapBounds { get { return new Bounds(Vector3.zero, new Vector3(width, height)); } }
    public int[,] Map { get { return map; } }
    private int[,] map;
    private int squareSize = 1;

    private Transform playerTransform;

    public GameObject shopPrefab;
    private GameObject shop;

    private void Start()
    {
        //astar = gameObject.AddComponent<Astar>();
        GenerateMap();
        CameraController.Instance.SetCamera(Camera.main);

        Vector2 bottomLeft = ConvertToWorldPos(0, 0);
        mapBoundary = new Rect(bottomLeft.x, bottomLeft.y, width, height);

        playerTransform = FindObjectOfType<Player>().transform;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            GenerateMap();
        }
    }

    // Tilemap Collider의 세팅이 전부 끝난 뒤에 Scan을 해야하기 때문에 Scan을 LateUpdate에 배치.
    private void LateUpdate()
    {
        if (updateCol == false)
        {
            astar.Scan();
            updateCol = true;
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

        //meshGen.GenerateMesh(borderedMap, 1);
        //meshGenerator.GenerateMesh(map, squareSize);

        boundaryTilemap.ClearAllTiles();
        grassTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        for (int x = -1; x <= width; x++)
        {
            boundaryTilemap.SetTile((Vector3Int)ConvertToWorldPos(x, -1), tiles[GRASS]);
            boundaryTilemap.SetTile((Vector3Int)ConvertToWorldPos(x, height), tiles[GRASS]);
        }
        for (int y = -1; y <= height; y++)
        {
            boundaryTilemap.SetTile((Vector3Int)ConvertToWorldPos(-1, y), tiles[GRASS]);
            boundaryTilemap.SetTile((Vector3Int)ConvertToWorldPos(width, y), tiles[GRASS]);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int pos = (Vector3Int)ConvertToWorldPos(x, y);
                grassTilemap.SetTile(pos, tiles[GRASS]);
                if (map[x, y] == WALL)
                {
                    wallTilemap.SetTile(pos, tiles[WALL]);
                    if (y > 1 && map[x, y - 1] == GRASS)
                    {
                        pos.y -= 1;
                        wallBottomTilemap.SetTile(pos, wallBottomTile);
                    }
                }
            }
        }

        {
            if (shop != null) Destroy(shop);

            int shopX, shopY;
            do
            {
                shopX = UnityEngine.Random.Range(0, width);
                shopY = UnityEngine.Random.Range(0, height);
            } while (map[shopX, shopY] != GRASS);

            Vector2Int shopPos = ConvertToWorldPos(shopX, shopY);
            shop = Instantiate(shopPrefab);
            shop.transform.position = (Vector2)shopPos;
        }
        Pathfinding.GridGraph astarGrid = (Pathfinding.GridGraph)astar.graphs[0];
        astarGrid.SetDimensions(width + 2, height + 2, 1);
        astarGrid.center = new Vector3(-.5f, 0);
        updateCol = false;
        //astar.SetMap(map);
    }

    public Vector2 GetEnemySpawnPos()
    {
        // 외곽 구역에 생성
        // 좌우와 상하 중 어디에 생성할지 먼저 정한 뒤
        // 좌우에 생성한다면 y값을 랜덤으로, 상하에 생성한다면 x값을 랜덤으로 함.

        int x, y;
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            // 좌우에 생성
            x = UnityEngine.Random.Range(-1, 1);
            if (x == 0) x = width;
            y = UnityEngine.Random.Range(0, height);
        }
        else
        {
            // 상하에 생성
            y = UnityEngine.Random.Range(-1, 1);
            if (y == 0) y = height;
            x = UnityEngine.Random.Range(0, width);
        }

        return ConvertToWorldPos(x, y);

        /*
        // 생성 금지 구역 세팅.
        // 플레이어를 기준으로 맵 전체 1/4 범위 세팅
        Vector2Int playerMapPos = ConvertToMapPos(RoundToInt(playerTransform.position));
        Rect playerZone = new Rect(playerMapPos.x - width / 4, playerMapPos.y - height / 4, width / 2, height / 2);

        int x, y;
        do
        {
            x = UnityEngine.Random.Range(0, width);
            y = UnityEngine.Random.Range(0, height);
            // 플레이어 주변이 아니면서 
            // 선택된 곳이 벽이 아니어야 하고
            // 주변에 벽이 하나 이하로만 있어야 함 (대각선 형태가 생기지 않는 위치)
            if (playerZone.Contains(new Vector2(x, y)) == false &&
                map[x, y] != WALL && GetSurroundGrassCount(x, y) >= 8)
                return ConvertToWorldPos(x, y);
        } while (true);
        */
    }

    public static bool ObjectOnBoundary(Vector2 pos)
    {
        Vector2Int mapPos = ConvertToMapPos(RoundToInt(pos));

        return mapPos.x == -1 || mapPos.x == Instance.width || mapPos.y == -1 || mapPos.y == Instance.height;
    }

    public static Vector2Int GetNearestMapBoundary(Vector2Int pos)
    {
        // x가 -1이라면 0이 제일 가까움
        // x가 width라면 width-1이 제일 가까움
        // y가 -1이라면 0이 제일 가까움
        // y가 height라면 height-1이 제일 가까움

        pos = ConvertToMapPos(pos);

        if (pos.x == -1) pos.x = 0;
        else if (pos.x == Instance.width) pos.x = Instance.width - 1;

        if (pos.y == -1) pos.y = 0;
        else if (pos.y == Instance.height) pos.y = Instance.height - 1;

        pos = ConvertToWorldPos(pos);

        return pos;
    }

    public void UpdateMapPath(Vector2 playerPos)
    {
        /*
        if (astar == null) return;
        astar.UpdateMapPath(playerPos);*/
    }

    public Vector2 FindPath(Vector2 start)
    {
        return Vector2.zero;
        //return astar.FindPath(start);
    }

    public static Vector2Int RoundToInt(Vector2 v)
    {
        return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
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
                int surroundWallAmount = GetSurroundGrassCount(x, y);

                if (surroundWallAmount > 4) map[x, y] = GRASS;
                else if (surroundWallAmount < 4) map[x, y] = WALL;
            }
        }
    }

    private int GetSurroundGrassCount(int gridX, int gridY)
    {
        int grassCount = 0;
        for (int x = gridX - 1; x <= gridX + 1; x++)
        {
            for (int y = gridY - 1; y <= gridY + 1; y++)
            {
                if (x < 0 || x >= width || y < 0 || y >= height) grassCount++;
                else if (!(x == gridX && y == gridY)) grassCount += map[x, y];
            }
        }

        return grassCount;
    }
}
