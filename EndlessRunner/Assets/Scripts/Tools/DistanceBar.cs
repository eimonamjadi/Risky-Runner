using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class DistanceBar : MonoBehaviour
{
    protected Slider _slider;
    public CinemachineVirtualCamera virtualCamera;
    protected CinemachineTrackedDolly dolly;
    public static float DistanceTraveled;
    protected float DistanceTravledPrevFrame;
    public static float DistanceTraveledDuringLastFrame;
    public float InitialOffset;

    private void OnEnable()
    {
        PlayerManager.OnGameStarted += UpdateInitialOffset;
    }

    private void OnDisable()
    {
        PlayerManager.OnGameStarted -= UpdateInitialOffset;
    }

    private void Start()
    {
        _slider = GetComponent<Slider>();
        dolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        if (virtualCamera == null) virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (PlayerManager.isGameStarted && dolly)
        {
            DistanceTravledPrevFrame = DistanceTraveled;
            DistanceTraveled = dolly.m_PathPosition - InitialOffset;
            DistanceTraveledDuringLastFrame = DistanceTraveled - DistanceTravledPrevFrame;
            _slider.value = Mathf.Clamp01(DistanceTraveled / (dolly.m_Path.PathLength - InitialOffset - 5f));
        }
    }

    public void UpdateInitialOffset()
    {
        InitialOffset = dolly.m_PathPosition;
    }
}
