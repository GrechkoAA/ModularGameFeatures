using System;
using System.Collections.Generic;
using Modules.Features.Currency.Bank.Scripts;
using NUnit.Framework;

namespace Modules.Features.Currency.Bank.Tests
{
    [TestFixture]
    public class TestsCurrencyBank
    {
        private CurrencyBank _bank;

        [SetUp]
        public void SetUp()
        {
            var cells = new List<(CurrencyType, CurrencyCell)>
            {
                (CurrencyType.Gold, new CurrencyCell(100)),
                (CurrencyType.Gems, new CurrencyCell(50))
            };

            _bank = new CurrencyBank(cells);
        }

        [Test]
        public void GetCell_ReturnsCorrectCell()
        {
            var goldCell = _bank.GetCell(CurrencyType.Gold);
            Assert.AreEqual(100, goldCell.Value);

            var gemsCell = _bank.GetCell(CurrencyType.Gems);
            Assert.AreEqual(50, gemsCell.Value);
        }

        [Test]
        public void AddCurrencies_AddsAmountsCorrectly()
        {
            _bank.AddCurrencies(new List<CurrencyAmount>
            {
                new CurrencyAmount(CurrencyType.Gold, 20),
                new CurrencyAmount(CurrencyType.Gems, 30)
            });

            Assert.AreEqual(120, _bank.GetCell(CurrencyType.Gold).Value);
            Assert.AreEqual(80, _bank.GetCell(CurrencyType.Gems).Value);
        }

        [Test]
        public void TrySpendCurrencies_SufficientFunds_ReturnsTrueAndSpends()
        {
            var result = _bank.TrySpendCurrencies(new List<CurrencyAmount>
            {
                new CurrencyAmount(CurrencyType.Gold, 50),
                new CurrencyAmount(CurrencyType.Gems, 20)
            });

            Assert.IsTrue(result);
            Assert.AreEqual(50, _bank.GetCell(CurrencyType.Gold).Value);
            Assert.AreEqual(30, _bank.GetCell(CurrencyType.Gems).Value);
        }

        [Test]
        public void TrySpendCurrencies_InsufficientFunds_ReturnsFalseAndDoesNotSpend()
        {
            var result = _bank.TrySpendCurrencies(new List<CurrencyAmount>
            {
                new CurrencyAmount(CurrencyType.Gold, 200), // больше, чем есть
                new CurrencyAmount(CurrencyType.Gems, 10)
            });

            Assert.IsFalse(result);
            Assert.AreEqual(100, _bank.GetCell(CurrencyType.Gold).Value);
            Assert.AreEqual(50, _bank.GetCell(CurrencyType.Gems).Value);
        }

        [Test]
        public void SetCurrencies_SetsValuesCorrectly()
        {
            _bank.SetCurrencies(new List<CurrencyAmount>
            {
                new CurrencyAmount(CurrencyType.Gold, 500),
                new CurrencyAmount(CurrencyType.Gems, 200)
            });

            Assert.AreEqual(500, _bank.GetCell(CurrencyType.Gold).Value);
            Assert.AreEqual(200, _bank.GetCell(CurrencyType.Gems).Value);
        }

        [Test]
        public void IsEnough_ReturnsTrueWhenEnough()
        {
            var cost = new List<CurrencyAmount>
            {
                new CurrencyAmount(CurrencyType.Gold, 50),
                new CurrencyAmount(CurrencyType.Gems, 40)
            };

            Assert.IsTrue(_bank.IsEnough(cost));
        }

        [Test]
        public void IsEnough_ReturnsFalseWhenNotEnough()
        {
            var cost = new List<CurrencyAmount>
            {
                new CurrencyAmount(CurrencyType.Gold, 150), // больше чем есть
                new CurrencyAmount(CurrencyType.Gems, 10)
            };

            Assert.IsFalse(_bank.IsEnough(cost));
        }

        [Test]
        public void Constructor_NullCells_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new CurrencyBank(null));
        }

        [Test]
        public void IsEnough_NullCost_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _bank.IsEnough(null));
        }
        
        [Test]
        public void IsEnough_ThrowsException_WhenCurrencyNotInBank()
        {
            var cells = new List<(CurrencyType, CurrencyCell)>
            {
                (CurrencyType.Gold, new CurrencyCell(100)),
            };

            CurrencyBank bank = new CurrencyBank(cells);
            
            var cost = new List<CurrencyAmount>
            {
                new CurrencyAmount(CurrencyType.Gems, 10) // Gems нет в банке
            };

            Assert.Throws<InvalidOperationException>(() => bank.IsEnough(cost));
        }
    }
}