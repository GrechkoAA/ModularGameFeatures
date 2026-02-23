using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Modules.Features.CurrencyBank.Tests")]

namespace Modules.Features.Currency.Bank.Scripts
{
    // <summary>
    /// Представляет отдельную ячейку валюты, которая хранит своё текущее значение и управляет им.
    /// Значение валюты всегда >= 0.  
    /// <para/>
    /// Поддерживаются события для отслеживания изменений:
    /// <list type="bullet">
    /// <item><description><see cref="OnAdded"/> — после добавления валюты.</description></item>
    /// <item><description><see cref="OnSpent"/> — после списания валюты.</description></item>
    /// <item><description><see cref="OnSet"/> — после прямой установки значения.</description></item>
    /// <item><description><see cref="OnChanged"/> — после любого изменения значения.</description></item>
    /// </list>
    /// </summary>
    public class CurrencyCell
    {
        public int Value { get; private set; }

        public readonly CurrencyType Type;

        /// <summary>
        /// Событие срабатывает после добавления валюты. Передаёт новое и предыдущее значение.
        /// </summary>
        public event Action<int, int> OnAdded;
        
        /// <summary>
        /// Событие срабатывает после списания валюты. Передаёт новое и предыдущее значение.
        /// </summary>
        public event Action<int, int> OnSpent;
        
        /// <summary>
        /// Событие срабатывает после прямой установки значения валюты. Передаёт новое и предыдущее значение.
        /// </summary>
        public event Action<int, int> OnSet;
        
        /// <summary>
        /// Событие срабатывает после любого изменения значения валюты (без параметров).
        /// </summary>
        public event Action OnChanged;

        internal CurrencyCell(int value, CurrencyType type)
        {
            Value = Math.Max(0, value);
            Type = type;
        }

        /// <summary>
        /// Добавляет указанное количество валюты.
        /// </summary>
        /// <param name="amount">Количество, которое нужно добавить.</param>
        /// <remarks>
        /// После изменения срабатывают события <see cref="OnAdded"/> и <see cref="OnChanged"/>.
        /// </remarks>
        public void Add(int amount)
        {
            int previous = Value;
            Value += amount;
            
            OnAdded?.Invoke(Value, previous);
            OnChanged?.Invoke();
        }

        /// <summary>
        /// Списывает указанное количество валюты.
        /// </summary>
        /// <param name="amount">Количество, которое нужно списать.</param>
        /// <remarks>
        /// Значение никогда не станет отрицательным.
        /// После изменения срабатывают события <see cref="OnSpent"/> и <see cref="OnChanged"/>.
        /// </remarks>
        public void Spend(int amount)
        {
            int previous = Value;
            Value = Math.Max(0, Value - amount);

            OnSpent?.Invoke(Value, previous);
            OnChanged?.Invoke();
        }

        /// <summary>
        /// Устанавливает новое значение валюты.
        /// </summary>
        /// <param name="value">Новое значение валюты. Если меньше 0, устанавливается 0.</param>
        /// <remarks>
        /// После изменения срабатывают события <see cref="OnSet"/> и <see cref="OnChanged"/>.
        /// </remarks>
        public void Set(int value)
        {
            int previous = Value;
            Value = value;

            if (value < 0)
                Value = 0;

            OnSet?.Invoke(Value, previous);
            OnChanged?.Invoke();
        }
    }
}