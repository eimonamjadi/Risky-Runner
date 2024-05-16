using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MMSingleton<GUIManager>
{
    public CanvasGroup PauseMenu;
    public CanvasGroup VictoryPanel;
    public CanvasGroup ViewSettingsMenu;
    public CanvasGroup NewSettingMenu;
    public CanvasGroup SensitivityMenu;
    public CanvasGroup InstructionPanel;
    public TextMeshProUGUI ControlInstructionField;
    public Image FlashPanel;
    public MMTouchButton LeftButton;
    public MMTouchButton RightButton;
    public bool DirectionButtonPressed
    {
        get { return LeftButton.ButtonIsPressed || RightButton.ButtonIsPressed; }   
    }

    public AudioClip SuccessMouseClick;
    public AudioClip FailedMouseClick;
    public CanvasGroup CurrActiveCanvas;
    public Text TapToStart; 

    private void Start()
    {
        /// Manage Canvases
        TurnOffMenu(NewSettingMenu);
        TurnOffMenu(ViewSettingsMenu);
        TurnOffMenu(VictoryPanel);
        TurnOffMenu(PauseMenu);
        TurnOffMenu(SensitivityMenu);
        if (CurrActiveCanvas != null)
        {
            CurrActiveCanvas.alpha = 1f;
            CurrActiveCanvas.blocksRaycasts = true;
        }

        /// Customize Instructions for Different Platforms and Levels
        GameObject tapObj = GameObject.Find("TapToStart");
        bool bFoundTap = false;
        if (tapObj && tapObj.TryGetComponent(out TapToStart))
        {
            bFoundTap = true;
        }
        /// Mobile
        if (Application.isMobilePlatform)
        {
            if (LeftButton) LeftButton.gameObject.SetActive(true);
            if (RightButton) RightButton.gameObject.SetActive(true);
            if (bFoundTap)
            {
                if (Loader.Instance.LoadingFixedSpeedLevel)
                {
                    TapToStart.text = "";
                    ControlInstructionField.text = "Press the [LEFT] and [RIGHT] buttons to dodge obstacles.";
                }
                else
                {
                    TapToStart.text = "";
                    ControlInstructionField.text = "Press the [LEFT] and [RIGHT] buttons to dodge obstacles.\nSwip [UP] and [DOWN] to adjust your speed.";
                }
            }
        }
        /// PC
        else
        {
            if (LeftButton) LeftButton.gameObject.SetActive(false);
            if (RightButton) RightButton.gameObject.SetActive(false);
            if (bFoundTap)
            {
                if (Loader.Instance.LoadingFixedSpeedLevel)
                {
                    TapToStart.text = "";
                    ControlInstructionField.text = "Press the [LEFT] and [RIGHT] arrow keys to dodge obstacles.";
                }
                else
                {
                    TapToStart.text = "";
                    ControlInstructionField.text = "Press the [LEFT] and [RIGHT] buttons to dodge obstacles.\nPress [UP] and [DOWN] to adjust your speed.";
                }
            }
        }
    }

    public bool TurnOnInstruction()
    {
        if (InstructionPanel != null)
        {
            TurnOnMenu(InstructionPanel);
            return true;
        }
        return false;
    }

    public void OpenVictoryMenu()
    {
        if (VictoryPanel != null)
        {
            TurnOnMenu(VictoryPanel);
            VictoryPanel.GetComponentInChildren<Countdown>().StartCountDown();
        }
    }

    public void OpenPauseMenu()
    {
        if (PauseMenu != null)
        {
            TurnOnMenu(PauseMenu);
            AudioSource source;
            if (TryGetComponent(out source) && Current.SuccessMouseClick != null)
            {
                source.clip = Current.SuccessMouseClick;
                source.Play();
            }
            if (LeftButton != null) LeftButton.gameObject.SetActive(false);
            if (RightButton != null) RightButton.gameObject.SetActive(false);
        }
    }

    public void ClosePauseMenu()
    {
        if (PauseMenu != null)
        {
            TurnOffMenu(PauseMenu);
            if (CurrActiveCanvas == PauseMenu) CurrActiveCanvas = null;
            if (LeftButton != null) LeftButton.gameObject.SetActive(true);
            if (RightButton != null) RightButton.gameObject.SetActive(true);
        }
    }

    public static void TurnOnMenu(CanvasGroup canvasGroup)
    {
        if (Current.CurrActiveCanvas != null)
        {
            TurnOffMenu(Current.CurrActiveCanvas);
            Current.CurrActiveCanvas = null;
        }
        Current.CurrActiveCanvas = canvasGroup;
        if (canvasGroup == null) return;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public static void TurnOffMenu(CanvasGroup canvasGroup)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
