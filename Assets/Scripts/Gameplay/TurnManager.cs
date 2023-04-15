using GridObjects;
using InputControll;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSystems
{
    public class TurnManager : GameSystem
    {
        public event Action TurnPasses;

        [SerializeField]
        public TurnCostManager TurnCost;

        [SerializeField]
        private BucketRandom<GridObject> objectsRandomiser;


        private ConstructionController _constructionController;

        protected override void InitSystem()
        {
            TurnCost.Init(this, Grids.WorldGrid.Instance);

            _constructionController = _systems.Get<ConstructionController>();
            _constructionController.OnBuildingBuild += OnBuildingBuild;

            NextTurn();
        }
        protected override void DeinitSystem()
        {
            _constructionController.OnBuildingBuild -= OnBuildingBuild;
        }

        private void OnBuildingBuild(GridObject obj)
        {
            NextTurn();
        }
        public void NextTurn()
        {
            TurnPasses?.Invoke();
            _constructionController.SetObject(objectsRandomiser.GetRandom());
        }
    }
}