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

    [Space]
    [SerializeField] private TMP_Dropdown resolDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    public void Init()
    {
        masterVolumeSlider.minValue = bgmVolumeSlider.minValue = sfxVolumeSlider.minValue = 1;
        masterVolumeSlider.maxValue = bgmVolumeSlider.maxValue = sfxVolumeSlider.maxValue = 100;
        masterVolumeSlider.wholeNumbers = bgmVolumeSlider.wholeNumbers = sfxVolumeSlider.wholeNumbers = true;

        masterVolumeSlider.onValueChanged.AddListener(v =>
        {
            masterVolumeText.text = v.ToString();
            GameSetting.Instance.AdjustVolume(GameSetting.SoundType.MASTER, v);
        });

        bgmVolumeSlider.onValueChanged.AddListener(v =>
        {
            bgmVolumeText.text = v.ToString();
            GameSetting.Instance.AdjustVolume(GameSetting.SoundType.BGM, v);
        });

        sfxVolumeSlider.onValueChanged.AddListener(v =>
        {
            sfxVolumeText.text = v.ToString();
            GameSetting.Instance.AdjustVolume(GameSetting.SoundType.SFX, v);
        });

        masterVolumeSlider.value = GameSetting.Instance.SettingInfo.options["volume"][0];
        bgmVolumeSlider.value = GameSetting.Instance.SettingInfo.options["volume"][1];
        sfxVolumeSlider.value = GameSetting.Instance.SettingInfo.options["volume"][2];

        //LoadResolutionInfo();
    }

    public void LoadResolutionInfo()
    {
        resolDropdown.ClearOptions();
        List<string> resolList = new List<string>();
        GameSetting.Instance.Resolutions.ForEach(resol =>
        {
            resolList.Add($"{resol.width} x {resol.height}");
        });
        resolDropdown.AddOptions(resolList);
        resolDropdown.value = GameSetting.Instance.CurrentResolutionIndex;
        fullscreenToggle.isOn = Screen.fullScreenMode == FullScreenMode.FullScreenWindow;
    }

    public void ChangeResolution()
    {
        resolDropdown.interactable = !fullscreenToggle.isOn;
        //GameSetting.Instance.ChangeResolution(resolDropdown.value, fullscreenToggle.isOn);
    }
}
