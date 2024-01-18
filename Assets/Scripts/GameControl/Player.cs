using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagedObject
{
    public static Player Instance { get { return instance; } }
    private static Player instance;

    private int maxhp;
    private int hp;
    private int speed = 5;

    public int MaxHp { get { return maxhp; } }
    public int Hp { get { return hp; } }
    private Rigidbody2D rigidbody;

    [SerializeField] private BoxCollider2D autoTargetCollider;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        rigidbody = GetComponent<Rigidbody2D>();

        hp = maxhp = 3;
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
        rigidbody.position += new Vector2(axisX, axisY) * Time.fixedDeltaTime * speed;
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

    public List<Collider2D> DetectEnemyTargets(float range)
    {
        // 위치를 캐릭터 앞으로 세팅
        // 크기는 range로 세팅
        autoTargetCollider.offset = new Vector2(range / 2 + .5f, 0);
        autoTargetCollider.size = Vector2.one * range;

        List<Collider2D> cols = new List<Collider2D>();
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = 1 << LayerMask.NameToLayer("Enemy");
        // 전방 사각형 모양
        autoTargetCollider.OverlapCollider(filter, cols);
        return cols;
    }
}
