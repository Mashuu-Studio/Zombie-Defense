using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionPatrolIcon : BuildModeItemIcon
{
    public override void Select()
    {
        UIController.Instance.SelectCompanion(this);
    }
}
