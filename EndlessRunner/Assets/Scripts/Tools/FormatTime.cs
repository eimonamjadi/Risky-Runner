using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormatTime : MMSingleton<FormatTime>
{
    protected float minutes = 0f;
    protected float seconds = 0f;
    protected float timer = 0f;
    public float TimeElapsed { get { return timer; } }
    protected Text text;

    private void OnEnable()
    {
        PlayerManager.OnGameStarted += ResetTimer;
    }

    private void OnDisable()
    {
        PlayerManager.OnGameStarted -= ResetTimer;
    }

    private void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (PlayerManager.isGameStarted && !PlayerManager.isGamePaused && !PlayerManager.isGameOver)
        {
            timer += Time.deltaTime;
            minutes = Mathf.Floor(timer / 60);
            seconds = Mathf.RoundToInt(timer % 60);

            text.text = minutes.ToString("00") + " : " + seconds.ToString("00");
        }
    }

    public void ResetTimer()
    {
        //timer = 0f;
    }
}
