using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CompanionManager
{
    public static List<Companion> Companions { get { return companions; } }
    private static List<Companion> companions;

    public static void Init()
    {
        companions = new List<Companion>()
        {
            new Companion()
            {
                key = "COMPANION.COMPANION1",
                hp = 5,
                def = 0,
            },
            new Companion()
            {
                key = "COMPANION.COMPANION2",
                hp = 10,
                def = 0,
            },
            new Companion()
            {
                key = "COMPANION.COMPANION3",
                hp = 15,
                def = 1,
            },
        };
    }

    public static Companion GetCompanion(string key)
    {
        foreach (var companion in companions)
        {
            if (key == companion.key) return companion;
        }
        return null;
    }
}
