using GridObjects;
using InputControll;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;
using GameSystems;

namespace GameSystems
{
    public class TurnManager : GameSystem
    {
        public event Action TurnPasses;

        [SerializeField]
        public TurnCostManager TurnCost;

        [SerializeField]
        private ConstructionController constructionController;

        [SerializeField]
        private BucketRandom<GridObject> objectsRandomiser;


        public override void InitSystem()
        {
            TurnCost.Init(this, Grids.WorldGrid.Instance);
            constructionController.OnBuildingBuild += OnBuildingBuild;

            NextTurn();
        }
        public override void DeinitSystem()
        {
            constructionController.OnBuildingBuild -= OnBuildingBuild;
        }

        private void OnBuildingBuild(GridObject obj)
        {
            NextTurn();
        }
        public void NextTurn()
        {
            TurnPasses?.Invoke();
            constructionController.SetObject(objectsRandomiser.GetRandom());
        }
    }
}