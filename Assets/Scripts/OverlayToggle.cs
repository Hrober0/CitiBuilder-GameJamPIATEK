using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayToggle : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer overlay;

    private void OnTriggerEnter(Collider other)
    {
        overlay.enabled = true;
    }

    private void OnTriggerExit(Collider other)
    {
        overlay.enabled = false;
    }
}
