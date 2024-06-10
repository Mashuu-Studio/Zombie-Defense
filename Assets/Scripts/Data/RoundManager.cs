using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoundManager
{
    public static List<Round> Rounds { get { return rounds; } }
    private static List<Round> rounds;

    public static void Init()
    {
        rounds = new List<Round>()
        {
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 50 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 60 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 70 },
                    { "ENEMY.ZOMBIEDOG", 15 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 80 },
                    { "ENEMY.ZOMBIEDOG", 15 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 80 },
                    { "ENEMY.DOCTORZOMBIE", 6 },
                    { "ENEMY.ZOMBIEDOG", 15 },
                    { "ENEMY.COMMANDOZOMBIE", 10 },
                },
            },

            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 100 },
                    { "ENEMY.DOCTORZOMBIE", 6 },
                    { "ENEMY.ZOMBIEDOG", 20 },
                    { "ENEMY.COMMANDOZOMBIE", 10 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 100 },
                    { "ENEMY.DOCTORZOMBIE", 6 },
                    { "ENEMY.ZOMBIEDOG", 20 },
                    { "ENEMY.COMMANDOZOMBIE", 10 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 100 },
                    { "ENEMY.DOCTORZOMBIE", 7 },
                    { "ENEMY.ZOMBIEDOG", 30 },
                    { "ENEMY.COMMANDOZOMBIE", 20 },
                    { "ENEMY.EODZOMBIE", 10 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 100 },
                    { "ENEMY.DOCTORZOMBIE", 7 },
                    { "ENEMY.ZOMBIEDOG", 30 },
                    { "ENEMY.COMMANDOZOMBIE", 20 },
                    { "ENEMY.EODZOMBIE", 15 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 100 },
                    { "ENEMY.DOCTORZOMBIE", 10 },
                    { "ENEMY.ZOMBIEDOG", 40 },
                    { "ENEMY.COMMANDOZOMBIE", 30 },
                    { "ENEMY.EODZOMBIE", 15 },
                    { "ENEMY.SPITTER", 7 },
                },
            },

            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 100 },
                    { "ENEMY.DOCTORZOMBIE", 10 },
                    { "ENEMY.ZOMBIEDOG", 40 },
                    { "ENEMY.COMMANDOZOMBIE", 30 },
                    { "ENEMY.EODZOMBIE", 15 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 25 },
                    { "ENEMY.SPITTER", 7 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 120 },
                    { "ENEMY.DOCTORZOMBIE", 10 },
                    { "ENEMY.ZOMBIEDOG", 50 },
                    { "ENEMY.COMMANDOZOMBIE", 40 },
                    { "ENEMY.EODZOMBIE", 17 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 25 },
                    { "ENEMY.SPITTER", 10 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 120 },
                    { "ENEMY.DOCTORZOMBIE", 10 },
                    { "ENEMY.ZOMBIEDOG", 60 },
                    { "ENEMY.COMMANDOZOMBIE", 40 },
                    { "ENEMY.EODZOMBIE", 17 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 35 },
                    { "ENEMY.SPITTER", 10 },
                    { "ENEMY.BOOMER", 3 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 150 },
                    { "ENEMY.DOCTORZOMBIE", 15 },
                    { "ENEMY.ZOMBIEDOG", 60 },
                    { "ENEMY.COMMANDOZOMBIE", 50 },
                    { "ENEMY.EODZOMBIE", 25 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 50 },
                    { "ENEMY.SPITTER", 20 },
                    { "ENEMY.BOOMER", 5 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 150 },
                    { "ENEMY.DOCTORZOMBIE", 15 },
                    { "ENEMY.ZOMBIEDOG", 80 },
                    { "ENEMY.COMMANDOZOMBIE", 60 },
                    { "ENEMY.EODZOMBIE", 30 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 60 },
                    { "ENEMY.SPITTER", 20 },
                    { "ENEMY.BOOMER", 8 },
                },
            },

            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 160 },
                    { "ENEMY.DOCTORZOMBIE", 15 },
                    { "ENEMY.ZOMBIEDOG", 80 },
                    { "ENEMY.COMMANDOZOMBIE", 60 },
                    { "ENEMY.EODZOMBIE", 30 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 60 },
                    { "ENEMY.SPITTER", 25 },
                    { "ENEMY.BOOMER", 12 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 180 },
                    { "ENEMY.DOCTORZOMBIE", 15 },
                    { "ENEMY.ZOMBIEDOG", 100 },
                    { "ENEMY.COMMANDOZOMBIE", 70 },
                    { "ENEMY.EODZOMBIE", 35 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 70 },
                    { "ENEMY.SPITTER", 25 },
                    { "ENEMY.BOOMER", 20 },
                    { "ENEMY.TANK", 3 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 200 },
                    { "ENEMY.DOCTORZOMBIE", 15 },
                    { "ENEMY.ZOMBIEDOG", 100 },
                    { "ENEMY.COMMANDOZOMBIE", 70 },
                    { "ENEMY.EODZOMBIE", 35 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 70 },
                    { "ENEMY.SPITTER", 30 },
                    { "ENEMY.BOOMER", 20 },
                    { "ENEMY.TANK", 3 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 200 },
                    { "ENEMY.DOCTORZOMBIE", 20 },
                    { "ENEMY.ZOMBIEDOG", 100 },
                    { "ENEMY.COMMANDOZOMBIE", 80 },
                    { "ENEMY.EODZOMBIE", 40 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 80 },
                    { "ENEMY.SPITTER", 40 },
                    { "ENEMY.BOOMER", 30 },
                    { "ENEMY.TANK", 3 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 250 },
                    { "ENEMY.DOCTORZOMBIE", 20 },
                    { "ENEMY.ZOMBIEDOG", 120 },
                    { "ENEMY.COMMANDOZOMBIE", 100 },
                    { "ENEMY.EODZOMBIE", 50 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 100 },
                    { "ENEMY.SPITTER", 50 },
                    { "ENEMY.BOOMER", 30 },
                    { "ENEMY.TANK", 5 },
                },
            },

            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 250 },
                    { "ENEMY.DOCTORZOMBIE", 20 },
                    { "ENEMY.ZOMBIEDOG", 120 },
                    { "ENEMY.COMMANDOZOMBIE", 100 },
                    { "ENEMY.EODZOMBIE", 50 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 110 },
                    { "ENEMY.SPITTER", 50 },
                    { "ENEMY.BOOMER", 40 },
                    { "ENEMY.TANK", 5 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 260 },
                    { "ENEMY.DOCTORZOMBIE", 25 },
                    { "ENEMY.ZOMBIEDOG", 120 },
                    { "ENEMY.COMMANDOZOMBIE", 100 },
                    { "ENEMY.EODZOMBIE", 50 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 120 },
                    { "ENEMY.SPITTER", 50 },
                    { "ENEMY.BOOMER", 40 },
                    { "ENEMY.TANK", 7 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 270 },
                    { "ENEMY.DOCTORZOMBIE", 25 },
                    { "ENEMY.ZOMBIEDOG", 130 },
                    { "ENEMY.COMMANDOZOMBIE", 120 },
                    { "ENEMY.EODZOMBIE", 55 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 120 },
                    { "ENEMY.SPITTER", 60 },
                    { "ENEMY.BOOMER", 50 },
                    { "ENEMY.TANK", 10 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 280 },
                    { "ENEMY.DOCTORZOMBIE", 25 },
                    { "ENEMY.ZOMBIEDOG", 130 },
                    { "ENEMY.COMMANDOZOMBIE", 120 },
                    { "ENEMY.EODZOMBIE", 55 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 130 },
                    { "ENEMY.SPITTER", 60 },
                    { "ENEMY.BOOMER", 50 },
                    { "ENEMY.TANK", 15 },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "ENEMY.ZOMBIE", 300 },
                    { "ENEMY.DOCTORZOMBIE", 25 },
                    { "ENEMY.ZOMBIEDOG", 150 },
                    { "ENEMY.COMMANDOZOMBIE", 150 },
                    { "ENEMY.EODZOMBIE", 65 },
                    { "ENEMY.FIREFIGHTERZOMBIE", 150 },
                    { "ENEMY.SPITTER", 80 },
                    { "ENEMY.BOOMER", 70 },
                    { "ENEMY.TANK", 20 },
                },
            },
        };
    }

    public static Round GetRound(int round)
    {
        if (rounds.Count > round) return rounds[round];
        return null;
    }
}
