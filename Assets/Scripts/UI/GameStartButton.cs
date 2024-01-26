using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStartButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        GameController.Instance.GoTo(SceneController.Scene.GAME);
    }
}
