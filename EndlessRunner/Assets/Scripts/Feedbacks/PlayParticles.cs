using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[AddComponentMenu("Feedbacks / PlayParticles")]
public class PlayParticles : Feedback
{
    public ParticleSystem Particle;

    public override void Initialization()
    {
        base.Initialization();
    }

    public override void StopFeedback()
    {
        base.StopFeedback();
    }


    protected override bool IsCurrentTaskFinished()
    {
        if (Particle == null) return false;
        return Particle.isPlaying;
    }

    protected override void ReadyToPlayFeedback()
    {
        base.ReadyToPlayFeedback();
        if (Particle) Particle.Play();
    }
}
