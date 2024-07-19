using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrioritySetting : MonoBehaviour
{
    public CinemachineFreeLook freeLookCam1;
    public CinemachineFreeLook freeLookCam2;

    public void buttonFreeLook1()
    {
        freeLookCam1.Priority = 12;
        freeLookCam2.Priority = 11;
    }

    public void buttonFreeLook2()
    {
        freeLookCam1.Priority = 11;
        freeLookCam2.Priority = 12;
    }
}
