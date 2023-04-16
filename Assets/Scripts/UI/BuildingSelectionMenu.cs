using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GridObjects;

namespace UI.HUD
{
    public class BuildingSelectionMenu : MonoBehaviour
    {
        [SerializeField] private HUDCard _cardPrefab;
        [SerializeField] private Transform _cardsParent;

        [SerializeField] private GridObject _temptBuild;

        private readonly List<HUDCard> _displayedCards = new();
        private readonly List<HUDCard> _unusedCards = new();

        private void Start()
        {
            AddCard(_temptBuild);
            AddCard(_temptBuild);
            AddCard(_temptBuild);
            AddCard(_temptBuild);
            AddCard(_temptBuild);
        }

        public void AddCard(GridObject obj)
        {
            if (_unusedCards.Count == 0)
                _unusedCards.Add(Instantiate(_cardPrefab, _cardsParent));

            var card = _unusedCards[0];
            _unusedCards.RemoveAt(0);
            card.Show(obj.DisplayedName, null, Random.Range(-3, 4), () => CardClick(card, obj));

            _displayedCards.Add(card);
        }
        public void HideCard(HUDCard card)
        {
            if (_displayedCards.Remove(card))
            {
                _unusedCards.Add(card);
                card.Hide();
            }
        }

        private void CardClick(HUDCard card, GridObject obj)
        {
            if (card.IsInCancelMode)
            {
                card.SetSelect(false);
                Debug.Log("TODO: deslect card");
            }
            else
            {
                card.SetSelect(true);
                Debug.Log("TODO: slect card");
            }
        }
    }
}