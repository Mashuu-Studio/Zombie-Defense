using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingUI : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private TextMeshProUGUI bgmVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    public void Init()
    {
        masterVolumeSlider.minValue = bgmVolumeSlider.minValue = sfxVolumeSlider.minValue = 1;
        masterVolumeSlider.maxValue = bgmVolumeSlider.maxValue = sfxVolumeSlider.maxValue = 100;
        masterVolumeSlider.wholeNumbers = bgmVolumeSlider.wholeNumbers = sfxVolumeSlider.wholeNumbers = true;

        masterVolumeSlider.onValueChanged.AddListener(v =>
        {
            masterVolumeText.text = v.ToString();
            v = v * v / 10000f;
            v = Mathf.Log10(v) * 20f;
            GameSetting.Instance.AdjustMasterVolume(v);
        });

        bgmVolumeSlider.onValueChanged.AddListener(v =>
        {
            bgmVolumeText.text = v.ToString();
            v = v * v / 10000f;
            v = Mathf.Log10(v) * 20f;
            GameSetting.Instance.AdjustBGMVolume(v);
        });

        sfxVolumeSlider.onValueChanged.AddListener(v =>
        {
            sfxVolumeText.text = v.ToString();
            v = v * v / 10000f;
            v = Mathf.Log10(v) * 20f;
            GameSetting.Instance.AdjustSFXVolume(v);
        });

        masterVolumeSlider.value = bgmVolumeSlider.value = sfxVolumeSlider.value = 100;
    }
}
