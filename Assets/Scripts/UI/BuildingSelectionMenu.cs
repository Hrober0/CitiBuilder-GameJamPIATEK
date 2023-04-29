using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GridObjects;
using GameSystems;

namespace UI.HUD
{
    public class BuildingSelectionMenu : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private UIHUDCard _cardPrefab;
        [SerializeField] private Transform _cardsParent;

        [SerializeField] private GridObject _temptBuild;

        private readonly List<UIHUDCard> _displayedCards = new();
        private readonly List<UIHUDCard> _unusedCards = new();


        private TurnManager _turnManager;

        private void OnEnable()
        {
            _turnManager = FindObjectOfType<TurnManager>();
            if (_turnManager == null)
            {
                Debug.LogWarning($"{typeof(TurnManager)} not found");
                enabled = false;
                return;
            }

            UpdateCards();
            _turnManager.OnHandChanged += UpdateCards;
        }
        private void OnDisable()
        {
            if (_turnManager != null)
                _turnManager.OnHandChanged -= UpdateCards;
        }

        private void UpdateCards()
        {
            for (int i = _displayedCards.Count - 1; i >= 0; i--)
                HideCard(_displayedCards[i]);

            for (int i = 0; i < _turnManager.HandCards.Count; i++)
            {
                var card = _turnManager.HandCards[i];
                AddCard(card.GridObject, i, card.IsSelected);
            }

            _canvas.enabled = _displayedCards.Count > 0;
        }

        public void AddCard(GridObject obj, int index, bool isSelected)
        {
            if (_unusedCards.Count == 0)
                _unusedCards.Add(Instantiate(_cardPrefab, _cardsParent));

            var card = _unusedCards[0];
            _unusedCards.RemoveAt(0);
            card.transform.SetSiblingIndex(_displayedCards.Count);

            card.Show(obj.DisplayedName, obj.Icon, obj.DeltaHot, isSelected, () => CardClick(index));

            _displayedCards.Add(card);
        }
        public void HideCard(UIHUDCard card)
        {
            if (_displayedCards.Remove(card))
            {
                _unusedCards.Add(card);
                card.Hide();
            }
        }

        private void CardClick(int index)
        {
            var card = _turnManager.GetCard(index);
            if (card == null)
                return;

            _turnManager.SelectCard(index, !card.IsSelected);
        }
    }
}