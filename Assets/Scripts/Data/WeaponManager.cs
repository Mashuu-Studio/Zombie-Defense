using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WeaponManager
{
    public static List<Weapon> Weapons { get { return weapons; } }
    private static List<Weapon> weapons;
    public static void Init()
    {
        weapons = new List<Weapon>()
        {
            new Weapon()
            {
                name = "PISTOL",
                price = 0,

                dmg = 1,
                adelay = 0.4f,
                range = 15,
                bulletspreadangle = 10,

                ammo = 7,
                reload = 1,
            },
            new Weapon()
            {
                name = "SMG",
                price = 300,

                dmg = 1,
                adelay = 0.05f,
                range = 15,
                bulletspreadangle = 10,

                ammo = 30,
                reload = 1,
            },
            new Weapon()
            {
                name = "ASSAULT RIFLE",
                price = 1000,

                dmg = 2,
                adelay = 0.1f,
                range = 25,
                bulletspreadangle = 5,

                ammo = 30,
                reload = 2,
            },
            new Weapon()
            {
                name = "SHOTGUN",
                price = 1000,

                dmg = 3,
                adelay = 2f,
                range = 8,
                bulletspreadangle = 45,
                bullets = 10,

                ammo = 5,
                reload = 3,
            },
            new Weapon()
            {
                name = "SNIPER RIFLE",
                price = 1000,

                dmg = 5,
                adelay = 2f,
                range = 40,
                bulletspreadangle = 2,
                bullets = 1,

                ammo = 7,
                pierce = true,
                reload = 2,
            },
            new Weapon()
            {
                name = "BAZUKA",
                price = 1000,

                dmg = 5,
                adelay = 3f,
                range = 40,
                bulletspreadangle = 2,
                bullets = 1,

                ammo = 1,
                point = 1,
                splash = 10,
                reload = 2,
            },
            new Weapon()
            {
                name = "TESLA",
                price = 1000,

                dmg = 1,
                adelay = .5f,
                range = 5,
                bulletspreadangle = 2,
                bullets = 5,

                ammo = 30,
                reload = 2,
                autotarget = true,
            },
        };
    }

    public static Weapon GetWeapon(string name)
    {
        foreach (Weapon weapon in weapons)
        {
            if (name == weapon.name) return weapon;
        }
        return null;
    }
}
