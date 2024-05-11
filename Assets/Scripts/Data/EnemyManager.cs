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

                hp = 15,
                speed = 3,
                dmg = 0,
                range = .8f,
                adelay = 1.2f,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "ENEMY.DOCTORZOMBIE",

                hp = 15,
                speed = 3,
                dmg = 1,
                range = .8f,
                adelay = 1,

                dropItem = new Item()
                {
                    itemKey = "HEALKIT",
                    buff = new BuffInfo(){ hp = 5 },
                    prob = 50,
                },

                thresholdHp = 10,
                restHealAmount = 2,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "ENEMY.FIREFIGHTERZOMBIE",

                hp = 15,
                speed = 3,
                dmg = 0,
                range = .8f,
                adelay = 1,

                resistances = new Dictionary<ObjectData.Attribute, float>()
                {
                    { ObjectData.Attribute.FIRE, .25f },
                },

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "ENEMY.EODZOMBIE",

                hp = 15,
                speed = 3,
                dmg = 0,
                range = .8f,
                adelay = .75f,

                resistances = new Dictionary<ObjectData.Attribute, float>()
                {
                    { ObjectData.Attribute.BULLET, .25f },
                    { ObjectData.Attribute.EXPLOSION, .25f },
                },

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "ENEMY.COMMANDOZOMBIE",

                hp = 15,
                speed = 3,
                dmg = 0,
                range = .8f,
                adelay = 1,

                resistances = new Dictionary<ObjectData.Attribute, float>()
                {
                    { ObjectData.Attribute.BULLET, .5f },
                },

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "ENEMY.ZOMBIEDOG",

                hp = 15,
                speed = 7,
                dmg = 0,
                range = .8f,
                adelay = 1.5f,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "ENEMY.SPITTER",

                hp = 15,
                speed = 3,
                dmg = 1,
                range = 5f,
                adelay = 1.2f,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "ENEMY.BOOMER",

                hp = 15,
                speed = 3,
                dmg = 1,
                range = 3f,
                adelay = 1.2f,

                debuff = new BuffInfo(){ time = 10, speed = -0.5f },

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "ENEMY.TANK",

                hp = 20,
                speed = 5,
                dmg = 1,
                range = 5f,
                adelay = 1,

                isSiege = true,

                exp = 5,
                money = 10,
            },
            new Enemy()
            {
                key = "ENEMY.RADIOACTIVEWASTE",

                hp = 40,
                speed = 2,
                dmg = 1,
                range = 1.2f,
                adelay = 1,

                separate = 3,

                exp = 4,
                money = 10,
            },
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
            },
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
