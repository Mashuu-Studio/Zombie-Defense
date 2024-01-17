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
        bgmSource = GetComponent<AudioSource>();
        sfxPool = GetComponent<Pool>();

        var go = new GameObject("SFX");
        var sfxSource = go.AddComponent<SfxSource>();

        sfxPool.Init(sfxSource);
    }

    [SerializeField] private List<SfxInfo> sfxInfos;
    private Dictionary<string, AudioClip> sfxes;

    private AudioSource bgmSource;
    private List<SfxSource> sfxSources;
    private Pool sfxPool;

    private float bgmVol;
    private float sfxVol;

    private void Start()
    {
        sfxSources = new List<SfxSource>();
        bgmSource.volume = bgmVol = 1;
        sfxVol = 1;

        sfxes = new Dictionary<string, AudioClip>();
        sfxInfos.ForEach(info => sfxes.Add(info.name.ToUpper(), info.clip));
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < sfxSources.Count; i++)
        {
            var source = sfxSources[i];
            if (source.isPlaying) source.SetVolume(sfxVol);
            else
            {
                sfxSources.Remove(source);
                sfxPool.Push(source);
                i--;
            }
        }
    }

    public void PlaySFX(GameObject obj, string name)
    {
        var source = (SfxSource)sfxPool.Pop();
        sfxSources.Add(source);
        source.SetVolume(sfxVol);
        name = name.ToUpper();
        if (sfxes.ContainsKey(name)) source.PlaySfx(sfxes[name]);
    }


    [System.Serializable]
    public struct SfxInfo
    {
        public string name;
        public AudioClip clip;
    }
}
