using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : Poolable
{
    [SerializeField] private ParticleSystem particle;

    private float originSize;

    private void Update()
    {
        if (!particle.isPlaying)
        {
            PoolController.Push(gameObject.name, this);
            var pmain = particle.main;
            pmain.startSize = new ParticleSystem.MinMaxCurve(originSize);
        }
    }

    public void Play(float angle, float ratio = -1)
    {
        var pmain = particle.main;
        originSize = pmain.startSize.constant;
        if (ratio > 0)
        {
            pmain.startSize = new ParticleSystem.MinMaxCurve(originSize * ratio);
        }
        pmain.startRotation = new ParticleSystem.MinMaxCurve(angle);
        SoundController.Instance.PlaySFX(transform, name, true);
        particle.Play();
    }
}
