using System;
using UnityEngine;

namespace Examples.Features.Currency.Bank.Scripts.UI.Currency.Presenters.CurrencyOperationPanel
{
    public interface ICurrencyOperationPanelPresenter
    {
        string Currency { get; }
        bool SpendInteractable { get; }
        Sprite Icon { get; }
        void OnAddClicked();
        void OnSpendClicked();

        event Action OnChangedSpendInteractable;
        event Action OnChangedCurrency;
    }
}