using System;
using System.Collections.Generic;
using Examples.Features.Currency.Bank.Scripts.System;
using Examples.Features.Currency.Bank.Scripts.UI.Currency.Presenters.CurrencyOperationPanel;
using Examples.Features.Currency.Bank.Scripts.UI.Currency.StaticData;
using Modules.Features.Currency.Bank.Scripts;
using UnityEngine;
using Zenject;

namespace Examples.Features.Currency.Bank.Scripts.UI.Currency.Presenters.CreatorCurrencyOperationPanel
{
    public class CurrencyBankPresenter : IInitializable, IDisposable
    {
        private readonly CurrencyOperationPanelCreator _currencyOperationPanel;
        private readonly CurrencyBank _currencyBank;
        private readonly CurrencyIconStaticData _currencyIconStaticData;

        private readonly List<CurrencyOperationPanelPresenter> _currencyPresenters;

        public CurrencyBankPresenter(CurrencyBank currencyBank, CurrencyOperationPanelCreator currencyOperationPanel, CurrencyIconStaticData currencyIconStaticData)
        {
            _currencyIconStaticData = currencyIconStaticData;
            _currencyBank = currencyBank;
            _currencyOperationPanel = currencyOperationPanel;

            _currencyPresenters = new List<CurrencyOperationPanelPresenter>(_currencyBank.Count);
        }

        public void Initialize()
        {
            foreach (CurrencyCell currencyCell in _currencyBank)
            {
                CurrencyOperationModel currencyOperationModel = new(currencyCell);
                View.CurrencyOperationPanel instanceCurrencyView = _currencyOperationPanel.Spawn();
                CurrencyOperationPanelPresenter currencyPresenter = new(currencyCell, currencyOperationModel, instanceCurrencyView, GetIcon(currencyCell.Type));

                instanceCurrencyView.Construct(currencyPresenter);
                instanceCurrencyView.Initialize();
                currencyPresenter.Initialize();

                _currencyPresenters.Add(currencyPresenter);
            }
        }

        private Sprite GetIcon(CurrencyType currencyType)
        {
            CurrencyIconSettings iconSettings = _currencyIconStaticData.GetCurrencySettings(currencyType);

            if (iconSettings.Icon == null)
            {
                Debug.LogError($"No icon found for currency {currencyType}");
                
                return null;
            }

            return iconSettings.Icon;
        }

        public void Dispose()
        {
            foreach (var currencyPresenter in _currencyPresenters)
            {
                currencyPresenter.Dispose();
                currencyPresenter.View.Dispose();
                
                if (currencyPresenter.View) 
                    _currencyOperationPanel.Unspawn(currencyPresenter.View);
            }
        }
    }
}