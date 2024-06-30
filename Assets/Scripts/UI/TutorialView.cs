using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TutorialView : MonoBehaviour
{
    [SerializeField] private TutorialImage[] imageWithLangs;
    [SerializeField] private GameObject[] steps;
    private int index;

    public void Init()
    {
        GetComponent<Button>().onClick.AddListener(() => Step());
    }

    public void SetActive(bool b)
    {
        TutorialUI.SetTutorial(b);
        gameObject.SetActive(b);
        if (b)
        {
            index = 0;
            for (int lang = 0; lang < imageWithLangs.Length; lang++)
            {
                for (int i = 0; i < imageWithLangs[lang].imageSteps.Length; i++)
                    imageWithLangs[lang].imageSteps[i].SetActive(false);
            }
            for (int i = 0; i < steps.Length; i++)
                steps[i].SetActive(false);
            Step();
        }
    }

    public void Step()
    {
        // ÀÌÀü ½ºÅÜÀ» ²¨ÁÜ.
        if (index > 0)
        {
            imageWithLangs[GameSetting.CurrentLanguage].imageSteps[index - 1].SetActive(false);
            steps[index - 1].SetActive(false);
        }
        // ½ºÅÜÀÌ ³²¾Ò´Ù¸é ¿ÀÇÂ, ¾ø´Ù¸é Á¾·á.
        if (index < steps.Length)
        {
            imageWithLangs[GameSetting.CurrentLanguage].imageSteps[index].SetActive(true);
            steps[index++].SetActive(true);
        }
        else SetActive(false);
    }
}

[System.Serializable]
public class TutorialImage
{
    public GameObject[] imageSteps;
}
