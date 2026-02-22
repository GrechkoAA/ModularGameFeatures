using Modules.Features.Currency.Bank.Scripts;
using NUnit.Framework;

namespace Modules.Features.Currency.Bank.Tests
{
    [TestFixture]
    public class TestsCurrencyCell
    {
        #region Earn

        [Test]
        public void Earn_IncreasesValue()
        {
            var storage = new CurrencyCell(10);

            storage.Add(5);

            Assert.AreEqual(15, storage.Value);
        }

        [Test]
        public void Earn_ShouldIncreaseValue_WhenStartingNegative()
        {
            var storage = new CurrencyCell(-10);

            storage.Add(5);

            Assert.AreEqual(5, storage.Value);
        }

        [Test]
        public void Earn_WhenValueOverflowsWithoutChecked_WrapsAround()
        {
            var storage = new CurrencyCell(int.MaxValue);

            storage.Add(1);

            Assert.AreEqual(int.MinValue, storage.Value);
        }

        #endregion

        #region Spend

        [Test]
        public void Spend_DoesNotGoBelowZero()
        {
            var storage = new CurrencyCell(5);

            storage.Spend(10);

            Assert.AreEqual(0, storage.Value);
        }

        [Test]
        public void Spend_ReducesValue()
        {
            var storage = new CurrencyCell(10);

            storage.Spend(3);

            Assert.AreEqual(7, storage.Value);
        }

        [Test]
        public void Set_DoesNotGoBelowZero()
        {
            var storage = new CurrencyCell(5);

            storage.Set(-10);

            Assert.AreEqual(0, storage.Value);
        }

        [Test]
        public void Set_Construct_DoesNotGoBelowZero()
        {
            var storage = new CurrencyCell(-5);

            Assert.AreEqual(0, storage.Value);
        }

        #endregion

        #region Set

        [Test]
        public void Set_ChangesValue()
        {
            var storage = new CurrencyCell(10);

            storage.Set(20);

            Assert.AreEqual(20, storage.Value);
        }

        [Test]
        public void Set_TriggersSetEvent()
        {
            var storage = new CurrencyCell(10);

            int previous = 0;
            int current = 0;

            storage.OnSet += (cur, prev) =>
            {
                current = cur;
                previous = prev;
            };

            storage.Set(15);

            Assert.AreEqual(15, current);
            Assert.AreEqual(10, previous);
        }

        [Test]
        public void Set_ToSameValue_StillTriggersSetEvent()
        {
            var storage = new CurrencyCell(10);

            int previous = 0;
            int current = 0;

            storage.OnSet += (cur, prev) =>
            {
                current = cur;
                previous = prev;
            };

            storage.Set(10);

            Assert.AreEqual(10, current);
            Assert.AreEqual(10, previous);
        }

        #endregion
    }
}