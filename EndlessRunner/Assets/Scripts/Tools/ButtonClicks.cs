using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClicks : MonoBehaviour
{
    public void LoadMainMenu(bool saveData)
    {
        if (saveData)
        {
            if (Loader.Instance.LoadingFixedSpeedLevel)
            {
                NewPlayerController newPlayerController = FindObjectOfType<NewPlayerController>();
                if (newPlayerController)
                {
                    FixedLevelData newData = new FixedLevelData(Loader.Instance.CurrReportNum, Loader.Instance.FixedSpeedRepetition, Loader.Instance.GetSpeed(Loader.Instance.FixedSpeedIndex), ScoreDisplay.Score, FormatTime.Current.TimeElapsed,
                                                                newPlayerController.DistanceTraveled, TileManager.StraightTilesNum, TileManager.CurvedTilesNum,
                                                                Obstacles.ObstaclesGenerated, StumbleCount.Count, newPlayerController.TurningsPressed, newPlayerController.RandomHeadingsOccurred,
                                                                newPlayerController.RandomHeadingChance, ScoreDisplay.OffPathDuration);
                    // local version
                    MetricsManager.WriteFixedLevelMetricsToCSV(newData);
                    // cloud version
                    if (!Application.isEditor) Loader.Instance.WriteToGoogleForm(newData);
                }
                else
                {
                    Debug.LogWarning("Missing a <NewPlayerController> class");
                }
            }
            else
            {
                NewPlayerController newPlayerController = FindObjectOfType<NewPlayerController>();
                if (newPlayerController)
                {
                    SelfPacedData newData = new SelfPacedData(Loader.Instance.CurrReportNum, Loader.Instance.SelfPacedTrialCount, newPlayerController.MaxMoveSpeed, newPlayerController.MinMoveSpeed, newPlayerController.NumSpeedChanges,
                                                              newPlayerController.AvgMoveSpeed, newPlayerController.MedianMoveSpeed, ScoreDisplay.Score, FormatTime.Current.TimeElapsed,
                                                              newPlayerController.DistanceTraveled, TileManager.StraightTilesNum, TileManager.CurvedTilesNum,
                                                              Obstacles.ObstaclesGenerated, StumbleCount.Count, newPlayerController.TurningsPressed, newPlayerController.RandomHeadingsOccurred,
                                                              newPlayerController.AverageRandomHeadingChances, ScoreDisplay.OffPathDuration);
                    // local version
                    MetricsManager.WriteSelfPacedMetricsToCSV(newData);
                    // cloud version
                    Loader.Instance.WriteToGoogleForm(newData);
                }
                else
                {
                    Debug.LogWarning("Missing a <NewPlayerController> class");
                }
            }
        }
        FindObjectOfType<Loader>().Load("LevelSelection");
    }

    public void LoadFixedSpeedLevel()
    {
        Loader.Instance.LoadNewFixedSpeedLevel();
    }

    public void LoadFixedLevelWithSelectedSettings()
    {
        AudioSource source;
        if (CustomSettingManager.HasInstance && CustomSettingManager.Current.SettingSelected)
        {
            Loader.Instance.SetFixedLevelSetting(CustomSettingManager.Current.GetSettingData());
            LoadFixedSpeedLevel();
            if (TryGetComponent(out source) && GUIManager.Current.SuccessMouseClick != null)
            {
                source.clip = GUIManager.Current.SuccessMouseClick;
                source.Play();
            }
        }
        else
        {
            if (TryGetComponent(out source) && GUIManager.Current.FailedMouseClick != null)
            {
                source.clip = GUIManager.Current.FailedMouseClick;
                source.Play();
            }
        }
    }

    public void LoadSelfPacedLevel()
    {
        FindObjectOfType<Loader>().LoadSelfPacedLevel();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SetHightLight(CustomSettingView view)
    {
        if (CustomSettingManager.HasInstance)
        {
            CustomSettingManager.Current.SetHightLight(view);
            AudioSource source;
            if (TryGetComponent(out source))
            {
                source.Play();
            }
        }
    }

    public void ConfirmAddSetting(CanvasGroup ViewSettingMenu)
    {
        AudioSource source;
        if (CustomSettingManager.HasInstance)
        {
            if (CustomSettingManager.Current.AddNewCustomSetting(FindObjectOfType<CustomSettingNew>()))
            {
                if (TryGetComponent(out source) && GUIManager.Current.SuccessMouseClick != null)
                {
                    source.clip = GUIManager.Current.SuccessMouseClick;
                    source.Play();
                }
                OpenViewSettingMenu(ViewSettingMenu);
            }
            else
            {
                if (TryGetComponent(out source) && GUIManager.Current.FailedMouseClick != null)
                {
                    source.clip = GUIManager.Current.FailedMouseClick;
                    source.Play();
                }
            }
        }
    }

    public void OpenCreateSettingMenu(CanvasGroup NewSettingMenu)
    {
        if (GUIManager.HasInstance)
        {
            AudioSource source;
            if (CustomSettingManager.HasInstance && CustomSettingManager.Current.CanCreateNew)
            {
                GUIManager.TurnOnMenu(NewSettingMenu);
                if (TryGetComponent(out source) && GUIManager.Current.SuccessMouseClick != null)
                {
                    source.clip = GUIManager.Current.SuccessMouseClick;
                    source.Play();
                }
            }
            else
            {
                if (TryGetComponent(out source) && GUIManager.Current.FailedMouseClick != null)
                {
                    source.clip = GUIManager.Current.FailedMouseClick;
                    source.Play();
                }
            }
        }
    }

    public void OpenViewSettingMenu(CanvasGroup ViewSettingMenu)
    {
        AudioSource source;
        if (CustomSettingManager.HasInstance)
        {
            GUIManager.TurnOnMenu(ViewSettingMenu);
            if (TryGetComponent(out source) && GUIManager.Current.SuccessMouseClick != null)
            {
                source.clip = GUIManager.Current.SuccessMouseClick;
                source.Play();
            }
        }
        else
        {
            if (TryGetComponent(out source) && GUIManager.Current.FailedMouseClick != null)
            {
                source.clip = GUIManager.Current.FailedMouseClick;
                source.Play();
            }
        }
    }

    public void OpenLevelSelectionMenu(CanvasGroup levelSelection)
    {
        AudioSource source;
        if (CustomSettingManager.HasInstance && CustomSettingManager.Current.CanCreateNew)
        {
            GUIManager.TurnOnMenu(levelSelection);
            if (TryGetComponent(out source) && GUIManager.Current.SuccessMouseClick != null)
            {
                source.clip = GUIManager.Current.SuccessMouseClick;
                source.Play();
            }
        }
        else
        {
            if (TryGetComponent(out source) && GUIManager.Current.FailedMouseClick != null)
            {
                source.clip = GUIManager.Current.FailedMouseClick;
                source.Play();
            }
        }
    } 

    public void OpenSensitivityMenu(CanvasGroup sensitivityMenu)
    {
        if (sensitivityMenu == null) return;
        AudioSource source;
        GUIManager.TurnOnMenu(sensitivityMenu);
        if (TryGetComponent(out source) && GUIManager.Current.SuccessMouseClick != null)
        {
            source.clip = GUIManager.Current.SuccessMouseClick;
            source.Play();
        }
    }

    public void ConfirmChangeSensitivity()
    {
        AudioSource source;
        Loader.Instance.LoadSensitivitySliderToPlayerController();
        if (TryGetComponent(out source) && GUIManager.Current.SuccessMouseClick != null)
        {
            source.clip = GUIManager.Current.SuccessMouseClick;
            source.Play();
        }
        GUIManager.Current.OpenPauseMenu();
    }

    public void DeleteCustomSetting(CustomSettingView view)
    {
        if (CustomSettingManager.HasInstance)
        {
            CustomSettingManager.Current.DeleteCustomSetting(view.ID);
        }
    }

    public void TurnOnInstruction()
    {
        AudioSource source;
        if (GUIManager.HasInstance)
        {
            GUIManager.TurnOnMenu(GUIManager.Current.InstructionPanel);
            if (TryGetComponent(out source) && GUIManager.Current.SuccessMouseClick != null)
            {
                source.clip = GUIManager.Current.SuccessMouseClick;
                source.Play();
            }
        }
        else if (GUIManager.HasInstance)
        {
            if (TryGetComponent(out source) && GUIManager.Current.FailedMouseClick != null)
            {
                source.clip = GUIManager.Current.FailedMouseClick;
                source.Play();
            }
        }
    }

    public void TurnOffInstruction()
    {
        AudioSource source;
        if (GUIManager.HasInstance)
        {
            GUIManager.TurnOffMenu(GUIManager.Current.InstructionPanel);
            if (TryGetComponent(out source) && GUIManager.Current.SuccessMouseClick != null)
            {
                source.clip = GUIManager.Current.SuccessMouseClick;
                source.Play();
            }
        }
        else if (GUIManager.HasInstance)
        {
            if (TryGetComponent(out source) && GUIManager.Current.FailedMouseClick != null)
            {
                source.clip = GUIManager.Current.FailedMouseClick;
                source.Play();
            }
        }
    }
}
