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
                key = "ENEMY.ZOMBIE",

                hp = 50,
                speed = 4,
                dmg = 2,
                range = .75f,
                adelay = 1f,

                money = 20,
            },
            new Enemy()
            {
                key = "ENEMY.DOCTORZOMBIE",

                hp = 50,
                speed = 4,
                dmg = 2,
                range = .75f,
                adelay = 1.5f,

                dropItem = new Item()
                {
                    itemKey = "HEALKIT",
                    buff = new BuffInfo(){ hp = 30 },
                    prob = 30,
                },

                thresholdHp = 30,
                restHealAmount = 10,

                money = 20,
            },
            new Enemy()
            {
                key = "ENEMY.FIREFIGHTERZOMBIE",

                hp = 70,
                speed = 4.5f,
                dmg = 4,
                range = .75f,
                adelay = .8f,

                resistances = new Dictionary<ObjectData.Attribute, float>()
                {
                    { ObjectData.Attribute.FIRE, .25f },
                },

                money = 20,
            },
            new Enemy()
            {
                key = "ENEMY.EODZOMBIE",

                hp = 100,
                speed = 3,
                dmg = 5,
                range = .75f,
                adelay = .8f,

                resistances = new Dictionary<ObjectData.Attribute, float>()
                {
                    { ObjectData.Attribute.BULLET, .5f },
                    { ObjectData.Attribute.EXPLOSION, .25f },
                },

                money = 50,
            },
            new Enemy()
            {
                key = "ENEMY.COMMANDOZOMBIE",

                hp = 70,
                speed = 5,
                dmg = 5,
                range = .75f,
                adelay = 1,

                resistances = new Dictionary<ObjectData.Attribute, float>()
                {
                    { ObjectData.Attribute.BULLET, .5f },
                },

                money = 20,
            },
            new Enemy()
            {
                key = "ENEMY.ZOMBIEDOG",

                hp = 30,
                speed = 9,
                dmg = 4,
                range = .75f,
                adelay = 2f,

                money = 10,
            },
            new Enemy()
            {
                key = "ENEMY.SPITTER",

                hp = 100,
                speed = 5.5f,
                dmg = 5,
                range = 5f,
                adelay = 1f,
                projSpeed = 15,
                debuff = new BuffInfo(){ time = 5, speed = -0.4f },

                money = 30,
            },
            new Enemy()
            {
                key = "ENEMY.BOOMER",

                hp = 70,
                speed = 2.5f,
                dmg = 5,
                range = 3.5f,
                adelay = .5f,
                projSpeed = 6,

                projSummon = true,
                summonProb = 50,
                summonUnit = "ENEMY.ZOMBIE",

                money = 30,
            },
            new Enemy()
            {
                key = "ENEMY.TANK",

                hp = 450,
                speed = 3.5f,
                dmg = 10,
                range = 6f,
                adelay = .6f,
                projSpeed = 8,

                isSiege = true,

                money = 100,
            },
            new Enemy()
            {
                key = "ENEMY.RADIOACTIVEWASTE",

                hp = 30,
                speed = 3,
                dmg = 1,
                range = 0.75f,
                adelay = 1,

                thresholdHp = 10,

                money = 0,
            }
            /*
            new Enemy()
            {
                key = "test_buff1",

                hp = 30,
                speed = 4,
                dmg = 0,
                range = 1.2f,
                adelay = 1f,

                buff = new BuffInfo(){ delay = 3, area = true, hp = 1, },

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "test_buff2",

                hp = 30,
                speed = 4,
                dmg = 0,
                range = 1.2f,
                adelay = 1f,

                buff = new BuffInfo(){ delay = 3, time = 3, dmg = 1, def = 1, },

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "test_inv",

                inv = true,

                hp = 15,
                speed = 5,
                dmg = 1,
                range = 3f,
                adelay = 1,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "test_flight",

                fly = true,

                hp = 15,
                speed = 5,
                dmg = 1,
                range = 3f,
                adelay = 1,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "test_invflight",

                inv = true,
                fly = true,

                hp = 15,
                speed = 5,
                dmg = 1,
                range = 3f,
                adelay = 1,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "test_summon",

                hp = 15,
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
                key = "test_combine",

                hp = 20,
                speed = 5,
                dmg = 0,
                range = 1.5f,
                adelay = 1,

                thresholdHp = 5,

                exp = 4,
                money = 10,
            },*/
        };
    }

    public static Enemy GetEnemy(string key)
    {
        foreach (var enemy in enemies)
        {
            if (key == enemy.key) return enemy;
        }
        return null;
    }
}
