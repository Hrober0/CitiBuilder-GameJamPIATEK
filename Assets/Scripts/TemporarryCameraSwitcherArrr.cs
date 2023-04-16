using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporarryCameraSwitcherArrr : MonoBehaviour
{
    public CinemachineVirtualCamera VC;

    public Transform TopPos;
    public Transform SidePos;

    public bool IsOverlayActive => VC.Follow == TopPos;

    public event Action OnOverlayChanged;

    private IEnumerator Start()
    {
        yield return null;
        InputManager.SecondaryAction.Started += SwitchPerSperspective;
    }

    public void SetOvelayActive(bool active)
    {
        if (IsOverlayActive != active)
        {
            SwitchPerSperspective();
            OnOverlayChanged?.Invoke();
        }
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

        OnOverlayChanged?.Invoke();
    }
}
