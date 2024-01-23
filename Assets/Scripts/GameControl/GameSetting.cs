using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GameSetting : MonoBehaviour
{
    public static GameSetting Instance { get { return instance; } }
    private static GameSetting instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        Application.targetFrameRate = 60;
        InitResolutions();
    }

    #region Audio
    [SerializeField] private AudioMixer mixer;
    public static string MASTER_MIXER = "Master";
    public static string BGM_MIXER = "BGM";
    public static string SFX_MIXER = "SFX";

    public AudioMixerGroup GetMixerGroup(string name)
    {
        return mixer.FindMatchingGroups(name)[0];
    }

    public void AdjustMasterVolume(float volume)
    {
        mixer.SetFloat(MASTER_MIXER, volume);
    }

    public void AdjustBGMVolume(float volume)
    {
        mixer.SetFloat(BGM_MIXER, volume);
    }

    public void AdjustSFXVolume(float volume)
    {
        mixer.SetFloat(SFX_MIXER, volume);
    }
    #endregion

    #region Graphic
    public List<Resolution> Resols { get { return resolutions; } }
    public int resolIndex { get { return currentResolutionIndex; } }
    private List<Resolution> resolutions;
    private int currentResolutionIndex;
    private FullScreenMode screenMode;

    private void InitResolutions()
    {
        Debug.Log($"{Screen.mainWindowDisplayInfo.width} x {Screen.mainWindowDisplayInfo.height}");
        resolutions = new List<Resolution>();
        foreach (var resol in Screen.resolutions)
        {
            // 16 : 9 면서 resolutions안에 없을 때
            if (resol.width / 16 == resol.height / 9 
                && resolutions.FindIndex(r => r.width == resol.width) == -1)
            {
                resolutions.Add(resol);
            }
        }
    }

    public void ChangeResolution(int index, bool fullscreen)
    {
        FullScreenMode screen = fullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        if (!(currentResolutionIndex == index && screenMode == screen))
        {
            currentResolutionIndex = index;
            screenMode = screen;
            int width = fullscreen ? Screen.mainWindowDisplayInfo.width : resolutions[currentResolutionIndex].width;
            int height = fullscreen ? Screen.mainWindowDisplayInfo.height : resolutions[currentResolutionIndex].height;
            Screen.SetResolution(width, height, screenMode);
        }
    }
    #endregion
}
