using GameSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UINextTurn : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;

        [SerializeField] private Button _nextButton;

        [SerializeField] private TextMeshProUGUI _pointsLabel;
        [SerializeField] private TextMeshProUGUI _pointsIncomeLabel;
        [SerializeField] private TextMeshProUGUI _heatPenaltyLabel;

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

            _pointsLabel.text = _turnManager.DisplayedPoints.ToString();
            _pointsIncomeLabel.text = _turnManager.PointsIncom.ToString();
            _heatPenaltyLabel.text = "Not yet";
        }

        private void PlayNextRound()
        {
            _canvas.enabled = false;
            _turnManager.NextTurn();
        }
    }
}