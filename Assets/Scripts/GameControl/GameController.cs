using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private Transform turretPointer;

    private void Awake()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Map Boundary"));
    }

    void Update()
    {
        float axisX = Input.GetAxis("Horizontal");
        float axisY = Input.GetAxis("Vertical");

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        player.MovePosition(player.position + new Vector2(axisX, axisY) * Time.deltaTime * 15);
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
