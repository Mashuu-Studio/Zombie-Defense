using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    public enum TutorialStep { LIBRARY = 0, GAME, SHOP, BUILDMODE, }
    [SerializeField] private TutorialView[] tutorialViews;
    public static bool IsTutorial { get { return isTutorial; } }
    private static bool isTutorial;

    public void Init()
    {
        isTutorial = false;
        foreach (var view in tutorialViews)
        {
            view.Init();
            view.SetActive(false);
        }
    }

    public static void SetTutorial(bool b)
    {
        isTutorial = b;
    }

    public void ShowTutorial(TutorialStep step)
    {
        int index = (int)step;
        if (GameSetting.Instance.SettingInfo.readTutorial[index]) return;

        tutorialViews[index].SetActive(true);
        GameSetting.Instance.SettingInfo.readTutorial[index] = true;
        GameSetting.Instance.SaveSetting();
        UIController.Instance.SetTutorialProgress();
    }
}
