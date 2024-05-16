using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class UpdateCamRotation : MonoBehaviour
{
    public CinemachineDollyCart Cart;
    public float LookDownDeg = 20f;
    CinemachineVirtualCamera virtualCam;
    CinemachineTrackedDolly dolly;
    CinemachinePathBase path;
    Camera mainCam;

    private void Awake()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>();
        dolly = virtualCam.GetCinemachineComponent<CinemachineTrackedDolly>();
        path = dolly.m_Path;
    }

    private void FixedUpdate()
    {
        //Cart.m_Position = Mathf.Clamp(dolly.m_PathPosition + 5f, 0f, path.MaxPos);
        //transform.rotation = Cart.transform.rotation;
        //Debug.Log("Rotation is " + transform.rotation);
        //transform.Rotate(Vector3.right, LookDownDeg);
    }

}
