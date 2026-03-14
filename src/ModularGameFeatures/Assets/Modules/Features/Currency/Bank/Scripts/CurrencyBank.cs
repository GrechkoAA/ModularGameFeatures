using System;
using System.Collections;
using System.Collections.Generic;

namespace Modules.Features.Currency.Bank.Scripts
{
    /// <summary>
    /// Представляет банк валют, содержащий несколько <see cref="CurrencyCell"/>.
    /// Позволяет добавлять, списывать и устанавливать значения валют централизованно.
    /// </summary>
    [Serializable]
    public class CurrencyBank : IEnumerable<CurrencyCell>
    {
        /// <summary>
        /// Внутренний словарь для хранения ячеек по типу валюты.
        /// Ключ — <see cref="CurrencyType"/>, значение — соответствующая <see cref="CurrencyCell"/>.
        /// </summary>
        private readonly Dictionary<CurrencyType, CurrencyCell> _cellsForward;

        /// <summary>
        /// Количество зарегистрированных валют в банке.
        /// </summary>
        public int Count => _cellsForward.Count;

        /// <summary>
        /// Инициализирует банк валют, создавая ячейки на основе переданного списка.
        /// </summary>
        /// <param name="cells">Список валют и их начальных количеств.</param>
        /// <exception cref="ArgumentNullException">Если <paramref name="cells"/> равен null.</exception>
        /// <exception cref="InvalidOperationException">Если список содержит дублирующийся тип валюты.</exception>
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
        /// Добавляет указанное количество валют для каждой ячейки.
        /// Количество валют добавляется без проверки достаточности.
        /// </summary>
        /// <param name="amountsToAdd">Список валют и количеств для добавления.</param>
        public void AddCurrencies(IEnumerable<CurrencyAmount> amountsToAdd) =>
            Apply(amountsToAdd, (c, a) => c.Add(a));

        /// <summary>
        /// Пытается списать указанные суммы валют.
        /// </summary>
        /// <param name="cost">Список валют и требуемых количеств для списания.</param>
        /// <returns>Возвращает <c>true</c>, если списание выполнено успешно, иначе <c>false</c>.</returns>
        public bool TrySpendCurrencies(IEnumerable<CurrencyAmount> cost)
        {
            if (!IsEnough(cost))
                return false;

            Apply(cost, (c, a) => c.Spend(a));

            return true;
        }

        /// <summary>
        /// Устанавливает новое значение для каждой валютной ячейки.
        /// </summary>
        /// <param name="newAmounts">Список валют и новых значений.</param>
        public void SetCurrencies(IEnumerable<CurrencyAmount> newAmounts) =>
            Apply(newAmounts, (c, a) => c.Set(a));

        /// <summary>
        /// Получает валютную ячейку по указанному типу валюты.
        /// </summary>
        /// <param name="type">Тип валюты.</param>
        /// <returns>Возвращает объект <see cref="CurrencyCell"/> соответствующего типа.</returns>
        /// <exception cref="InvalidOperationException">Если валюта с указанным типом не зарегистрирована.</exception>
        public CurrencyCell GetCell(CurrencyType type)
        {
            if (!_cellsForward.TryGetValue(type, out var cell))
                throw new InvalidOperationException($"Currency {type} not registered");

            return cell;
        }

        /// <summary>
        /// Проверяет, достаточно ли каждой валюты для выполнения указанного списка.
        /// </summary>
        /// <param name="cost">Список валют и требуемых количеств.</param>
        /// <returns><c>true</c>, если валют хватает для всех элементов списка, иначе <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Если <paramref name="cost"/> равен null.</exception>
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

        /// <summary>
        /// Возвращает перечислитель по всем валютным ячейкам банка.
        /// </summary>
        public IEnumerator<CurrencyCell> GetEnumerator() =>
            _cellsForward.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <summary>
        /// Применяет указанное действие ко всем элементам списка валют.
        /// </summary>
        /// <param name="cost">Список валют и количеств.</param>
        /// <param name="action">Действие для применения к каждой ячейке.</param>
        /// <exception cref="ArgumentNullException">Если <paramref name="cost"/> равен null.</exception>
        private void Apply(IEnumerable<CurrencyAmount> cost, Action<CurrencyCell, int> action)
        {
            if (cost == null)
                throw new ArgumentNullException(nameof(cost));

            foreach (var price in cost)
                action(GetCell(price.Type), price.Amount);
        }
    }
}