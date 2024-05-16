using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class ScoreDisplay : MonoBehaviour
{
    protected Text textField;
    protected static int score = 0;
    public static int Score { get { return score; } }
    public static float OffPathDuration { get { return _offPathTimer; } }
    [Tooltip("Weights for Collision on Obstacles Penalty")]
    protected float A = 0.5f; // Reduced from 1f
    [Tooltip("Weights for Off-Track Penalty")]
    protected float B = 0.5f; // Reduced from 1f
    [Tooltip("Weights for Distance Travelled")]
    public float C = 1f;
    private static float _offPathTimer = 0f;

    public Color PenalizedColor;
    public Color WarningColor;
    protected Color OriginalColor;
    protected bool bInWarningState = false;

    public enum ScoreCountType
    {
        ContinuousModel,
        OneShotPenaltyModel
    }
    public ScoreCountType ScoreType = ScoreCountType.ContinuousModel;
    protected Coroutine _activeCoroutine = null;
    protected float PrevTimeElapsed;
    float clampedTotalTime = 1f;
    float ClampedTimeInverse;

    private void Awake()
    {
        textField = GetComponent<Text>();
        OriginalColor = textField.color;
    }

    private void Start()
    {
        score = 0;
        _offPathTimer = 0f;
        StartCoroutine(IncrementC());
    }

    private void Update()
    {
        if (PlayerManager.isGameStarted && !PlayerManager.isGamePaused)
        {
            clampedTotalTime = Mathf.Max(1f, FormatTime.Current.TimeElapsed);
            if (ScoreType == ScoreCountType.ContinuousModel)
            {
                // Calculate inferred speed
                float inferredSpeed = DistanceBar.DistanceTraveled / clampedTotalTime;

                // Determine if speed is greater than the threshold
                float speedThreshold = 20f;
                float speedMultiplier = inferredSpeed > speedThreshold ? 1.5f : 1.0f;

                float collisionPenalty = A * StumbleCount.Count * speedMultiplier;
                float offPathPenalty = B * _offPathTimer / clampedTotalTime * speedMultiplier;

                // Calculate total penalty
                float totalPenalty = collisionPenalty + offPathPenalty;
                // Cap the total penalty to avoid excessive deduction
                float maxPenalty = 20f; // Example cap value
                totalPenalty = Mathf.Min(totalPenalty, maxPenalty);

                score = Mathf.Max(0, Mathf.FloorToInt(ClampedTimeInverse * (C * DistanceBar.DistanceTraveled - totalPenalty)));
            }
            textField.text = score.ToString();
        }
    }


    public void AddToOffPathTimer(float amount)
    {
        _offPathTimer += amount;
    }

    protected IEnumerator IncrementC()
    {
        PrevTimeElapsed = FormatTime.Current.TimeElapsed;
        A = C * 10f;
        B = C * 3f;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            if (FormatTime.Current.TimeElapsed - PrevTimeElapsed > 1.5f)
            {
                C += .2f;
                A = clampedTotalTime / 10f * C * 10f;
                B = clampedTotalTime / 10f * C * 3f;
                ClampedTimeInverse = 1 / clampedTotalTime;
                PrevTimeElapsed = FormatTime.Current.TimeElapsed;
            }
        }
    }

    public void InWarningState()
    {
        bInWarningState = true;
        if (textField.color != PenalizedColor)
        {
            textField.color = WarningColor;
            textField.fontStyle = FontStyle.Bold;
        }
    }

    public void ResetWarningColor()
    {
        bInWarningState = false;
        if (textField.color != PenalizedColor)
        {
            textField.color = OriginalColor;
            textField.fontStyle = FontStyle.Normal;
        }
    }

    public void SetTextPenalizedColor()
    {
        StopCoroutine(SetRedColorCoroutine());
        StartCoroutine(SetRedColorCoroutine());
    }

    protected IEnumerator SetRedColorCoroutine()
    {
        textField.color = PenalizedColor;
        textField.fontStyle = FontStyle.Bold;
        yield return new WaitForSeconds(1.5f);
        textField.color = bInWarningState ? WarningColor : OriginalColor;
        textField.fontStyle = bInWarningState ? FontStyle.Bold : FontStyle.Normal;
    }
}
