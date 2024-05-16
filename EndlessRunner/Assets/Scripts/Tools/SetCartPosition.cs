using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class SetCartPosition : MonoBehaviour
{
    CinemachineDollyCart cart;
    public CinemachineVirtualCamera cam;
    public float PathOffset = 5f;
    protected CinemachineTrackedDolly trackedDolly;

    private void Awake()
    {

    }

    void FixedUpdate()
    {
        cart.m_Position = trackedDolly.m_PathPosition + PathOffset;
        cart.transform.rotation = trackedDolly.m_Path.EvaluateOrientation(trackedDolly.m_PathPosition);
    }
}
