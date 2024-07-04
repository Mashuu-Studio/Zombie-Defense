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
    private const int width = 500;
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
        // Weapon�� ��쿡�� ������ �߰��� ǥ��.
        if (isWeapon)
        {
            string dmg = weapon.bullets > 1 ? $"{weapon.dmg}x{weapon.bullets}" : weapon.dmg.ToString();
            string aspeed = string.Format("{0:0.0}", 1 / weapon.adelay) + "/s";
            string ammo = weapon.ammo.ToString();
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
        /* wrapping�� Ǯ����� �� text content�� ����� üũ.
         * �ִ� ����� �����Ͽ� �׿� ���� height �߰� ����
         * �� �� �ٽ� wrapping�Ͽ� �ڽ��� �˸°� ������ ��.
         */

        descText.enableWordWrapping = false;
        float descHeight = Mathf.CeilToInt(descText.preferredWidth / width) * descText.preferredHeight;
        // ���� ������ ǥ������ �ʾƵ� �ȴٸ� descText�� top bottom�� ������ �ʿ䰡 ����.
        // anchor�� 0,0 / 1,1 �� �� (��ü���� ���̴� ����� ��)
        // anchoredposition�� max - min / 2 �� �ǰ� 
        // sizeDelta�� -(min + max)�� ��.
        // descText�� stat�� �������� 50, 150 �̸� �������� 50, 0�̹Ƿ�
        // (0, 50) , (0, -200) �� (0, -25) , (0, -50)���� ������ �� ����.
        float top = 50;
        float bottom = isWeapon ? 150 : 0;

        descText.rectTransform.anchoredPosition = new Vector2(0, (bottom - top) / 2);
        descText.rectTransform.sizeDelta = new Vector2(0, -(top + bottom));

        rect.sizeDelta = new Vector2(width, 50 + descHeight + bottom);
        descText.enableWordWrapping = true;
    }

    public void MoveDescription(Vector3 pos)
    {
        /* ������ ������ ���� �� Description�� ������ ���� �� ���ٸ� ��ġ����.
         * �������� �����ٸ� pos�� x�� rect��ŭ ��.
         * �Ʒ��� �����ٸ� pos�� y�� rect��ŭ ����.
         */

        Vector2 size = new Vector2(
                pos.x + rect.sizeDelta.x,
                pos.y - rect.sizeDelta.y);

        if (size.x > Screen.currentResolution.width) pos.x -= rect.sizeDelta.x;
        if (size.y > Screen.currentResolution.height) pos.y -= rect.sizeDelta.y;

        rect.anchoredPosition = pos;
    }
}
