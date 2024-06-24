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
                price = 250,

                attribute = ObjectData.Attribute.EXPLOSION,

                dmg = 50,
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

                dmg = 30,
                adelay = .34f,
                range = 10,
                bulletspreadangle = 7,

                bulletSpeed = 30,

                ammo = 15,
                reload = 1,
                infAmount = true,
            },
            new Weapon()
            {
                key = "WEAPON.SMG",
                price = 300,
                magprice = 100,

                attribute = ObjectData.Attribute.BULLET,

                dmg = 15,
                adelay = 0.033f,
                range = 10,
                bulletspreadangle = 10,

                bulletSpeed = 30,

                ammo = 30,
                reload = 1,
            },
            new Weapon()
            {
                key = "WEAPON.AR",
                price = 1500,
                magprice = 500,

                attribute = ObjectData.Attribute.BULLET,

                dmg = 35,
                adelay = 0.15f,
                range = 15,
                bulletspreadangle = 3,

                bulletSpeed = 40,

                ammo = 40,
                reload = 2,
                pierce = 1,
            },
            new Weapon()
            {
                key = "WEAPON.SHOTGUN",
                price = 1000,
                magprice = 250,

                attribute = ObjectData.Attribute.BULLET,

                dmg = 25,
                adelay = .8f,
                range = 4,
                bulletspreadangle = 45,

                bulletSpeed = 25,
                bullets = 10,

                ammo = 7,
                reload = .5f,
                singleBulletReload = true,
            },
            new Weapon()
            {
                key = "WEAPON.SR",
                price = 3000,
                magprice = 1500,

                attribute = ObjectData.Attribute.BULLET,

                dmg = 100,
                adelay = 1f,
                range = 20,
                bulletspreadangle = 1,

                bulletSpeed = 50,

                ammo = 10,
                reload = 2,

                pierce = 6,
            },
            new Weapon()
            {
                key = "WEAPON.BAZOOKA",
                price = 5000,
                magprice = 750,

                attribute = ObjectData.Attribute.EXPLOSION,

                dmg = 200,
                adelay = 2f,
                range = 10,
                bulletspreadangle = 3,

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
                price = 3000,
                magprice = 1500,

                attribute = ObjectData.Attribute.ELECTRIC,

                dmg = 25,
                adelay = .5f,
                range = 3,
                bulletspreadangle = 0,

                bullets = 25,

                ammo = 20,
                reload = 1,

                autotarget = true,
            },
            new Weapon()
            {
                key = "WEAPON.FLAMETHROWER",
                price = 3000,
                magprice = 1500,

                attribute = ObjectData.Attribute.FIRE,

                dmg = 5,
                adelay = .05f,
                range = 6,
                bulletspreadangle = 30,

                bulletSize = .75f,
                bulletSpeed = 10,

                ammo = 100,
                reload = 2,

                pierce = 25,
            },
            new Weapon()
            {
                key = "WEAPON.MINIGUN",
                price = 7500,
                magprice = 2500,

                attribute = ObjectData.Attribute.BULLET,

                dmg = 60,
                adelay = 0.2f,
                range = 15,
                bulletspreadangle = 10,

                bulletSpeed = 30,

                ammo = 100,
                reload = 4f,

                pierce = 1,
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
        for (int i = 0; i < weapons.Count; i++)
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
