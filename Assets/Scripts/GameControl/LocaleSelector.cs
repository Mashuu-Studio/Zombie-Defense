using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector: MonoBehaviour
{
    bool active;
    public void ChangeLanguage(int index)
    {
        if (!active) StartCoroutine(SetLanguage(index));
    }

    IEnumerator SetLanguage(int index)
    {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        active = false;
    }
}
