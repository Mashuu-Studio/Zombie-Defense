using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BuildingManager
{
    public static List<Building> Buildings { get { return buildings; } }
    private static List<Building> buildings;
    public static void Init()
    {
        buildings = new List<Building>()
        {
            new Building()
            {
                key = "BUILDING.WOODENBARRICADE",
                hp = 50,
                price = 100,
            },
            new Building()
            {
                key = "BUILDING.IRONBARRICADE",
                hp = 100,
                price = 300,
            },
            new Building()
            {
                key = "BUILDING.CONCRETEBARRICADE",
                hp = 150,
                price = 500,
            },
            new Building()
            {
                key = "BUILDING.BARBEDWIRE",
                hp = 10,
                dmg = 10,
                adelay = 1,
                buff = new BuffInfo(){time = 5, speed = -.75f},
                price = 1000,
            },
            new Building()
            {
                key = "BUILDING.SPIKETRAP",
                hp = 10,
                dmg = 30,
                adelay = 1,
                price = 1000,
            },
            new Building()
            {
                key = "BUILDING.LANDMINE",
                hp = 1,
                dmg = 100,
                range = 2,
                adelay = 1,
                price = 500,
            },
            new Building()
            {
                key = "BUILDING.TURRET",
                hp = 30,
                range = 5,
                speed = 30,
                adelay = 1,
                price = 1500,
            },
        };
    }

    public static Building GetBuilding(string key)
    {
        foreach (Building building in buildings)
        {
            if (key == building.key) return building;
        }
        return null;
    }
}
