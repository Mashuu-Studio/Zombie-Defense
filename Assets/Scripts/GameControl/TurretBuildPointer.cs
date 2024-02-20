using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(SpriteRenderer))]
public class TurretBuildPointer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private static Color buildableColor = new Color(0, 1, 0, .7f);
    private static Color notbuildableColor = new Color(1, 0, 0, .7f);

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ChangeSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    public void SetColor(bool buildable)
    {
        spriteRenderer.color = buildable ? buildableColor : notbuildableColor;
    }
}
