using System;

namespace Modules.Features.Currency.Bank.Scripts
{
    /// <summary>
    /// Представляет отдельную ячейку валюты.
    /// Хранит текущее количество валюты и предоставляет методы для управления значением.
    /// <para/>
    /// Значение валюты никогда не может быть меньше 0.
    /// Любые отрицательные входные значения автоматически приводятся к 0.
    /// <para/>
    /// Класс предоставляет события, позволяющие отслеживать изменения значения:
    /// <list type="bullet">
    /// <item><description><see cref="OnAdded"/> — вызывается после добавления валюты.</description></item>
    /// <item><description><see cref="OnSpent"/> — вызывается после списания валюты.</description></item>
    /// <item><description><see cref="OnSet"/> — вызывается после прямой установки значения.</description></item>
    /// <item><description><see cref="OnChanged"/> — вызывается после любого изменения значения.</description></item>
    /// </list>
    /// </summary>
    public class CurrencyCell
    {
        /// <summary>
        /// Текущее количество валюты.
        /// Значение всегда гарантированно больше либо равно 0.
        /// </summary>
        public int Value { get; private set; }

        /// <summary>
        /// Тип валюты, к которой относится данная ячейка.
        /// </summary>
        public readonly CurrencyType Type;

        /// <summary>
        /// Событие вызывается после добавления валюты.
        /// </summary>
        /// <remarks>
        /// Передаёт:
        /// <list type="bullet">
        /// <item><description>Новое значение валюты.</description></item>
        /// <item><description>Предыдущее значение валюты.</description></item>
        /// </list>
        /// </remarks>
        public event Action<int, int> OnAdded;

        /// <summary>
        /// Событие вызывается после списания валюты.
        /// </summary>
        /// <remarks>
        /// Передаёт:
        /// <list type="bullet">
        /// <item><description>Новое значение валюты.</description></item>
        /// <item><description>Предыдущее значение валюты.</description></item>
        /// </list>
        /// </remarks>
        public event Action<int, int> OnSpent;

        /// <summary>
        /// Событие вызывается после прямой установки значения валюты.
        /// </summary>
        /// <remarks>
        /// Передаёт:
        /// <list type="bullet">
        /// <item><description>Новое значение валюты.</description></item>
        /// <item><description>Предыдущее значение валюты.</description></item>
        /// </list>
        /// </remarks>
        public event Action<int, int> OnSet;

        /// <summary>
        /// Событие вызывается после любого изменения значения валюты.
        /// </summary>
        /// <remarks>
        /// Используется для случаев, когда не важно каким именно методом было изменено значение.
        /// </remarks>
        public event Action OnChanged;


        /// <summary>
        /// Создаёт новую ячейку валюты.
        /// </summary>
        /// <param name="value">
        /// Начальное значение валюты.
        /// Если передано отрицательное значение, оно будет автоматически приведено к 0.
        /// </param>
        /// <param name="type">Тип валюты.</param>
        internal CurrencyCell(int value, CurrencyType type)
        {
            int validValue = Math.Max(0, value);

            Value = validValue;
            Type = type;
        }

        /// <summary>
        /// Добавляет указанное количество валюты к текущему значению.
        /// </summary>
        /// <param name="amount">
        /// Количество валюты для добавления.
        /// Отрицательное значение будет интерпретировано как 0.
        /// </param>
        /// <remarks>
        /// После изменения вызываются события:
        /// <see cref="OnAdded"/> и <see cref="OnChanged"/>.
        /// </remarks>
        public void Add(int amount)
        {
            int validAmount = Math.Max(0, amount);
            int previous = Value;

            Value += validAmount;

            OnAdded?.Invoke(Value, previous);
            OnChanged?.Invoke();
        }

        /// <summary>
        /// Списывает указанное количество валюты из текущего значения.
        /// </summary>
        /// <param name="amount">
        /// Количество валюты для списания.
        /// Отрицательное значение будет интерпретировано как 0.
        /// </param>
        /// <remarks>
        /// Значение валюты никогда не станет отрицательным.
        /// Если списываемая сумма больше текущего значения, результат будет 0.
        /// <para/>
        /// После изменения вызываются события:
        /// <see cref="OnSpent"/> и <see cref="OnChanged"/>.
        /// </remarks>
        public void Spend(int amount)
        {
            int validAmount = Math.Max(0, amount);
            int previous = Value;

            Value = Math.Max(0, Value - validAmount);

            OnSpent?.Invoke(Value, previous);
            OnChanged?.Invoke();
        }

        /// <summary>
        /// Устанавливает новое значение валюты.
        /// </summary>
        /// <param name="value">
        /// Новое значение валюты.
        /// Если передано отрицательное значение, оно будет приведено к 0.
        /// </param>
        /// <remarks>
        /// После изменения вызываются события:
        /// <see cref="OnSet"/> и <see cref="OnChanged"/>.
        /// </remarks>
        public void Set(int value)
        {
            int validValue = Math.Max(0, value);
            int previous = Value;

            Value = validValue;

            OnSet?.Invoke(Value, previous);
            OnChanged?.Invoke();
        }
    }
}