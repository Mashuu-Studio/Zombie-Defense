using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectData
{
    public string key;
}

public abstract class BuyableData : ObjectData
{
    public int price;
}

public class Weapon : BuyableData
{
    public bool usable;

    public int dmg;
    public float adelay;
    public float dmgdelay = 0;
    public float range;
    public int bulletspreadangle;

    public float bulletSize = 1;
    public float bulletSpeed = 20;
    public int bullets = 1;
    public float radius = 0;

    public int ammo;
    public int curammo;
    public float reload;

    public bool pierce = false;
    public bool point = false;
    public bool autotarget = false;
    public bool consumable = false;

    public Weapon() { }

    public Weapon(Weapon w)
    {
        key = w.key;
        price = w.price;

        dmg = w.dmg;
        adelay = w.adelay;
        dmgdelay = w.dmgdelay;
        range = w.range;
        bulletspreadangle = w.bulletspreadangle;

        ammo = curammo = w.ammo;
        reload = w.reload;

        bulletSize = w.bulletSize;
        bulletSpeed = w.bulletSpeed;
        bullets = w.bullets;
        radius = w.radius;

        pierce = w.pierce;
        point = w.point;
        autotarget = w.autotarget;
        consumable = w.consumable;
    }

    public void Reload()
    {
        curammo = ammo;
    }
}
public class Turret : BuyableData
{
    public int hp;
    public int range;
    public int dmg;
    public float adelay;
    public int speed;
}

public class OtherItem : BuyableData
{

}

public class Enemy : ObjectData
{
    public bool inv;
    public bool fly;

    public int hp;
    public int dmg;
    public int speed;
    public float range;
    public float adelay;

    public BuffInfo buff;
    public string summonUnit;
    public float summonCD;
    public int summonAmount;

    public int separate;

    public int exp;
    public int money;
}

