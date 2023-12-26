using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform turretPointer;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Map Boundary"));
    }

    void Update()
    {        
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        turretPointer.position = TurretController.GetDirection(player.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        MapGenerator.Instance.UpdateMapPath(player.position);

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 dir = mouseWorldPos - player.transform.position;
            ((Bullet)PoolController.Pop("Bullet")).SetBullet(player.transform.position, dir, WeaponController.Instance.CurWeapon.dmg, WeaponController.Instance.CurWeapon.speed);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TurretController.Instance.AddTurret(turretPointer.position, "Barricade");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TurretController.Instance.AddTurret(turretPointer.position, "Turret");
        }

    }

}
