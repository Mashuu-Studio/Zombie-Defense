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

        AudioClip[] arr = Resources.LoadAll<AudioClip>("Sounds/SFX");
        foreach (var clip in arr)
        {
            sfxes.Add(clip.name.ToUpper(), clip);
        }
    }

    public bool ContainsSFX(string name)
    {
        return sfxes.ContainsKey(name);
    }

    public void PlaySFX(Transform source, string name, bool separate = false)
    {
        var sfx = (SfxSource)sfxPool.Pop();
        if (separate) sfx.transform.position = source.position;
        else
        {
            sfx.transform.SetParent(source);
            sfx.transform.localPosition = Vector3.zero;
        }
        sfxSources.Add(sfx);
        name = name.ToUpper();
        if (sfxes.ContainsKey(name)) sfx.PlaySfx(sfxes[name]);
    }

    public void Push(SfxSource sfx)
    {
        sfxSources.Remove(sfx);
        sfxPool.Push(sfx);
    }
}
