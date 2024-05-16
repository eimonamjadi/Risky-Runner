using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Feedback : MonoBehaviour
{
    [HideInInspector] public bool bIsPlaying = false;
    [Header("Timing")]
    public Timing _timing;
    protected Coroutine playFeedbackCoroutine = null;

    public virtual void Initialization()
    {
        bIsPlaying = false;
        _timing.RepeatCount = 0;
    }

    public void PlayFeedback()
    {
        bIsPlaying = true;
        _timing.RepeatCount = 0;
        StartCoroutine(PlayInitialDelay());
    }

    protected virtual void ReadyToPlayFeedback()
    {
        _timing.RepeatCount++;
    }

    public virtual void StopFeedback()
    {
        bIsPlaying = false;
    }

    public bool IsPlaying()
    {
        if (!bIsPlaying) return false;
        else return !IsCurrentTaskFinished();
    }

    protected abstract bool IsCurrentTaskFinished();

    protected IEnumerator PlayInitialDelay()
    {
        yield return new WaitForSeconds(_timing.InitialDelay);
        ReadyToPlayFeedback();
    }

    protected IEnumerator PlayRepeatInterval()
    {
        yield return new WaitForSeconds(_timing.RepeatDelay);
        ReadyToPlayFeedback();
        playFeedbackCoroutine = null;
    }

    protected virtual void Update()
    {
        if (playFeedbackCoroutine == null && bIsPlaying && IsCurrentTaskFinished())
        {
            if (!_timing.RepeatForever && (_timing.NumRepeat == 0 || _timing.RepeatCount == _timing.NumRepeat)) StopFeedback();
            else playFeedbackCoroutine = StartCoroutine(PlayRepeatInterval());
        }
    }
}

[System.Serializable]
public class Timing
{
    [Min(0f)] public float InitialDelay = 0f;
    public bool RepeatForever = false;
    [Min(0)] public int NumRepeat = 0;
    [Min(0f)] public float RepeatDelay = 0f;
    [HideInInspector] public int RepeatCount = 0;
}