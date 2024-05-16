using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour
{
    protected int countDown = 5;
    protected Text text;
    
    public void StartCountDown()
    {
        text = GetComponent<Text>();
        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        while(countDown > 0)
        {
            yield return new WaitForSeconds(1f);
            countDown--;
            text.text = countDown.ToString();
        }
        if (Loader.Instance.LoadingFixedSpeedLevel)
        {
            Loader.Instance.LoadNewFixedSpeedLevel();
        }
        else
        {
            Loader.Instance.LoadSelfPacedLevel();
        }
    }
}
