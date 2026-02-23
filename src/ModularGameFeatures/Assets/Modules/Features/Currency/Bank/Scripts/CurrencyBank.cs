using System;
using System.Collections;
using System.Collections.Generic;

namespace Modules.Features.Currency.Bank.Scripts
{
    [Serializable]
    public class CurrencyBank : IEnumerable<CurrencyCell>
    {
        private readonly Dictionary<CurrencyType, CurrencyCell> _cellsForward;

        public int Count => _cellsForward.Count;

        public CurrencyBank(IEnumerable<CurrencyAmount> cells)
        {
            if (cells == null)
                throw new ArgumentNullException(nameof(cells));

            _cellsForward = new Dictionary<CurrencyType, CurrencyCell>();

            foreach (var cell in cells)
            {
                if (_cellsForward.ContainsKey(cell.Type))
                    throw new InvalidOperationException($"Duplicate currency type {cell.Type}");

                _cellsForward.Add(cell.Type, new CurrencyCell(cell.Amount, cell.Type));
            }
        }

        /// <summary>
        /// Добавляет указанное количество каждой валюты без проверки достаточности.
        /// </summary>
        /// <param name="amountsToAdd">Список валют и количеств, которые нужно добавить.</param>
        public void AddCurrencies(IEnumerable<CurrencyAmount> amountsToAdd) =>
            Apply(amountsToAdd, (c, a) => c.Add(a));

        /// <summary>
        /// Списывает валюту, если хватает.
        /// </summary>
        /// <param name="cost">Список требуемых валют и их количеств.</param>
        /// <returns>Возвращает true, если списание выполнено, иначе false.</returns>
        public bool TrySpendCurrencies(IEnumerable<CurrencyAmount> cost)
        {
            if (!IsEnough(cost))
                return false;

            Apply(cost, (c, a) => c.Spend(a));

            return true;
        }

        /// <summary>
        /// Устанавливает новые значения валют для каждого указанного элемента.
        /// </summary>
        /// <param name="newAmounts">Список валют и их новых количеств.</param>
        public void SetCurrencies(IEnumerable<CurrencyAmount> newAmounts) =>
            Apply(newAmounts, (c, a) => c.Set(a));

        /// <summary>
        /// Получает объект валютной ячейки по указанному типу валюты.
        /// </summary>
        /// <param name="type">Тип валюты, ячейку которой нужно получить.</param>
        /// <returns>Возвращает объект <see cref="CurrencyCell"/> соответствующего типа.</returns>
        /// <exception cref="InvalidOperationException">Если указанная валюта не зарегистрирована.</exception>
        public CurrencyCell GetCell(CurrencyType type)
        {
            if (!_cellsForward.TryGetValue(type, out var cell))
                throw new InvalidOperationException($"Currency {type} not registered");

            return cell;
        }

        /// <summary>
        /// Проверяет, достаточно ли валюты для указанного списка.
        /// </summary>
        /// <param name="cost">Список валют и требуемых количеств для проверки.</param>
        /// <returns>Возвращает true, если всех валют хватает, иначе false.</returns>
        /// <exception cref="ArgumentNullException">Если передан null в параметр <paramref name="cost"/>.</exception>
        public bool IsEnough(IEnumerable<CurrencyAmount> cost)
        {
            if (cost == null)
                throw new ArgumentNullException(nameof(cost));

            foreach (var price in cost)
            {
                if (price.Amount >= GetCell(price.Type).Value)
                    return false;
            }

            return true;
        }

        public IEnumerator<CurrencyCell> GetEnumerator() =>
            _cellsForward.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        private void Apply(IEnumerable<CurrencyAmount> cost, Action<CurrencyCell, int> action)
        {
            if (cost == null)
                throw new ArgumentNullException(nameof(cost));

            foreach (var price in cost)
                action(GetCell(price.Type), price.Amount);
        }
    }
}