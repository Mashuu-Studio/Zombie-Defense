using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyManager
{
    public static List<Enemy> Enemies { get { return enemies; } }
    private static List<Enemy> enemies;
    public static void Init()
    {
        enemies = new List<Enemy>()
        {/*
            new Enemy()
            {
                hp = 3,
                speed = 5,
                dmg = 1,
                range = 3f,
                adelay = 1,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                hp = 5,
                speed = 2,
                dmg = 3,
                range = 0.8f,
                adelay = 3,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                hp = 1,
                speed = 4,
                dmg = 2,
                range = 1.2f,
                adelay = .5f,

                exp = 5,
                money = 10,
            },*/
            new Enemy()
            {
                buff = new BuffInfo(){delay = 3, area = true, hp = 1, },

                hp = 5,
                speed = 4,
                dmg = 1,
                range = 1.2f,
                adelay = 1f,

                exp = 5,
                money = 10,

            },
            new Enemy()
            {
                buff = new BuffInfo(){ delay = 3, time = 3, dmg = 1, def = 1, },

                hp = 5,
                speed = 4,
                dmg = 1,
                range = 1.2f,
                adelay = 1f,

                exp = 5,
                money = 10,
            }
        };
    }
}
