using GameSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UISkipTourButton : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _button;

        private TurnManager _turnManager;

        private void OnEnable()
        {
            _turnManager = FindObjectOfType<TurnManager>();
            _turnManager.TurnReachSkippPoint += Open;
            _turnManager.TurnEndBuild += Close;

            _button.onClick.AddListener(SkipRound);
        }
        private void OnDisable()
        {
            if (_turnManager != null)
            {
                _turnManager.TurnReachSkippPoint -= Open;
                _turnManager.TurnEndBuild -= Close;
            }

            _button.onClick.RemoveListener(SkipRound);
        }

        private void Open()
        {
            _canvas.enabled = true;
        }
        private void Close()
        {
            _canvas.enabled = false;
        }

        private void SkipRound()
        {
            _turnManager.EndTour();
        }
    }
}

