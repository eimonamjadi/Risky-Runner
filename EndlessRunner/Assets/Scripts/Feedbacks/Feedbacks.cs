using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Feedbacks : MonoBehaviour
{
    protected Feedback[] _feedbacks;

    private void OnEnable()
    {
        _feedbacks = GetComponentsInChildren<Feedback>();
    }

    private void Start()
    {
        foreach (Feedback f in _feedbacks)
        {
            f.Initialization();
        }
    }

    public void PlayFeedbacks()
    {
        foreach(Feedback feedback in _feedbacks)
        {
            feedback.PlayFeedback();
        }
    }

    public void StopFeedbacks()
    {
        foreach(Feedback feedback in _feedbacks)
        {
            feedback.StopFeedback();
        }
    }

    public bool IsPlaying()
    {
        foreach (Feedback f in _feedbacks)
        {
            if (f.IsPlaying()) return true;
        }
        return false;
    }

}
