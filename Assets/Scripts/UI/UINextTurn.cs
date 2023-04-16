using GameSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UINextTurn : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _nextButton;

        private TurnManager _turnManager;

        private void OnEnable()
        {
            _turnManager = FindObjectOfType<TurnManager>();
            _turnManager.TurnEndSmimulationEnd += Open;

            _nextButton.onClick.AddListener(PlayNextRound);
        }

        private void OnDisable()
        {
            if (_turnManager != null)
                _turnManager.TurnEndSmimulationEnd -= Open;

            _nextButton.onClick.RemoveListener(PlayNextRound);
        }

        private void Open()
        {
            _canvas.enabled = true;
        }

        private void PlayNextRound()
        {
            _canvas.enabled = false;
            _turnManager.NextTurn();
        }
    }
}