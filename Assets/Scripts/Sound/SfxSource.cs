using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SfxSource : Poolable
{
    private AudioSource source;
    public bool isPlaying { get { return source.isPlaying; } }
    public override void Init()
    {
        source = GetComponent<AudioSource>();
        source.volume = 1;
        source.outputAudioMixerGroup = GameSetting.Instance.GetMixerGroup(GameSetting.SFX_MIXER);
    }
    
    private void Update()
    {
        if (gameObject.activeSelf && !source.isPlaying) SoundController.Instance.Push(this);
    }
    
    public void PlaySfx(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }
    
    private void OnEnable()
    {
        if (source != null) source.Stop();
    }
}
