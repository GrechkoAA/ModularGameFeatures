using System;
using UnityEngine;
using UnityEngine.UI;

namespace Examples.Features.Currency.Bank.Scripts.UI.Currency.View
{
    public class CurrencyOperationView : MonoBehaviour
    {
        [SerializeField] private Button _button;

        public event Action OnClicked;

        private void Awake() =>
            _button.onClick.AddListener(Clicked);

        private void OnDestroy() =>
            _button.onClick.RemoveListener(Clicked);

        public void SetInteractable(bool interactable) =>
            _button.interactable = interactable;

        private void Clicked() =>
            OnClicked?.Invoke();
    }
}