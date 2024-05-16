using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StumbleCount : MonoBehaviour
{
    public static int Count = 0;
    protected Text textField;
    protected ScoreDisplay score;

    private void OnEnable()
    {
        if (FindObjectOfType<PlayerController>())
        {
            PlayerController controller = FindObjectOfType<PlayerController>();
            controller.OnStumble += CountIncrement;
        }
        else if (FindObjectOfType<NewPlayerController>())
        {
            NewPlayerController controller = FindObjectOfType<NewPlayerController>();
            controller.OnStumble += CountIncrement;
        }
    }

    private void OnDisable()
    {
        if (FindObjectOfType<PlayerController>())
        {
            PlayerController controller = FindObjectOfType<PlayerController>();
            controller.OnStumble -= CountIncrement;
        }
        else if (FindObjectOfType<NewPlayerController>())
        {
            NewPlayerController controller = FindObjectOfType<NewPlayerController>();
            controller.OnStumble -= CountIncrement;
        }
    }

    private void Awake()
    {
        textField = GetComponent<Text>();
        score = FindObjectOfType<ScoreDisplay>();
    }

    public void CountIncrement()
    {
        Count++;
        if (textField)
        {
            textField.text = Count.ToString();
            score.SetTextPenalizedColor();
        }
    }
}
