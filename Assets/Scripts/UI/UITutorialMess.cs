using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace UI.Tutorial
{
    public class UITutorialMess : MonoBehaviour
    {
        [SerializeField] private Color _activeColor;
        [SerializeField] private Image _bgImage;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private Button _button;


        private Action _onClick;
        private Sequence _sequence;


        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }
        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        public void Set(string text, Action onClick)
        {
            _label.text = text;
            _onClick = onClick;
            SetActive(true);

            _sequence.Kill();
            _bgImage.color = Color.white;
            _sequence = DOTween.Sequence()
                .SetLoops(-1)
                .Append(_bgImage.DOColor(_activeColor, 1))
                .AppendInterval(.1f)
                .Append(_bgImage.DOColor(Color.white, 1))
                .AppendInterval(.4f)
                ;
        }

        private void OnClick() => _onClick?.Invoke();

        public void SetActive(bool active)
        {
            if (!active)
                _sequence.Kill();
            gameObject.SetActive(active);
        }
    }
}