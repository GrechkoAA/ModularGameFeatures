using System;
using System.Collections.Generic;
using Modules.Features.Currency.Bank.Scripts;
using NUnit.Framework;

namespace Modules.Features.Currency.Bank.Tests
{
    public class CurrencyCellTests
    {
        #region Constructor
        
        [TestCase(-10, 0)]
        [TestCase(0, 0)]
        [TestCase(10, 10)]
        public void Constructor_ShouldClampInitialValue(int initial, int expected)
        {
            CurrencyCell currency = new(initial, CurrencyType.None);

            // Assert
            Assert.AreEqual(expected, currency.Value);
        }
        
        #endregion
        
        #region Add

        [TestCaseSource(nameof(AddCases))]
        public void Add_ShouldWorkCorrectly(int initialAmount, int addAmount, int expected)
        {
            // Arrange
            CurrencyCell currencyGold = new(initialAmount, CurrencyType.None);

            // Act
            currencyGold.Add(addAmount);

            // Assert
            Assert.AreEqual(expected, currencyGold.Value);
        }
        
        [TestCaseSource(nameof(AddCases))]
        public void Add_ShouldTriggerAddedEvent(int initialAmount, int addAmount, int expected)
        {
            // Arrange
            CurrencyCell currencyGold = new(initialAmount, CurrencyType.None);
            
            // Act
            int validInitialAmount = currencyGold.Value;
            int previous = 0;
            int current = 0;

            currencyGold.OnAdded += (cur, prev) =>
            {
                current = cur;
                previous = prev;
            };

            currencyGold.Add(addAmount);
            
            // Assert
            Assert.AreEqual(expected, current);
            Assert.AreEqual(validInitialAmount, previous);
        }

        private static IEnumerable<TestCaseData> AddCases()
        {
            // starting value = 0
            yield return new TestCaseData(0, 0, 0).SetName("WhenStartingZeroAndAddingZero_ThenValueRemainsZero");
            yield return new TestCaseData(0, 5, 5).SetName("WhenStartingZeroAndAddingPositive_ThenValueBecomesAddedAmount");
            yield return new TestCaseData(0, -5, 0).SetName("WhenStartingZeroAndAddingNegative_ThenValueRemainsZero");

            // starting value > 0
            yield return new TestCaseData(10, 0, 10).SetName("WhenStartingPositiveAndAddingZero_ThenValueRemainsSame");
            yield return new TestCaseData(1, 1, 2).SetName("WhenStartingPositiveAndAddingPositive_ThenValueIncreases");
            yield return new TestCaseData(15, 15, 30).SetName("WhenStartingPositiveAndAddingPositive_ThenValueIncreasesLarge");
            yield return new TestCaseData(10, -5, 10).SetName("WhenStartingPositiveAndAddingNegative_ThenValueRemainsSame");

            // starting value < 0
            yield return new TestCaseData(-10, 0, 0).SetName("WhenStartingNegativeAndAddingZero_ThenValueBecomesZero");
            yield return new TestCaseData(-10, 5, 5).SetName("WhenStartingNegativeAndAddingPositive_ThenValueBecomesAddedAmount");
            yield return new TestCaseData(-100, 100, 100).SetName("WhenStartingNegativeAndAddingPositive_ThenValueBecomesAddedAmountLarge");
            yield return new TestCaseData(-10, -5, 0).SetName("WhenStartingNegativeAndAddingNegative_ThenValueBecomesZero");
            yield return new TestCaseData(-100, -100, 0).SetName("WhenStartingNegativeAndAddingNegative_ThenValueBecomesZeroLarge");
        }

        #endregion

        #region Spend

        [TestCaseSource(nameof(SpendCases))]
        public void Spend_ShouldWorkCorrectly(int initialAmount, int spendAmount, int expected)
        {
            // Arrange
            CurrencyCell currencyGold = new(initialAmount, CurrencyType.None);

            // Act
            currencyGold.Spend(spendAmount);

            // Assert
            Assert.AreEqual(expected, currencyGold.Value);
        }
        
        [TestCaseSource(nameof(SpendCases))]
        public void Spend_ShouldTriggerSpendEvent(int initialAmount, int spendAmount, int expected)
        {
            // Arrange
            CurrencyCell currencyGold = new(initialAmount, CurrencyType.None);
            
            // Act
            int validInitialAmount = currencyGold.Value;
            int previous = 0;
            int current = 0;

            currencyGold.OnSpent += (cur, prev) =>
            {
                current = cur;
                previous = prev;
            };

            currencyGold.Spend(spendAmount);
            
            // Assert
            Assert.AreEqual(expected, current);
            Assert.AreEqual(validInitialAmount, previous);
        }

