using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountedWeapon : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private bool detectTarget;

    private void Update()
    {
        // Ž���� Ÿ���� ���ٸ� ȸ��
        // 1�����ӿ� 1.5��ŭ ȸ�� (4�ʿ� �� ����)
        if (detectTarget == false)
        {
            transform.localEulerAngles = new Vector3(0, 0, transform.localEulerAngles.z + 1.5f);
        }
    }

    public void Mount(float weapon)
    {
        animator.SetFloat("weapon", weapon);
        Debug.Log(weapon);
    }

    public void ChangeColor(Color c)
    {
        spriteRenderer.color = c;
    }

    public void Detect(bool b)
    {
        detectTarget = b;
    }

    public void Fire(Vector3 target)
    {
        animator.SetTrigger("fire");
        LookAt(target);
    }

    private void LookAt(Vector3 target)
    {
        Vector2 dir = target - transform.position;
        float degree = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
        transform.localEulerAngles = new Vector3(0, 0, degree + 90);
    }
}
