using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectHpBar : MonoBehaviour
{
    [SerializeField] private Slider hpBar;
    private IDamagedObject target;
    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.identity;
        hpBar.value = target.Hp;
    }

    public void SetHpBar(IDamagedObject target, int hp)
    {
        this.target = target;
        hpBar.value = hpBar.maxValue = hp;
    }
}
