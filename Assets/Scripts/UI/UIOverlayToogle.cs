using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOverlayToogle : MonoBehaviour
{
    [SerializeField] private Toggle _toogle;

    private TemporarryCameraSwitcherArrr _overlay;

    private void OnEnable()
    {
        _overlay = FindObjectOfType<TemporarryCameraSwitcherArrr>();

        if (_overlay)
        {
            _overlay.OnOverlayChanged += OnOverlayChanged;
            OnOverlayChanged();
        }
        _toogle.onValueChanged.AddListener(HandlSwich);
    }
    private void OnDisable()
    {
        if (_overlay)
            _overlay.OnOverlayChanged -= OnOverlayChanged;

        _toogle.onValueChanged.RemoveListener(HandlSwich);
    }

    private void HandlSwich(bool active)
    {
        if (_overlay)
            _overlay.SetOvelayActive(active);
    }
    private void OnOverlayChanged()
    {
        _toogle.isOn = _overlay.IsOverlayActive;
    }
}
