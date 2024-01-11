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
    public float splash = 0;
    public int target = 1;

    public void Reload()
    {
        curammo = ammo;
    }
}
