using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public string name;
    public int price;

    public int dmg;
    public float adelay;
    public float range;
    public int bulletspreadangle;

    public int ammo;
    public int curammo;
    public float reload;

    public int bullets = 1;
    public bool pierce = false;
    public float splash = 0;
    public int target = 1;

    public Weapon() { }

    public Weapon(Weapon w)
    {
        name = w.name;
        price = w.price;

        dmg = w.dmg;
        adelay = w.adelay;
        range = w.range;
        bulletspreadangle = w.bulletspreadangle;

        ammo = curammo = w.ammo;
        reload = w.reload;

        bullets = w.bullets;
        pierce = w.pierce;
        splash = w.splash;
        target = w.target;
    }

    public void Reload()
    {
        curammo = ammo;
    }
}
