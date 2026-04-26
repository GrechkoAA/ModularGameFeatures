using System;
using System.Collections.Generic;
using System.Linq;
using Modules.Features.Currency.Bank.Scripts;
using NUnit.Framework;

namespace Modules.Features.Currency.Bank.Tests
{
    public class CurrencyCellTests
    {
        private static CurrencyCell GetAnyValidCurrency(int initialAmount)
        {
            var currencyType = Enum.GetValues(typeof(CurrencyType))
                .Cast<CurrencyType>()
                .First(x => x != CurrencyType.None);

            return new CurrencyCell(initialAmount, currencyType);
        }

        #region Constructor

        [TestCase(-10, 0)]
        [TestCase(0, 0)]
        [TestCase(10, 10)]
        public void Constructor_ShouldClampInitialValue(int initial, int expected)
        {
            // Arrange
            CurrencyCell sut = GetAnyValidCurrency(initial);

            // Assert
            Assert.AreEqual(expected, sut.Value);
        }

        #endregion

        #region Add

        [TestCaseSource(nameof(AddCases))]
        public void Add_ShouldWorkCorrectly(int initialAmount, int addAmount, int expected)
        {
            // Arrange
            CurrencyCell sut = GetAnyValidCurrency(initialAmount);

            // Act
            sut.Add(addAmount);

            // Assert
            Assert.AreEqual(expected, sut.Value);
        }

        [TestCaseSource(nameof(AddCases))]
        public void Add_ShouldTriggerAddedEvent(int initialAmount, int addAmount, int expected)
        {
            // Arrange
            CurrencyCell sut = GetAnyValidCurrency(initialAmount);
            int validInitialAmount = sut.Value;
            int previous = 0;
            int current = 0;

            // Act
            sut.OnAdded += (cur, prev) =>
            {
                current = cur;
                previous = prev;
            };

            sut.Add(addAmount);

            // Assert
            Assert.AreEqual(expected, current);
            Assert.AreEqual(validInitialAmount, previous);
        }

        private static IEnumerable<TestCaseData> AddCases()
        {
            // starting value = 0
            yield return new TestCaseData(0, 0, 0).SetName("WhenStartingZero_AndAddingZero_ThenValueRemainsZero");
            yield return new TestCaseData(0, 5, 5).SetName(
                "WhenStartingZero_AndAddingPositive_ThenValueBecomesAddedAmount");
            yield return new TestCaseData(0, -5, 0).SetName("WhenStartingZero_AndAddingNegative_ThenValueRemainsZero");

            // starting value > 0
            yield return new TestCaseData(10, 0, 10).SetName("WhenStartingPositive_AndAddingZero_ThenValueRemainsSame");
            yield return new TestCaseData(1, 1, 2).SetName("WhenStartingPositive_AndAddingPositive_ThenValueIncreases");
            yield return new TestCaseData(15, 15, 30).SetName(
                "WhenStartingPositive_AndAddingPositive_ThenValueIncreasesLarge");
            yield return new TestCaseData(10, -5, 10).SetName(
                "WhenStartingPositive_AndAddingNegative_ThenValueRemainsSame");

            // starting value < 0
            yield return new TestCaseData(-10, 0, 0).SetName("WhenStartingNegative_AndAddingZero_ThenValueBecomesZero");
            yield return new TestCaseData(-10, 5, 5).SetName(
                "WhenStartingNegative_AndAddingPositive_ThenValueBecomesAddedAmount");
            yield return new TestCaseData(-100, 100, 100).SetName(
                "WhenStartingNegative_AndAddingPositive_ThenValueBecomesAddedAmountLarge");
            yield return new TestCaseData(-10, -5, 0).SetName(
                "WhenStartingNegative_AndAddingNegative_ThenValueBecomesZero");
            yield return new TestCaseData(-100, -100, 0).SetName(
                "WhenStartingNegative_AndAddingNegative_ThenValueBecomesZeroLarge");
        }

        #endregion

        #region Spend

        [TestCaseSource(nameof(SpendCases))]
        public void Spend_ShouldWorkCorrectly(int initialAmount, int spendAmount, int expected)
        {
            // Arrange
            CurrencyCell sut = GetAnyValidCurrency(initialAmount);

            // Act
            sut.Spend(spendAmount);

            // Assert
            Assert.AreEqual(expected, sut.Value);
        }

        [TestCaseSource(nameof(SpendCases))]
        public void Spend_ShouldTriggerSpendEvent(int initialAmount, int spendAmount, int expected)
        {
            // Arrange
            CurrencyCell sut = GetAnyValidCurrency(initialAmount);
            int validInitialAmount = sut.Value;
            int previous = 0;
            int current = 0;

            // Act
            sut.OnSpent += (cur, prev) =>
            {
                current = cur;
                previous = prev;
            };

            sut.Spend(spendAmount);

            // Assert
            Assert.AreEqual(expected, current);
            Assert.AreEqual(validInitialAmount, previous);
        }

