using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagedObject
{
    private int hp;
    public int Hp { get { return hp; } }
    private Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    float axisX;
    float axisY;

    private void Update()
    {
        axisX = Input.GetAxis("Horizontal");
        axisY = Input.GetAxis("Vertical");

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        LookAt(mouseWorldPos);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rigidbody.position += new Vector2(axisX, axisY) * Time.fixedDeltaTime * 5;
        CameraController.Instance.MoveCamera(rigidbody.position);
    }

    public void LookAt(Vector3 target)
    {
        Vector2 dir = target - transform.position;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        transform.rotation = Quaternion.Euler(0, 0, degree);
    }

    public void Damaged(int dmg)
    {
    }
}
