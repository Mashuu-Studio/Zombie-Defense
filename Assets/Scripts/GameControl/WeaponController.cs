using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public static WeaponController Instance { get { return instance; } }
    private static WeaponController instance;

    List<Weapon> weapons;
    int curIndex = 0;
    public Weapon CurWeapon { get { return weapons[curIndex]; } }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        weapons = new List<Weapon>()
        {
            new Weapon()
            {
                speed = 50,
                dmg = 1
            },
            new Weapon()
            {
                speed = 30,
                dmg = 2
            },
            new Weapon()
            {
                speed = 10,
                dmg = 3
            }
        };
    }

    private void Update()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0) curIndex--;
        if (scroll < 0) curIndex++;

        if (curIndex < 0) curIndex = weapons.Count - 1;
        if (curIndex >= weapons.Count) curIndex = 0;
    }
}
