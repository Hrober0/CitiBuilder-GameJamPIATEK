using GameSystems;
using GridObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeatSimulation
{
    public class HeatGenerator : GridObjectModule
    {
        public float HeatGeneration => heatGeneration;



        [SerializeField]
        private float heatGeneration;


        private HeatManager _heatManager;


        public override void OnBuildingConstructed()
        {
            base.OnBuildingConstructed();

            _heatManager = SystemsManager.Instance.Get<HeatManager>();
            _heatManager.RegisterGenerator(this);
        }

        public override void OnBuildingDestroyed()
        {
            base.OnBuildingDestroyed();

            if (_heatManager)
                _heatManager.RemoveGenerator(this);
        }
    }
}