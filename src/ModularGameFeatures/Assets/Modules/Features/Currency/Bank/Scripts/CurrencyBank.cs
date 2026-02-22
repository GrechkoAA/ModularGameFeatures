using System;
using System.Collections;
using System.Collections.Generic;

namespace Modules.Features.Currency.Bank.Scripts
{
    [Serializable]
    public class CurrencyBank : IEnumerable<CurrencyCell>
    {
        //TODO реализация двунаправленного словаря
        private readonly Dictionary<CurrencyType, CurrencyCell> _cellsForward;
        private readonly Dictionary<CurrencyCell, CurrencyType> _cellsReverse;

        public int Count => _cellsForward.Count;

        public CurrencyBank(IEnumerable<(CurrencyType currencyType, CurrencyCell currencyCell)> cells)
        {
            if (cells == null)
                throw new ArgumentNullException(nameof(cells));

            _cellsForward = new Dictionary<CurrencyType, CurrencyCell>();
            _cellsReverse = new Dictionary<CurrencyCell, CurrencyType>();

            foreach (var cell in cells)
            {
                _cellsForward.Add(cell.currencyType, cell.currencyCell);
                _cellsReverse.Add(cell.currencyCell, cell.currencyType);
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

        public CurrencyType GetType(CurrencyCell cell)
        {
            if (cell == null)
                throw new ArgumentNullException(nameof(cell));

            if (!_cellsReverse.TryGetValue(cell, out var cellType))
                throw new InvalidOperationException($"Currency {cell} not registered");

            return cellType;
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
                if (GetCell(price.Type).Value < price.Amount)
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