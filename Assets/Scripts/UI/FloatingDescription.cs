using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using TMPro;

public class FloatingDescription : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI statText;
    [SerializeField] private LocalizeStringEvent nameLocalizeStringEvent;
    [SerializeField] private LocalizeStringEvent descLocalizeStringEvent;
    [SerializeField] private LocalizeStringEvent statLocalizeStringEvent;
    private const int width = 300;
    private RectTransform rect;
    private bool isWeapon;
    private void Awake()
    {
        rect = (RectTransform)transform;
        gameObject.SetActive(false);
    }

    public void SetDescription(Vector3 pos, string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            gameObject.SetActive(false);
            return;
        }

        nameLocalizeStringEvent.SetEntry(key);
        descLocalizeStringEvent.SetEntry(key);

        Weapon weapon = WeaponManager.GetWeapon(key);
        isWeapon = weapon != null && !weapon.consumable;
        // Weapon일 경우에는 스탯을 추가로 표기.
        if (isWeapon)
        {
            string dmg = weapon.bullets > 1 ? $"{weapon.dmg}x{weapon.bullets}" : $"{weapon.dmg}";
            string aspeed = $"{string.Format("{0:0.0}", 1 / weapon.adelay)}/s";
            string ammo = $"{weapon.ammo}";
            statLocalizeStringEvent.SetEntry("GAME.SHOP.STATDESCRIPTION");
            statLocalizeStringEvent.StringReference.Arguments = new object[] { dmg, aspeed, ammo };
            statText.gameObject.SetActive(true);
        }
        else statText.gameObject.SetActive(false);

        MoveDescription(pos);
        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public void SetSize()
    {
        /* wrapping을 풀어놓은 뒤 text content의 사이즈를 체크.
         * 최대 사이즈를 제한하여 그에 따라 height 추가 조정
         * 이 후 다시 wrapping하여 박스에 알맞게 들어가도록 함.
         */

        descText.enableWordWrapping = false;
        float descHeight = Mathf.CeilToInt(descText.preferredWidth / width) * descText.preferredHeight;
        // 만약 스탯을 표기하지 않아도 된다면 descText의 top bottom을 조정할 필요가 있음.
        // anchor가 0,0 / 1,1 일 때 (전체에서 줄이는 방식일 때)
        // anchoredposition은 max - min / 2 가 되고 
        // sizeDelta는 -(min + max)가 됨.
        // descText는 stat이 있을떄는 50, 150 이며 없을때는 50, 0이므로
        // (0, 50) , (0, -200) 과 (0, -25) , (0, -50)으로 구분할 수 있음.
        float top = 50;
        float bottom = isWeapon ? 150 : 0;

        descText.rectTransform.anchoredPosition = new Vector2(0, (bottom - top) / 2);
        descText.rectTransform.sizeDelta = new Vector2(0, -(top + bottom));

        rect.sizeDelta = new Vector2(width, 50 + descHeight + bottom);
        descText.enableWordWrapping = true;
    }

    public void MoveDescription(Vector3 pos)
    {
        /* 사이즈 조정이 끝난 후 Description이 밖으로 나갈 것 같다면 위치조정.
         * 우측으로 나간다면 pos의 x를 rect만큼 뺌.
         * 아래로 나간다면 pos의 y를 rect만큼 더함.
         */

        Vector2 size = new Vector2(
                pos.x + rect.sizeDelta.x,
                pos.y - rect.sizeDelta.y);

        if (size.x > Screen.currentResolution.width) pos.x -= rect.sizeDelta.x;
        if (size.y > Screen.currentResolution.height) pos.y -= rect.sizeDelta.y;

        rect.anchoredPosition = pos;
    }
}
