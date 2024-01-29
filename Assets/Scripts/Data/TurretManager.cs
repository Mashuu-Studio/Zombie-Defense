using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TurretManager
{
    public static List<Turret> Turrets { get { return turrets; } }
    private static List<Turret> turrets;
    public static void Init()
    {
        turrets = new List<Turret>()
        {
            new Turret()
            {
                name = "Barricade",
                hp = 10,
            },
            new Turret()
            {
                name = "Turret",
                hp = 3,
                dmg = 1,
                range = 5,
                speed = 30,
                adelay = 1 ,
            },
            new Turret()
            {
                name = "Scan Tower",
                hp = 10,
                range = 5,
            }
        };
    }

    public static Turret GetTurret(string name)
    {
        foreach (Turret turret in turrets)
        {
            if (name == turret.name) return turret;
        }
        return null;
    }
}
