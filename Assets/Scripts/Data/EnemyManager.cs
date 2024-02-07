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
        {
            new Enemy()
            {
                name = "test1",

                hp = 3,
                speed = 5,
                dmg = 0,
                range = 3f,
                adelay = 1,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                name = "test2",

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
                name = "test3",

                hp = 1,
                speed = 5,
                dmg = 2,
                range = 1.2f,
                adelay = .5f,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                name = "test_buff1",

                hp = 5,
                speed = 4,
                dmg = 1,
                range = 1.2f,
                adelay = 1f,

                buff = new BuffInfo(){ delay = 3, area = true, hp = 1, },

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                name = "test_buff2",

                hp = 5,
                speed = 4,
                dmg = 1,
                range = 1.2f,
                adelay = 1f,

                buff = new BuffInfo(){ delay = 3, time = 3, dmg = 1, def = 1, },

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                name = "test_inv",

                inv = true,

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
                name = "test_flight",

                fly = true,

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
                name = "test_invflight",

                inv = true,
                fly = true,

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
                name = "test_summon",

                hp = 3,
                speed = 2,
                dmg = 1,
                range = 3f,
                adelay = 1,

                summonUnit = "test3",
                summonCD = 1f,
                summonAmount = 2,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                name = "test_divide",

                hp = 8,
                speed = 2,
                dmg = 1,
                range = 3f,
                adelay = 1,

                separate = 3,

                exp = 4,
                money = 10,
            },
        };
    }

    public static Enemy GetEnemy(string name)
    {
        foreach (var enemy in enemies)
        {
            if (name == enemy.name) return enemy;
        }
        return null;
    }
}
