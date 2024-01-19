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
    }

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
}
