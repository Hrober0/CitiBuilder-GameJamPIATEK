using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporarryCameraSwitcherArrr : MonoBehaviour
{
    public CinemachineVirtualCamera VC;

    public Transform TopPos;
    public Transform SidePos;

    private IEnumerator Start()
    {
        yield return null;
        InputManager.SecondaryAction.Started += SwitchPerSperspective;
    }

    private void SwitchPerSperspective()
    {
        if (VC.Follow == TopPos)
        {
            VC.Follow = SidePos;
        }
        else
        {
            VC.Follow = TopPos;
        }
    }
}
