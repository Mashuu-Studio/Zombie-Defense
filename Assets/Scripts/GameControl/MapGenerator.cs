using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public int width, height;
    public string seed;
    public bool useSeed;
    public int smoothing;

    [Range(0, 100)]
    public int randomFillPercent;
    int[,] map;

    private void Start()
    {
        GenerateMap();
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

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(map, 1);
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
                int terrain = 0;
                if (rand.Next() % 100 <= randomFillPercent) terrain = 1;
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

                if (surroundWallAmount > 4) map[x, y] = 1;
                else if (surroundWallAmount < 4) map[x, y] = 0;
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

    private void OnDrawGizmos()
    {/*
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? new Color(.2f, .5f, .2f) : new Color(.65f, .45f, .4f);
                    Vector2 pos = new Vector2(x - width / 2 + .5f, y - height / 2 + .5f);
                    Gizmos.DrawCube(pos, Vector2.one);
                }
            }
        }*/
    }
}
