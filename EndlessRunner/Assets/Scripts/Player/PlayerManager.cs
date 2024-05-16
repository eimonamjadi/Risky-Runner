using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
	public static bool isGameOver;
    public static bool isGameStarted;
    public GameObject startingText;

    public static bool isGamePaused;
    public static event UnityAction OnPauseGame;
    public static event UnityAction OnResumeGame;
    public static event UnityAction OnGameStarted;

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        isGameStarted = false;
        isGamePaused = false;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameStarted && Input.GetMouseButtonUp(0))
        {
            StartGame();
        }

        if (!isGameOver && Input.GetKeyDown(KeyCode.Escape))
        {
            Pause(!isGamePaused);
        }
    }

    public void StartGame()
    {
        isGameStarted = true;
        if (OnGameStarted != null) OnGameStarted();
        Destroy(startingText);
    }

    public void Pause(bool pause)
    {
        isGamePaused = pause;
        /// Resume
        if (!isGamePaused)
        {
            if (OnResumeGame != null) OnResumeGame();
            GUIManager.Current.ClosePauseMenu();
        }
        else
        {
            /// Pause
            if (OnPauseGame != null) OnPauseGame();
            GUIManager.Current.OpenPauseMenu();
        }
    }
}
