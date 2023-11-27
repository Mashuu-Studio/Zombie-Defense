using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D player;
    [SerializeField] private Transform turretPointer;


    void Update()
    {
        float axisX = Input.GetAxis("Horizontal");
        float axisY = Input.GetAxis("Vertical");

        player.MovePosition(player.position + new Vector2(axisX, axisY) * Time.deltaTime * 15);

        turretPointer.position = TurretController.GetDirection(player.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));

        MapGenerator.Instance.UpdateMapPath(player.position);
        if (Input.GetKeyDown("q"))
        {
            TurretController.Instance.AddTurret(turretPointer.position, "Turret");
        }
    }

}
