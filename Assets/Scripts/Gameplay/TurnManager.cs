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
        public class HandField
        {
            public GridObject GridObject { get; private set; }
            public bool IsSelected { get; set; }

            public HandField(GridObject gridObject)
            {
                this.GridObject = gridObject;
                this.IsSelected = false;
            }
            public HandField(GridObject gridObject, bool isSelected)
            {
                this.GridObject = gridObject;
                this.IsSelected = isSelected;
            }
        }


        public event Action TurnPasses;

        [SerializeField] private TurnCostManager _turnCost;

        [SerializeField] private BucketRandom<GridObject> _objectsRandomiser;


        private ConstructionController _constructionController;

        private int _cardsInTour = 5;


        protected override void InitSystem()
        {
            _turnCost.Init(this, Grids.WorldGrid.Instance);

            _constructionController = _systems.Get<ConstructionController>();
            _constructionController.OnBuildingBuild += OnBuildingBuild;

            NextTurn();
        }
        protected override void DeinitSystem()
        {
            _constructionController.OnBuildingBuild -= OnBuildingBuild;
        }


        private readonly List<HandField> _handCards = new();
        public IReadOnlyList<HandField> HandCards => _handCards;
        public event Action OnHandChanged;

        public void SelectCard(int index, bool isSelected)
        {
            Debug.Log($"Selected card on {index} index, is selected {isSelected}");

            foreach (var card in _handCards)
                card.IsSelected = false;

            _constructionController.SetObject(null);


            if (isSelected)
            {
                var card = GetCard(index);
                if (card == null)
                    return;


                _handCards[index] = new(card.GridObject, isSelected);
                _constructionController.SetObject(card.GridObject);
            }
            
            OnHandChanged?.Invoke();
        }
        public HandField GetCard(int index)
        {
            if (index < 0 || index >= _handCards.Count)
            {
                Debug.LogWarning($"Missing card on index {index}");
                return null;
            }

            return _handCards[index];
        }


        private void OnBuildingBuild((GridObject placed, GridObject pattern) value)
        {
            foreach (var item in _handCards)
            {
                if (item.IsSelected && item.GridObject == value.pattern)
                {
                    _handCards.Remove(item);
                    OnHandChanged?.Invoke();
                    break;
                }
            }

            if (_handCards.Count == 0)
                NextTurn();
        }
        public void NextTurn()
        {
            TurnPasses?.Invoke();

            _handCards.Clear();
            for (int i = 0; i < _cardsInTour; i++)
                _handCards.Add(new(_objectsRandomiser.GetRandom()));

            OnHandChanged?.Invoke();
        }
    }
}