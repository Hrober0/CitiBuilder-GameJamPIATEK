using GridObjects;
using Grids;
using InputControll;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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


        public event Action TurnEndBuild;
        public event Action TurnPasses;
        public event Action TurnEndSmimulationEnd;
        public event Action TurnReachSkippPoint;


        [SerializeField] private TurnCostManager _turnCost;

        [SerializeField] private BucketRandom<GridObject> _objectsRandomiser;


        private ConstructionController _constructionController;
        private TemporarryCameraSwitcherArrr _overlay;


        private readonly int _cardsInTour = 5;


        private float _points = 0;
        private float _heatPenalty = 0;
        private float _pointsAtRoundStart = 0;
        public int DisplayedPoints => PointsToDisplayedPoints(_points);
        public int PointsIncom => PointsToDisplayedPoints(_points - _pointsAtRoundStart + _heatPenalty);
        public int HeatPenalty => PointsToDisplayedPoints(_heatPenalty);

        public event Action OnPointsChanged;


        protected override void InitSystem()
        {
            _turnCost.Init(this, Grids.WorldGrid.Instance);

            _constructionController = _systems.Get<ConstructionController>();
            _constructionController.OnBuildingBuild += OnBuildingBuild;

            _overlay = FindObjectOfType<TemporarryCameraSwitcherArrr>();

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

            if (_handCards.Count <= 3)
                TurnReachSkippPoint?.Invoke();

            _points += value.pattern.PointsForPlaced;

            if (_handCards.Count == 0)
                EndTour();
        }

        public void EndTour()
        {
            StartCoroutine(ShowPropagation());
        }


        private IEnumerator ShowPropagation()
        {
            TurnEndBuild?.Invoke();

            _handCards.Clear();
            OnHandChanged?.Invoke();

            _overlay.SetOvelayActive(true);
            yield return new WaitForSeconds(1);

            TurnPasses?.Invoke();

            yield return new WaitForSeconds(3);

            _heatPenalty = _turnCost.NextTurnCost(WorldGrid.Instance);

            _points -= _heatPenalty;

            TurnEndSmimulationEnd?.Invoke();
        }

        public void NextTurn()
        {
            _pointsAtRoundStart = _points;

            _overlay.SetOvelayActive(false);

            _handCards.Clear();
            for (int i = 0; i < _cardsInTour; i++)
                _handCards.Add(new(_objectsRandomiser.GetRandom()));

            OnHandChanged?.Invoke();
        }


        private int PointsToDisplayedPoints(float points) => Mathf.RoundToInt(points * 50);
    }
}