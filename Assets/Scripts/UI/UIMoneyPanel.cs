using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSystems;
using InputControll;
using TMPro;
using GridObjects;
using DG.Tweening;

namespace UI
{
    public class UIMoneyPanel : MonoBehaviour
    {
        [Header("Colors")]
        [SerializeField] private Color _addMoneyColor;
        [SerializeField] private Color _remMoneyColor;

        [Header("Main panel")]
        [SerializeField] private TextMeshProUGUI _mainMoneyLabel;

        [Header("Animation panel")]
        [SerializeField] private RectTransform _animationPanel;
        [SerializeField] private TextMeshProUGUI _animationMoneyLabel;
        [SerializeField] private RectTransform _animationTarget;

        private Sequence _animationSeq;

        private TurnManager _turnManager;
        private ConstructionController _constructionController;

        private void OnEnable()
        {
            _constructionController = SystemsManager.Instance.Get<ConstructionController>();
            _constructionController.OnBuildingBuild += AnimOnBuildingBuild;

            _turnManager = SystemsManager.Instance.Get<TurnManager>();
            _turnManager.TurnStart += UpdateMoneyLabel;
            UpdateMoneyLabel();
        }
        private void OnDisable()
        {
            if (_constructionController != null)
                _constructionController.OnBuildingBuild -= AnimOnBuildingBuild;

            if (_turnManager != null)
                _turnManager.TurnStart -= UpdateMoneyLabel;
        }


        private void AnimOnBuildingBuild((GridObject placedBuilding, GridObject objectPattern) placeEvt)
        {
            if (placeEvt.placedBuilding == null)
                return;

            int points = TurnManager.PointsToDisplayedPoints(placeEvt.objectPattern.PointsForPlaced);

            _animationMoneyLabel.text = points.ToString("+#;-#;0");
            _animationMoneyLabel.color = points >= 0 ? _addMoneyColor : _remMoneyColor;

            var worldPos = placeEvt.placedBuilding.transform.position;
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos) + Vector3.up * 10;

            _animationSeq.Kill();
            _animationSeq = DOTween.Sequence()
                .AppendInterval(.5f)
                .AppendCallback(Show)
                .Append(_animationPanel.DOScale(Vector3.one, .2f).SetEase(Ease.InQuad))
                .Append(_animationPanel.DOMove(_animationTarget.position, .5f).SetEase(Ease.OutQuad))
                .Append(_animationPanel.DOScale(Vector3.zero, .2f).SetEase(Ease.OutQuad))
                .AppendCallback(UpdateMoneyLabel)
                .AppendCallback(Hide)
            ;

            void Show()
            {
                _animationPanel.localScale = Vector3.zero;
                _animationPanel.position = screenPos;
                _animationPanel.gameObject.SetActive(true);
            }
            void Hide()
            {
                _animationPanel.gameObject.SetActive(false);
            }
        }

        private void UpdateMoneyLabel()
        {
            int points = _turnManager.DisplayedPoints;
            _mainMoneyLabel.text = points.ToString();
            _mainMoneyLabel.color = points >= 0 ? _addMoneyColor : _remMoneyColor;
            _mainMoneyLabel.rectTransform.DOShakeScale(.4f, .5f);
        }
    }
}