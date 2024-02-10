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
                key = "TURRET.BARRICADE",
                hp = 10,
            },
            new Turret()
            {
                key = "TURRET.TURRET",
                hp = 3,
                dmg = 1,
                range = 5,
                speed = 30,
                adelay = 1 ,
            },
            new Turret()
            {
                key = "TURRET.SCANTOWER",
                hp = 10,
                range = 5,
            }
        };
    }

    public static Turret GetTurret(string key)
    {
        foreach (Turret turret in turrets)
        {
            if (key == turret.key) return turret;
        }
        return null;
    }
}
