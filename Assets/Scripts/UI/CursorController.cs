using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public static CursorController Instance { get { return instance; } }
    private static CursorController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    [SerializeField] private Texture2D cursorTex;

    public void SetCursor(bool b)
    {
        var tex = b ? cursorTex : null;
        CursorMode mode = b ? CursorMode.ForceSoftware : CursorMode.Auto;
        Cursor.SetCursor(tex, Vector2.zero, mode);
    }
}
