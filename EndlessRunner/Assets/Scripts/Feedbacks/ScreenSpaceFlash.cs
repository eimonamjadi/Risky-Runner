using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Feedbacks/ScreenSpaceFlash")]
public class ScreenSpaceFlash : Feedback
{
    public Color TargetColor;
    protected Color CurrColor;
    protected Color StartColor;
    public float Duration;
    protected float Timer;

    public override void Initialization()
    {
        base.Initialization();
        StartColor = GUIManager.Current.FlashPanel.color = new Color(1, 1, 1, 0);
    }

    protected override void ReadyToPlayFeedback()
    {
        base.ReadyToPlayFeedback();
        StartCoroutine(PingpongOnce());
    }

    protected IEnumerator PingpongOnce()
    {
        Timer = 0f;
        do
        {
            Timer += Time.deltaTime;
            CurrColor = Color.Lerp(StartColor, TargetColor, Timer / (0.5f * Duration));
            GUIManager.Current.FlashPanel.color = CurrColor;
            yield return new WaitForEndOfFrame();
        } while (!CompareColor(CurrColor, TargetColor));
        Timer = 0f;
        do
        {
            Timer += Time.deltaTime;
            CurrColor = Color.Lerp(TargetColor, StartColor, Timer / (0.5f * Duration));
            GUIManager.Current.FlashPanel.color = CurrColor;
            yield return new WaitForEndOfFrame();
        } while (!CompareColor(CurrColor, TargetColor));
    }

    public override void StopFeedback()
    {
        base.StopFeedback();
        StopCoroutine(PingpongOnce());
        GUIManager.Current.FlashPanel.color = StartColor;
    }

    protected override bool IsCurrentTaskFinished()
    {
        return CurrColor == StartColor;
    }

    protected bool CompareColor(Color c1, Color c2)
    {
        if (Mathf.RoundToInt(c1.r * 1000.0f) != Mathf.RoundToInt(c2.r * 1000.0f)) return false;
        if (Mathf.RoundToInt(c1.g * 1000.0f) != Mathf.RoundToInt(c2.g * 1000.0f)) return false;
        if (Mathf.RoundToInt(c1.b * 1000.0f) != Mathf.RoundToInt(c2.b * 1000.0f)) return false;
        if (Mathf.RoundToInt(c1.a * 1000.0f) != Mathf.RoundToInt(c2.a * 1000.0f)) return false;
        return true;
    }
}
