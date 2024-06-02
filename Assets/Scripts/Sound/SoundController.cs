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

        sfxPool = GetComponent<Pool>();

        var go = new GameObject("SFX");
        var sfxSource = go.AddComponent<SfxSource>();
        go.transform.SetParent(transform);

        sfxPool.Init(sfxSource);

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

    public void PlaySFX(Vector2 pos, string name)
    {
        var source = (SfxSource)sfxPool.Pop();
        source.transform.position = pos;
        sfxSources.Add(source);
        name = name.ToUpper();
        if (sfxes.ContainsKey(name)) source.PlaySfx(sfxes[name]);
    }

    public void Push(SfxSource sfx)
    {
        sfxSources.Remove(sfx);
        sfxPool.Push(sfx);
    }
}
