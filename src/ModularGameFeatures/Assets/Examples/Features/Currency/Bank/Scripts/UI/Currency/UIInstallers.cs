using Examples.Features.Currency.Bank.Scripts.UI.Currency.Presenters.CreatorCurrencyOperationPanel;
using Examples.Features.Currency.Bank.Scripts.UI.Currency.StaticData;
using Modules.Features.Currency.Bank.Scripts;
using UnityEngine;
using Zenject;

namespace Examples.Features.Currency.Bank.Scripts.UI.Currency
{
    public class UIInstallers : MonoInstaller
    {
        [SerializeField] private CurrencyOperationPanelCreator _currencyOperationPanelCreator;
        [SerializeField] private CurrencyAmountStaticData _currencyAmountStaticData;
        [SerializeField] private CurrencyIconStaticData _currencyIconStaticData;
       
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CurrencyBank>().AsSingle().WithArguments(_currencyAmountStaticData.GetCells()).NonLazy();
            Container.BindInterfacesAndSelfTo<CurrencyBankPresenter>().AsSingle().WithArguments(_currencyOperationPanelCreator, _currencyIconStaticData).NonLazy();
        }
    }
}