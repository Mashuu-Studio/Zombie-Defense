using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RoundManager
{
    public static List <Round> Rounds { get { return rounds; } }
    private static List<Round> rounds;

    public static void Init()
    {
        rounds = new List<Round>()
        {
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "test_summon", 2.5f },
                    //{ "test2", 4.5f },
                    //{ "test3", 2.5f },
                },
            },/*
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "test1", 1.5f },
                    { "test_buff1", 4.5f },
                    { "test_inv", 2.5f },
                },
            },
            new Round()
            {
                enemyInfo = new Dictionary<string, float>()
                {
                    { "test_flight", 1.5f },
                    { "test_buff2", 4.5f },
                    { "test_invflight", 2.5f },
                },
            },*/
        };
    }

    public static Round GetRound(int round)
    {
        if (rounds.Count > round) return rounds[round];
        return null;
    }
}
