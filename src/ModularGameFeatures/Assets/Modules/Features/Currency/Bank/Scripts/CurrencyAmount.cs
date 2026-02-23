using System;

namespace Modules.Features.Currency.Bank.Scripts
{
    [Serializable]
    public readonly struct CurrencyAmount
    {
        public CurrencyAmount(CurrencyType type, int amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be negative");

            Type = type;
            Amount = amount;
        }

        public CurrencyType Type { get; }

        public int Amount { get; }

        public override string ToString() 
            => $"{Type}: {Amount}";
    }
}