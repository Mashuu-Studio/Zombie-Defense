using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using System.IO;

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
        LoadSetting();
    }

    #region Audio
    [SerializeField] private AudioMixer mixer;
    public enum SoundType { MASTER = 0, BGM, SFX, }

    public static string MASTER_MIXER = "Master";
    public static string BGM_MIXER = "BGM";
    public static string SFX_MIXER = "SFX";

    public AudioMixerGroup GetMixerGroup(string name)
    {
        return mixer.FindMatchingGroups(name)[0];
    }

    public void AdjustVolume(SoundType type, float volume)
    {
        setting.options["volume"][(int)type] = (int)volume;
        volume = volume * volume / 10000f;
        volume = Mathf.Log10(volume) * 20f;

        switch (type)
        {
            case SoundType.MASTER: mixer.SetFloat(MASTER_MIXER, volume); break;
            case SoundType.BGM: mixer.SetFloat(BGM_MIXER, volume); break;
            case SoundType.SFX: mixer.SetFloat(SFX_MIXER, volume); break;
        }

        SaveSetting();
    }
    #endregion

    #region Graphic
    public List<Resolution> Resolutions { get { return resolutions; } }
    public int CurrentResolutionIndex { get { return currentResolutionIndex; } }
    private List<Resolution> resolutions;
    private int currentResolutionIndex;
    private FullScreenMode screenMode;

    Vector2Int currentMainDisplayResolution;

    private void Update()
    {
        if (currentMainDisplayResolution.x != Screen.mainWindowDisplayInfo.width
            || currentMainDisplayResolution.y != Screen.mainWindowDisplayInfo.height)
        {
            InitResolutions(true);
        }
    }

    private void InitResolutions(bool mainScreenChanged = false)
    {
        currentMainDisplayResolution = new Vector2Int(Screen.mainWindowDisplayInfo.width, Screen.mainWindowDisplayInfo.height);
        resolutions = new List<Resolution>();
        foreach (var resol in Screen.resolutions)
        {
            // 16 : 9 �鼭 resolutions�ȿ� ���� ��
            if (resol.width / 16 == resol.height / 9
                && resolutions.FindIndex(r => r.width == resol.width) == -1)
            {
                if (Screen.width == resol.width) currentResolutionIndex = resolutions.Count;
                resolutions.Add(resol);
            }
        }

        if (mainScreenChanged) UIController.Instance.LoadResolutionInfo();
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

    #region Data
    public Setting SettingInfo { get { return setting; } }
    private Setting setting;

    public class Setting
    {
        public Dictionary<string, int[]> options;

        public void Init()
        {
            options = new Dictionary<string, int[]>();
            options.Add("volume", new int[] { 100, 100, 100 });
        }

        public new string ToString()
        {
            string str = "";

            foreach (var key in options.Keys)
            {
                str += $"{key.ToLower()}=";
                int[] data = options[key];
                for (int i = 0; i < data.Length; i++)
                {
                    str += data[i].ToString();
                    if (i != data.Length - 1) str += ",";
                }
                str += "\n";
            }

            return str;
        }

        public void LoadData(string str)
        {
            Init();

            StringReader reader = new StringReader(str);
            string line;
            do
            {
                line = reader.ReadLine();

                if (line != null)
                {
                    string[] s = line.Split("=");
                    // key, data�� ���е� ��쿡�� �ε�.
                    // ���� ������ �����͸� ��ġ���� 2���� �ޱ⶧���� ������.
                    if (s.Length >= 2)
                    {
                        string key = s[0];
                        if (options.ContainsKey(key))
                        {
                            string[] datastrings = s[1].Split(",");
                            for (int i = 0; i < datastrings.Length && i < options[key].Length; i++)
                            {
                                int data;
                                if (int.TryParse(datastrings[i], out data) == false) continue;
                                options[key][i] = data;
                            }
                        }
                    }
                }
                else break;
            }
            while (line != null);
        }
    }

    public void SaveSetting()
    {
        string path = Path.Combine(Application.persistentDataPath, "setting.ini");
        string text = setting.ToString();

        File.WriteAllText(path, text);
    }

    public void LoadSetting()
    {
        string path = Path.Combine(Application.persistentDataPath, "setting.ini");

        string data = "";

        if (File.Exists(path)) data = File.ReadAllText(path);
        setting = new Setting();
        setting.LoadData(data);
    }
    #endregion
}