using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

namespace UI.HUD
{
    public class HUDCard : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameLabel;
        [SerializeField] private Button _mainButton;

        [Header("Object")]
        [SerializeField] private Image _objectImage;
        [SerializeField] private Sprite _cancelIcon;
        
        [Header("Temperature")]
        [SerializeField] private Image _tempImage;
        [SerializeField] private Sprite _hotIcon;
        [SerializeField] private Sprite _coldIcon;


        private Action _onClick;
        private Sprite _objectIcon;
        private readonly List<Image> _temperatureImages = new();

        private void Awake()
        {
            _mainButton.onClick.RemoveListener(() => _onClick?.Invoke());
            _tempImage.gameObject.SetActive(false);
            _temperatureImages.Add(_tempImage);
        }

        public void Show(string objectName, Sprite objectIcon, int hotDelta, Action onClick)
        {
            _nameLabel.text = objectName;

            _objectIcon = objectIcon;
            _onClick = onClick;

            if (hotDelta != 0)
            {
                var icon = hotDelta < 0 ? _coldIcon : _hotIcon;
                int index = 0;
                for (; index < Math.Abs(hotDelta); index++)
                {
                    if (index >= _temperatureImages.Count)
                        _temperatureImages.Add(Instantiate(_tempImage, _tempImage.transform.parent));
                    var img = _temperatureImages[index];
                    img.sprite = icon;
                    img.gameObject.SetActive(true);
                }
                for (; index < _temperatureImages.Count; index++)
                    _temperatureImages[index].gameObject.SetActive(false);
            }

            gameObject.SetActive(true);
        }
        public void Hide() => gameObject.SetActive(false);

        public void SetSelect(bool isSelected)
        {
            _objectImage.sprite = isSelected ? _objectIcon : _cancelIcon;
        }
        public bool IsInCancelMode => _objectImage.sprite == _cancelIcon;
    }
}