        private static IEnumerable<TestCaseData> SpendCases()
        {
            // starting value = 0
            yield return new TestCaseData(0, 0, 0).SetName("WhenStartingZero_AndSpendingZero_ThenValueRemainsZero");
            yield return new TestCaseData(0, 5, 0).SetName("WhenStartingZero_AndSpendingPositive_ThenValueRemainsZero");
            yield return
                new TestCaseData(0, -5, 0).SetName("WhenStartingZero_AndSpendingNegative_ThenValueRemainsZero");

            // starting value > 0
            yield return new TestCaseData(10, 0, 10).SetName(
                "WhenStartingPositive_AndSpendingZero_ThenValueRemainsSame");
            yield return new TestCaseData(10, 5, 5).SetName(
                "WhenStartingPositive_AndSpendingLessThanValue_ThenValueDecreases");
            yield return new TestCaseData(10, 10, 0).SetName(
                "WhenStartingPositive_AndSpendingExactValue_ThenValueBecomesZero");
            yield return new TestCaseData(10, 15, 0).SetName(
                "WhenStartingPositive_AndSpendingMoreThanValue_ThenValueBecomesZero");
            yield return new TestCaseData(10, -5, 10).SetName(
                "WhenStartingPositive_AndSpendingNegative_ThenValueRemainsSame");

            // starting value < 0
            yield return new TestCaseData(-10, 0, 0).SetName(
                "WhenStartingNegative_AndSpendingZero_ThenValueBecomesZero");
            yield return new TestCaseData(-10, 5, 0).SetName(
                "WhenStartingNegative_AndSpendingPositive_ThenValueBecomesZero");
            yield return new TestCaseData(-10, -5, 0).SetName(
                "WhenStartingNegative_AndSpendingNegative_ThenValueBecomesZero");
        }

        #endregion

        #region Set

        [TestCaseSource(nameof(SetCases))]
        public void Set_ShouldWorkCorrectly(int initialAmount, int setValue, int expected)
        {
            // Arrange
            CurrencyCell sut = GetAnyValidCurrency(initialAmount);

            // Act
            sut.Set(setValue);

            // Assert
            Assert.AreEqual(expected, sut.Value);
        }

        [TestCaseSource(nameof(SetCases))]
        public void Set_ShouldTriggerSetEvent(int initialAmount, int setValue, int expected)
        {
            // Arrange
            CurrencyCell sut = GetAnyValidCurrency(initialAmount);
            int validInitialAmount = sut.Value;
            int previous = 0;
            int current = 0;

            // Act
            sut.OnSet += (cur, prev) =>
            {
                current = cur;
                previous = prev;
            };

            sut.Set(setValue);

            // Assert
            Assert.AreEqual(expected, current);
            Assert.AreEqual(validInitialAmount, previous);
        }

        private static IEnumerable<TestCaseData> SetCases()
        {
            // starting value = 0
            yield return new TestCaseData(0, 0, 0).SetName("WhenStartingZero_AndSettingZero_ThenValueRemainsZero");
            yield return new TestCaseData(0, 5, 5).SetName(
                "WhenStartingZero_AndSettingPositive_ThenValueBecomesSetAmount");
            yield return new TestCaseData(0, -5, 0).SetName("WhenStartingZero_AndSettingNegative_ThenValueBecomesZero");

            // starting value > 0
            yield return new TestCaseData(10, 0, 0).SetName("WhenStartingPositive_AndSettingZero_ThenValueBecomesZero");
            yield return new TestCaseData(10, 5, 5).SetName(
                "WhenStartingPositive_AndSettingPositive_ThenValueBecomesSetAmount");
            yield return new TestCaseData(10, 20, 20).SetName(
                "WhenStartingPositive_AndSettingLargerPositive_ThenValueBecomesSetAmount");
            yield return new TestCaseData(10, -5, 0).SetName(
                "WhenStartingPositive_AndSettingNegative_ThenValueBecomesZero");

            // starting value < 0
            yield return
                new TestCaseData(-10, 0, 0).SetName("WhenStartingNegative_AndSettingZero_ThenValueBecomesZero");
            yield return new TestCaseData(-10, 5, 5).SetName(
                "WhenStartingNegative_AndSettingPositive_ThenValueBecomesSetAmount");
            yield return new TestCaseData(-10, -5, 0).SetName(
                "WhenStartingNegative_AndSettingNegative_ThenValueBecomesZero");
        }

        #endregion

        #region ChangeEvent

        [TestCaseSource(nameof(ChangeMethods))]
        public void Methods_ShouldTriggerChangedEvent(int initialAmount, Action<CurrencyCell> operation)
        {
            // Arrange
            CurrencyCell sut = GetAnyValidCurrency(initialAmount);
            bool triggered = false;
            sut.OnChanged += () => triggered = true;

            // Act
            operation(sut);

            // Assert
            Assert.IsTrue(triggered, $"OnChanged was not triggered for initialAmount={initialAmount}");
        }

        private static IEnumerable<TestCaseData> ChangeMethods()
        {
            int[] initialValues = { -10, 0, 10 };
            int[] testValues = { -5, 0, 5, 15 };

            // Add
            foreach (int initial in initialValues)
            {
                foreach (int value in testValues)
                {
                    yield return new TestCaseData(initial, new Action<CurrencyCell>(c => c.Add(value)))
                        .SetName($"Add_Initial{initial}_Value{value}_ShouldTriggerChangedEvent");
                }
            }

            // Spend
            foreach (int initial in initialValues)
            {
                foreach (int value in testValues)
                {
                    yield return new TestCaseData(initial, new Action<CurrencyCell>(c => c.Spend(value)))
                        .SetName($"Spend_Initial{initial}_Value{value}_ShouldTriggerChangedEvent");
                }
            }

            // Set
            foreach (int initial in initialValues)
            {
                foreach (int value in testValues)
                {
                    yield return new TestCaseData(initial, new Action<CurrencyCell>(c => c.Set(value)))
                        .SetName($"Set_Initial{initial}_Value{value}_ShouldTriggerChangedEvent");
                }
            }
        }

        #endregion
    }
}