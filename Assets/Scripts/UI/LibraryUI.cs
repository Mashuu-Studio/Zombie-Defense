using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;
using TMPro;

public class LibraryUI : MonoBehaviour
{
    [SerializeField] private Transform itemsParent;
    [SerializeField] private LibraryItem itemPrefab;
    [SerializeField] private GameObject desc;
    [Space]
    [SerializeField] private Image image;
    [SerializeField] private LocalizeStringEvent nameString;
    [SerializeField] private LocalizeStringEvent descString;
    [Space]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [Space]
    [SerializeField] private TextMeshProUGUI dmgText;
    [SerializeField] private TextMeshProUGUI aspeedText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [Space]
    [SerializeField] private TextMeshProUGUI bulletResText;
    [SerializeField] private TextMeshProUGUI explosResText;
    [SerializeField] private TextMeshProUGUI fireResText;
    [SerializeField] private TextMeshProUGUI elecResText;
    private List<LibraryItem> items = new List<LibraryItem>();

    public void Init()
    {
        itemPrefab.gameObject.SetActive(false);
        foreach (var enemy in EnemyManager.Enemies)
        {
            var item = Instantiate(itemPrefab, itemsParent);
            item.Init(enemy.key);
            item.gameObject.SetActive(true);
            items.Add(item);
        }
        SetActive(false);
    }

    public void SetActive(bool b)
    {
        gameObject.SetActive(b);
        desc.SetActive(false);
    }

    public void UpdateDescription(string key)
    {
        var data = EnemyManager.GetEnemy(key);
        desc.SetActive(data != null);
        if (data == null) return;

        image.sprite = SpriteManager.GetSprite(key);
        nameString.SetEntry(key);
        descString.SetEntry(key);

        hpText.text = data.hp.ToString();
        speedText.text = data.speed.ToString();
        rewardText.text = data.money.ToString();

        dmgText.text = data.dmg.ToString();
        aspeedText.text = string.Format("{0:0.0}/s", 1 / data.adelay);
        rangeText.text = data.range.ToString();

        bulletResText.text = explosResText.text = fireResText.text = elecResText.text = "100%";

        foreach (var res in data.resistances)
        {
            string val = ((int)(res.Value * 100)).ToString() + "%";
            switch(res.Key)
            {
                case ObjectData.Attribute.BULLET: bulletResText.text = val; break;
                case ObjectData.Attribute.EXPLOSION: explosResText.text = val; break;
                case ObjectData.Attribute.FIRE: fireResText.text = val; break;
                case ObjectData.Attribute.ELECTRIC: elecResText.text = val; break;
            }
        }
    }
}
