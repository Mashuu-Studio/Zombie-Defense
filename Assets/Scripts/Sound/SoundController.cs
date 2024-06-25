using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Pool))]
public class SoundController : MonoBehaviour
{
    public static SoundController Instance { get { return instance; } }
    private static SoundController instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private SfxSource sfxSourcePrefab;
    private Dictionary<string, AudioClip> sfxes;

    private AudioSource bgmSource;
    private List<SfxSource> sfxSources;
    private Pool sfxPool;

    private const int EACHPLAYLIMIT = 4;
    private const int TOTALPLAYLIMIT = 32;
    private Dictionary<string, int> eachPlayingSfxAmount;
    private int totalPlayingSfxAmount;

    public void Init()
    {
        bgmSource = GetComponent<AudioSource>();
        bgmSource.outputAudioMixerGroup = GameSetting.Instance.GetMixerGroup(GameSetting.BGM_MIXER);
        bgmSource.clip = Resources.Load<AudioClip>("Sounds/BGM");
        bgmSource.volume = 1;
        bgmSource.loop = true;
        bgmSource.Play();

        sfxSourcePrefab.gameObject.SetActive(false);

        sfxPool = GetComponent<Pool>();
        sfxPool.Init(sfxSourcePrefab);

        sfxSources = new List<SfxSource>();
        sfxes = new Dictionary<string, AudioClip>();
        eachPlayingSfxAmount = new Dictionary<string, int>();
        totalPlayingSfxAmount = 0;

        AudioClip[] arr = Resources.LoadAll<AudioClip>("Sounds/SFX");
        foreach (var clip in arr)
        {
            string name = clip.name.ToUpper();
            sfxes.Add(name, clip);
            eachPlayingSfxAmount.Add(name, 0);
        }
    }

    public bool ContainsSFX(string name)
    {
        return sfxes.ContainsKey(name);
    }

    public void PlaySFX(Transform source, string name, bool separate = false)
    {
        name = name.ToUpper();
        // 한 번에 재생할 수 재생 수를 제한함.
        if (eachPlayingSfxAmount[name] >= EACHPLAYLIMIT
            || totalPlayingSfxAmount >= TOTALPLAYLIMIT) return;

        var sfx = (SfxSource)sfxPool.Pop();
        if (separate) sfx.transform.position = source.position;
        else
        {
            sfx.transform.SetParent(source);
            sfx.transform.localPosition = Vector3.zero;
        }
        sfxSources.Add(sfx);
        if (sfxes.ContainsKey(name))
        {
            sfx.PlaySfx(sfxes[name]);
            eachPlayingSfxAmount[name]++;
            totalPlayingSfxAmount++;
        }
    }

    public void Push(SfxSource sfx)
    {
        eachPlayingSfxAmount[sfx.clipName]--;
        totalPlayingSfxAmount--;
        sfxSources.Remove(sfx);
        sfxPool.Push(sfx);
    }
}
