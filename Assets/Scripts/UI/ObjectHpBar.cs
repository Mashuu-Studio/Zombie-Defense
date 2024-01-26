using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectHpBar : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    private Transform target;

    private void Start()
    {
        target = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;}

    public void UpdateHpBar(int hp)
    {
        hpBar.value = hp;
    }

    public void SetHpBar(int hp, Vector2 size)
    {
        hpBar.value = hpBar.maxValue = hp;
        ((RectTransform)hpBar.transform).sizeDelta = new Vector2(size.x * 3 / 2, 0.25f);
        ((RectTransform)hpBar.transform).anchoredPosition = new Vector2(0, size.y * 3 / 4);
    }
}
