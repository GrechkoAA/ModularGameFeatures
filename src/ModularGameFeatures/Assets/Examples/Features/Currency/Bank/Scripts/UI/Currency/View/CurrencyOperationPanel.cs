using Examples.Features.Currency.Bank.Scripts.UI.Currency.Presenters.CurrencyOperationPanel;
using UnityEngine;

namespace Examples.Features.Currency.Bank.Scripts.UI.Currency.View
{
    public class CurrencyOperationPanel : MonoBehaviour
    {
        [SerializeField] private CurrencyView _currencyView;
        [SerializeField] private CurrencyOperationView _plusView;
        [SerializeField] private CurrencyOperationView _minusView;
        
        private ICurrencyOperationPanelPresenter _presenter;

        public void Construct(ICurrencyOperationPanelPresenter presenter) => 
            _presenter = presenter;

        public void Initialize()
        {
            _plusView.OnClicked += AddOperation;
            _minusView.OnClicked += SpendOperation;
            _presenter.OnChangedCurrency += ChangeCurrency;
            _presenter.OnChangedSpendInteractable += SetSpendInteractable;
            
            SetIcon();
            SetSpendInteractable();
        }

        public void Dispose()
        {
            _plusView.OnClicked -= AddOperation;
            _minusView.OnClicked -= SpendOperation;
            _presenter.OnChangedCurrency -= ChangeCurrency;
            _presenter.OnChangedSpendInteractable -= SetSpendInteractable;
        }

        private void ChangeCurrency() => 
            _currencyView.Set(_presenter.Currency);

        private void SetSpendInteractable() =>
            _minusView.SetInteractable(_presenter.SpendInteractable);

        private void SetIcon() => 
            _currencyView.SetIcon(_presenter.Icon);

        private void AddOperation() => 
            _presenter.OnAddClicked();

        private void SpendOperation() => 
            _presenter.OnSpendClicked();
    }
}