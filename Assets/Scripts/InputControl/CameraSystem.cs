using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;
using HeatSimulation;

namespace InputControll
{
    public class CameraSystem : GameSystem
    {
        [SerializeField] private CinemachineVirtualCamera _vCamera;

        [SerializeField] private Transform _topPos;
        [SerializeField] private Transform _sidePos;

        protected override void InitSystem()
        {
            _systems.Get<HeatManager>().OnOverlaySwitch += OnOverlaySwitch;
        }
        protected override void DeinitSystem()
        {
            _systems.Get<HeatManager>().OnOverlaySwitch -= OnOverlaySwitch;
        }

        private void OnOverlaySwitch(bool active)
        {
            _vCamera.Follow = active ? _topPos : _sidePos;
        }
    }
}