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
    }

    public void PlaySfx(AudioClip clip)
    {
        source.clip = clip;
        source.Play();
    }

    public void SetVolume(float vol)
    {
        source.volume = vol;
    }

    private void OnEnable()
    {
        if (source != null) source.Stop();
    }
}
