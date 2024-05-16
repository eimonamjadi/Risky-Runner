using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : MMSingleton<SpeedSlider>
{
    Slider _slider;
    float TargetVal;
    Coroutine sliderCoroutine;
    public static bool CanChangeSpeed = true;

    protected override void Awake()
    {
        base.Awake();
        _slider = GetComponent<Slider>();
    }

    public void SetTargetValue(float val)
    {
        TargetVal = (val - Loader.MinSpeed) * 1.0f / (Loader.MaxSpeed - Loader.MinSpeed);
        _slider.value = TargetVal;
    }

    public void UpdateSliderValue(float targetSpeed)
    {
        float StartVal = TargetVal;
        CanChangeSpeed = false;
        if (sliderCoroutine != null)
        {
            StopCoroutine(sliderCoroutine);
            StartVal = _slider.value;
        }
        TargetVal = (targetSpeed - Loader.MinSpeed) / (Loader.MaxSpeed - Loader.MinSpeed);
        sliderCoroutine = StartCoroutine(AdjustSliderValue(StartVal, TargetVal));
    }

    protected IEnumerator AdjustSliderValue(float StartVal, float TargetVal)
    {
        float timer = 0f;
        while (!Mathf.Approximately(_slider.value, TargetVal))
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            /// the slider should reach the target value in 0.25 seconds
            _slider.value = Mathf.Lerp(StartVal, TargetVal, timer/0.25f);
        }
        sliderCoroutine = null;
        CanChangeSpeed = true;
    }

}
