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
                key = "WEAPON.GRENADE",
                price = 100,

                attribute = ObjectData.Attribute.EXPLOSION,

                dmg = 30,
                adelay = 0f,
                dmgdelay = 1.5f,

                bulletSpeed = 15,
                radius = 2,

                point = true,
                consumable = true,
            },
            new Weapon()
            {
                key = "WEAPON.PISTOL",
                price = 0,

                attribute = ObjectData.Attribute.BULLET,

                dmg = 5,
                adelay = 0.4f,
                range = 15,
                bulletspreadangle = 10,

                bulletSpeed = 30,

                ammo = 7,
                reload = 1,
                infAmount = true,
            },
            new Weapon()
            {
                key = "WEAPON.SMG",
                price = 300,

                attribute = ObjectData.Attribute.BULLET,

                dmg = 3,
                adelay = 0.05f,
                range = 15,
                bulletspreadangle = 10,

                bulletSpeed = 30,

                ammo = 30,
                reload = 1,
            },
            new Weapon()
            {
                key = "WEAPON.AR",
                price = 1000,

                attribute = ObjectData.Attribute.BULLET,

                dmg = 7,
                adelay = 0.15f,
                range = 25,
                bulletspreadangle = 2,

                bulletSpeed = 40,

                ammo = 30,
                reload = 2,
            },
            new Weapon()
            {
                key = "WEAPON.SHOTGUN",
                price = 1000,

                attribute = ObjectData.Attribute.BULLET,

                dmg = 5,
                adelay = .7f,
                range = 8,
                bulletspreadangle = 45,

                bulletSpeed = 25,
                bullets = 10,

                ammo = 5,
                reload = .7f,
                singleBulletReload = true,
            },
            new Weapon()
            {
                key = "WEAPON.SR",
                price = 1000,

                attribute = ObjectData.Attribute.BULLET,

                dmg = 30,
                adelay = 2f,
                range = 40,
                bulletspreadangle = 2,

                bulletSpeed = 50,
                bullets = 1,

                ammo = 7,
                reload = 2,

                pierce = true,
            },
            new Weapon()
            {
                key = "WEAPON.BAZOOKA",
                price = 1000,

                attribute = ObjectData.Attribute.EXPLOSION,

                dmg = 30,
                adelay = 3f,
                range = 40,
                bulletspreadangle = 0,

                bulletSpeed = 10,
                bullets = 1,
                radius = 3,

                ammo = 1,
                reload = 2,

                point = true,
            },
            new Weapon()
            {
                key = "WEAPON.TESLA",
                price = 1000,

                attribute = ObjectData.Attribute.ELECTRIC,

                dmg = 4,
                adelay = .5f,
                range = 3,
                bulletspreadangle = 2,

                bullets = 5,

                ammo = 30,
                reload = 2,

                autotarget = true,
            },
            new Weapon()
            {
                key = "WEAPON.FLAMETHROWER",
                price = 1000,

                attribute = ObjectData.Attribute.FIRE,

                dmg = 1,
                adelay = .03f,
                range = 8,
                bulletspreadangle = 30,

                bulletSize = .75f,
                bulletSpeed = 10,

                ammo = 100,
                reload = 3,

                pierce = true,
            },
            new Weapon()
            {
                key = "WEAPON.MINIGUN",
                price = 1000,

                attribute = ObjectData.Attribute.BULLET,

                dmg = 7,
                adelay = 0.1f,
                range = 25,
                bulletspreadangle = 10,

                bulletSpeed = 30,

                ammo = 200,
                reload = 4f,
            },
        };
    }

    public static Weapon GetWeapon(string key)
    {
        foreach (Weapon weapon in weapons)
        {
            if (key == weapon.key) return weapon;
        }
        return null;
    }

    public static List<Weapon> GetWeapons()
    {
        List<Weapon> list = new List<Weapon>();

        foreach (Weapon weapon in weapons)
        {
            if (weapon.consumable) continue;
            list.Add(weapon);
        }
        return list;
    }

    public static float GetWeaponParameter(string key)
    {
        float p = 0;
        for (int i = 0; i< weapons.Count; i++)
        {
            if (weapons[i].key == key)
            {
                p = i / (weapons.Count - 1);
                break;
            }
        }

        return p;
    }
}
