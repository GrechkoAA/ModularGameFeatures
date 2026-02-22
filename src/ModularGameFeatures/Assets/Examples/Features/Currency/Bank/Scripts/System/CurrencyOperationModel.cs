using Modules.Features.Currency.Bank.Scripts;

namespace Examples.Features.Currency.Bank.Scripts.System
{
    public class CurrencyOperationModel
    {
        private readonly CurrencyCell _currencyCell;

        public CurrencyOperationModel(CurrencyCell currencyCell) => 
            _currencyCell = currencyCell;

        public void Add() => 
            _currencyCell.Add(1);

        public void Spend() => 
            _currencyCell.Spend(1);
    }
}