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

        /// <summary>
        /// Создаёт новый экземпляр валюты с указанным количеством.
        /// </summary>
        /// <param name="newAmount">Новое количество валюты.</param>
        /// <returns>Новый объект CurrencyAmount с тем же типом и указанным количеством.</returns>
        public CurrencyAmount WithAmount(int newAmount) =>
            new(Type, newAmount);

        public override string ToString() 
            => $"{Type}: {Amount}";
    }
}