        private static IEnumerable<TestCaseData> SpendCases()
        {
            // starting value = 0
            yield return new TestCaseData(0, 0, 0).SetName("WhenStartingZeroAndSpendingZero_ThenValueRemainsZero");
            yield return new TestCaseData(0, 5, 0).SetName("WhenStartingZeroAndSpendingPositive_ThenValueRemainsZero");
            yield return new TestCaseData(0, -5, 0).SetName("WhenStartingZeroAndSpendingNegative_ThenValueRemainsZero");
            
            // starting value > 0
            yield return new TestCaseData(10, 0, 10).SetName("WhenStartingPositiveAndSpendingZero_ThenValueRemainsSame");
            yield return new TestCaseData(10, 5, 5).SetName("WhenStartingPositiveAndSpendingLessThanValue_ThenValueDecreases");
            yield return new TestCaseData(10, 10, 0).SetName("WhenStartingPositiveAndSpendingExactValue_ThenValueBecomesZero");
            yield return new TestCaseData(10, 15, 0).SetName("WhenStartingPositiveAndSpendingMoreThanValue_ThenValueBecomesZero");
            yield return new TestCaseData(10, -5, 10).SetName("WhenStartingPositiveAndSpendingNegative_ThenValueRemainsSame");
            
            // starting value < 0
            yield return new TestCaseData(-10, 0, 0).SetName("WhenStartingNegativeAndSpendingZero_ThenValueBecomesZero");
            yield return new TestCaseData(-10, 5, 0).SetName("WhenStartingNegativeAndSpendingPositive_ThenValueBecomesZero");
            yield return new TestCaseData(-10, -5, 0).SetName("WhenStartingNegativeAndSpendingNegative_ThenValueBecomesZero");
        }
        
        #endregion

        #region Set
        
        [TestCaseSource(nameof(SetCases))]
        public void Set_ShouldWorkCorrectly(int initialAmount, int setValue, int expected)
        {
            // Arrange
            CurrencyCell currencyGold = new(initialAmount, CurrencyType.None);

            // Act
            currencyGold.Set(setValue);

            // Assert
            Assert.AreEqual(expected, currencyGold.Value);
        }

        [TestCaseSource(nameof(SetCases))]
        public void Set_ShouldTriggerSetEvent(int initialAmount, int setValue, int expected)
        {
            // Arrange
            CurrencyCell currencyGold = new(initialAmount, CurrencyType.None);
            
            // Act
            int validInitialAmount = currencyGold.Value;
            int previous = 0;
            int current = 0;

            currencyGold.OnSet += (cur, prev) =>
            {
                current = cur;
                previous = prev;
            };

            currencyGold.Set(setValue);
            
            // Assert
            Assert.AreEqual(expected, current);
            Assert.AreEqual(validInitialAmount, previous);
        }

        private static IEnumerable<TestCaseData> SetCases()
        {
            // starting value = 0
            yield return new TestCaseData(0, 0, 0).SetName("WhenStartingZeroAndSettingZero_ThenValueRemainsZero");
            yield return new TestCaseData(0, 5, 5).SetName("WhenStartingZeroAndSettingPositive_ThenValueBecomesSetAmount");
            yield return new TestCaseData(0, -5, 0).SetName("WhenStartingZeroAndSettingNegative_ThenValueBecomesZero");

            // starting value > 0
            yield return new TestCaseData(10, 0, 0).SetName("WhenStartingPositiveAndSettingZero_ThenValueBecomesZero");
            yield return new TestCaseData(10, 5, 5).SetName("WhenStartingPositiveAndSettingPositive_ThenValueBecomesSetAmount");
            yield return new TestCaseData(10, 20, 20).SetName("WhenStartingPositiveAndSettingLargerPositive_ThenValueBecomesSetAmount");
            yield return new TestCaseData(10, -5, 0).SetName("WhenStartingPositiveAndSettingNegative_ThenValueBecomesZero");

            // starting value < 0
            yield return new TestCaseData(-10, 0, 0).SetName("WhenStartingNegativeAndSettingZero_ThenValueBecomesZero");
            yield return new TestCaseData(-10, 5, 5).SetName("WhenStartingNegativeAndSettingPositive_ThenValueBecomesSetAmount");
            yield return new TestCaseData(-10, -5, 0).SetName("WhenStartingNegativeAndSettingNegative_ThenValueBecomesZero");
        }

        #endregion

        #region ChangeEvent
        
        [TestCaseSource(nameof(ChangeMethods))]
        public void Methods_ShouldTriggerChangedEvent(int initialAmount, Action<CurrencyCell> operation)
        {
            // Arrange
            CurrencyCell currency = new(initialAmount, CurrencyType.None);
            bool triggered = false;
            currency.OnChanged += () => triggered = true;

            // Act
            operation(currency);

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