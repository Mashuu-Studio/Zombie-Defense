using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundStartButton : ScriptableButton
{
    protected override void ClickEvent()
    {
        GameController.Instance.StartRound();
    }
}
