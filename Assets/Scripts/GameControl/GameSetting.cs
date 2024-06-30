using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Localization.Settings;
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
    #region Data
    public Setting SettingInfo { get { return setting; } }
    private Setting setting;

    public static int CurrentLanguage { get { return Instance.setting.language; } }

    private bool changingLang;
    public void ChangeLanguage(int index)
    {
        if (!changingLang) StartCoroutine(SetLanguage(index));
    }

    IEnumerator SetLanguage(int index)
    {
        changingLang = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        setting.language = index;
        SaveSetting();
        changingLang = false;
    }

    public class Setting
    {
        public int language;
        public bool[] readTutorial;
        public Dictionary<string, int[]> options;
        public int[] Volume { get { return options["volume"]; } }

        public int TutorialProgress
        {
            get
            {
                int progress = 0;
                for (int i = 0; i < readTutorial.Length; i++)
                    progress += (readTutorial[i] ? 1 : 0) * (int)Mathf.Pow(10, i);

                return progress;
            }
        }

        public void Init()
        {
            language = 0;
            readTutorial = new bool[4];
            options = new Dictionary<string, int[]>();
            options.Add("volume", new int[] { 50, 100, 100 });
        }

#if UNITY_WEBGL

        public void LoadData(int lang, int[] volumes, int tutorialProgress)
        {
            Init();

            language = lang;
            Instance.ChangeLanguage(language);

            for (int i = 0; i < 3; i++)
                options["volume"][i] = volumes[i];

            for (int i = 0; i < readTutorial.Length; i++)
            {
                readTutorial[i] = tutorialProgress % 10 == 1;
                tutorialProgress /= 10;
            }
        }
    }

    public void SaveSetting()
    {
        PlayerPrefs.SetInt("LANGUAGE", setting.language);

        PlayerPrefs.SetInt("MASTER", setting.Volume[0]);
        PlayerPrefs.SetInt("BGM", setting.Volume[1]);
        PlayerPrefs.SetInt("SFX", setting.Volume[2]);

        PlayerPrefs.SetInt("TUTORIAL", setting.TutorialProgress);

        PlayerPrefs.Save();
    }

    public void LoadSetting()
    {
        int[] volumes = new int[3]
        {
            PlayerPrefs.GetInt("MASTER", 50),
            PlayerPrefs.GetInt("BGM", 100),
            PlayerPrefs.GetInt("SFX", 100)
        };

        int tutorialProgress = PlayerPrefs.GetInt("TUTORIAL", 0000);

        int lang = PlayerPrefs.GetInt("LANGUAGE", 0);
        setting = new Setting();
        setting.LoadData(lang, volumes, tutorialProgress);
    }
#else
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
                    // key, data로 구분된 경우에만 로드.
                    // 설령 유저가 데이터를 고치더라도 2개만 받기때문에 괜찮음.
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
#endif
    #endregion
}
