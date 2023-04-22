using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HeatSimulation;
using GameSystems;

public class UIOverlayToogle : MonoBehaviour
{
    [SerializeField] private Toggle _toogle;

    private HeatManager _heatManager;

    private void OnEnable()
    {
        _heatManager = SystemsManager.Instance.Get<HeatManager>();
        _heatManager.OnOverlaySwitch += UpdateToggle;

        UpdateToggle(_heatManager.IsOverlayActive);
        _toogle.onValueChanged.AddListener(HandlSwich);
    }
    private void OnDisable()
    {
        _heatManager.OnOverlaySwitch -= UpdateToggle;

        _toogle.onValueChanged.RemoveListener(HandlSwich);
    }

    private void HandlSwich(bool active)
    {
        _heatManager.EnableOverlay(active);
    }
    private void UpdateToggle(bool active)
    {
        if (_toogle.isOn != active)
            _toogle.isOn = active;
    }
}
