using System;
using Examples.Features.Currency.Bank.Scripts.System;
using Modules.Features.Currency.Bank.Scripts;
using UnityEngine;

namespace Examples.Features.Currency.Bank.Scripts.UI.Currency.Presenters.CurrencyOperationPanel
{
    public class CurrencyOperationPanelPresenter : ICurrencyOperationPanelPresenter, IDisposable
    {
        private readonly CurrencyCell _currencyCell;
        private readonly CurrencyOperationModel _currencyOperationModel;

        private bool _currentSpendInteractable;
        private readonly View.CurrencyOperationPanel _view;

        public event Action OnChangedCurrency;
        public event Action OnChangedSpendInteractable;

        public CurrencyOperationPanelPresenter(CurrencyCell currencyCell, CurrencyOperationModel currencyOperationModel,
            View.CurrencyOperationPanel currencyOperationPanel, Sprite icon)
        {
            Icon = icon;
            _currencyCell = currencyCell;
            _currencyOperationModel = currencyOperationModel;
            _view = currencyOperationPanel;
        }
        
        public string Currency => _currencyCell.Value.ToString();

        public bool SpendInteractable => _currencyCell.Value > 0;
        public Sprite Icon { get; }

        public View.CurrencyOperationPanel View => _view;

        public void Initialize()
        {
            _currencyCell.OnChanged += ChangeCurrency;
            ChangeCurrency();
        }
        
        public void Dispose() => 
            _currencyCell.OnChanged -= ChangeCurrency;

        private void ChangeCurrency()
        {
            OnChangedCurrency?.Invoke();

            ChangeSpendInteractable();
        }

        private void ChangeSpendInteractable()
        {
            if (_currentSpendInteractable != SpendInteractable) 
                OnChangedSpendInteractable?.Invoke();

            _currentSpendInteractable = SpendInteractable;
        }

        public void OnAddClicked() => 
            _currencyOperationModel.Add();

        public void OnSpendClicked() => 
            _currencyOperationModel.Spend();
    }
}