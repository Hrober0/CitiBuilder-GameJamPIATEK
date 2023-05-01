using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;

namespace UI.Tutorial
{
    public class UITutorial : MonoBehaviour
    {
        [SerializeField] private UITutorialMess _templateMess;

        [SerializeField, TextArea] private string _resetMess = "Reset";

        private TurnManager _turnManager;

        
        private readonly HashSet<string> _usedMessanges = new();

        private readonly List<UITutorialMess> _activeMessanges = new();
        private readonly List<UITutorialMess> _unactiveMessanges = new();


        private void OnEnable()
        {
            _turnManager = SystemsManager.Instance.Get<TurnManager>();
            _turnManager.TurnStart += ShowResetTip;

            _templateMess.SetActive(false);
        }
        private void OnDisable()
        {
            if (_turnManager != null)
            {
                _turnManager.TurnStart -= ShowResetTip;

            }
        }

        private void ShowResetTip() => TryShowMess(_resetMess);

        void TryShowMess(string messText)
        {
            if (_usedMessanges.Contains(messText))
                return;

            _usedMessanges.Add(messText);

            UITutorialMess messObj;
            if (_unactiveMessanges.Count == 0)
            {
                messObj = Instantiate(_templateMess, _templateMess.transform.parent);
                messObj.name = $"Mess{_unactiveMessanges.Count + _activeMessanges.Count}";
                _unactiveMessanges.Add(messObj);
            }
            else
            {
                messObj = _unactiveMessanges[0];
                _unactiveMessanges.RemoveAt(0);
            }

            _activeMessanges.Add(messObj);
            messObj.Set(messText, () => HideMess(messObj));
        }

        private void HideMess(UITutorialMess messObj)
        {
            if (!_activeMessanges.Contains(messObj))
            {
                Debug.LogWarning($"Mess {messObj.name} in not active!");
                return;
            }

            messObj.SetActive(false);
            _activeMessanges.Remove(messObj);
            _unactiveMessanges.Add(messObj);
        }
    }
}