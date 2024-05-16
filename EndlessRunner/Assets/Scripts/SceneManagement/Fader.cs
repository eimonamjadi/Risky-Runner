using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    CanvasGroup canvasGroup;
    Coroutine activeFade = null;
    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeOutImmediate()
    {
        canvasGroup.alpha = 1f;
    }

    public IEnumerator FadeOutIn()
    {
        yield return FadeOut(2f);
        yield return FadeIn(1f);
    }

    public IEnumerator FadeOut(float time)
    {
        yield return Fade(1f, time);
    }

    public IEnumerator FadeIn(float time)
    {
        yield return Fade(0f, time);
    }

    private IEnumerator Fade(float target, float time)
    {
        if (activeFade != null)
        {
            StopCoroutine(activeFade);
        }
        StartCoroutine(FadeRoutine(target, time));
        yield return activeFade;
    }

    private IEnumerator FadeRoutine(float target, float time)
    {
        while (!Mathf.Approximately(canvasGroup.alpha, target))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, target, Time.deltaTime / time);
            yield return null;
        }
    }
}
