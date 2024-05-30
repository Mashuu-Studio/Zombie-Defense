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
                hp = 10,
            },
            new Building()
            {
                key = "BUILDING.IRONBARRICADE",
                hp = 20,
            },
            new Building()
            {
                key = "BUILDING.CONCRETEBARRICADE",
                hp = 30,
            },
            new Building()
            {
                key = "BUILDING.TURRET",
                hp = 3,
                dmg = 1,
                range = 5,
                speed = 30,
                adelay = 1 ,
            },
            new Building()
            {
                key = "BUILDING.TRAP",
                hp = 10,
                dmg = 2,
                adelay = 1,
            },
            new Building()
            {
                key = "BUILDING.BARBEDWIRE",
                hp = 10,
                adelay = 1,
                buff = new BuffInfo(){time = 5, speed = -.5f},
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
