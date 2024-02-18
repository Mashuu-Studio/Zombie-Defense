using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DescriptionIcon : MonoBehaviour
{
    private string key;

    private Image image;
    private bool isOn;

    private Vector3 mousePos;
    private bool contains;

    public void SetIcon(string key)
    {
        if (image == null)
        {
            image = GetComponent<Image>();
            isOn = false;
        }

        this.key = key;
    }

    void Update()
    {
        // Off되어있을 때에는 마우스의 위치가 Description Icon과 겹치는지 체크.
        // On되어있을때는 마우스가 Icon에서 나갔는지를 체크
        mousePos = Input.mousePosition;
        contains = UIController.PointOverUI(gameObject);

        if (isOn && !contains)
        {
            UIController.Instance.SetDescription(mousePos, string.Empty);
            isOn = false;
        }
    }

    private void LateUpdate()
    {
        if (contains)
        {
            if (isOn == false)
            {
                UIController.Instance.SetDescription(mousePos, key);
                isOn = true;
            }
            else
            {
                UIController.Instance.MoveDescription(mousePos);
            }
        }
    }
}