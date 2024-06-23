using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class LibraryItem : MonoBehaviour
{
    [SerializeField] private Image image;
    private string key;
    public void Init(string key)
    {
        this.key = key;
        image.sprite = SpriteManager.GetSprite(key);
        GetComponent<Button>().onClick.AddListener(() => UIController.Instance.UpdateLibraryDescription(this.key));
    }
}
