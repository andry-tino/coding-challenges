using System;
using System.Linq;

using Xunit;
using Xunit.Sdk;

using PromoEng.Engine.Rules;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="PairOfDifferentSkusForRule"/>.
    /// </summary>
    public class PairOfDifferentSkusForRuleTests
    {
        [Theory]
        [InlineData(false, false, 50)]
        [InlineData(false, false, -50)]
        [InlineData(true, false, 50)]
        [InlineData(false, true, 50)]
        [InlineData(true, true, 50)]
        public void CreateRule(bool nullSku1, bool nullSku2, decimal batchPrice)
        {
            Sku sku1 = nullSku1 ? null : new Sku("A", 100);
            Sku sku2 = nullSku2 ? null : new Sku("B", 200);
            PairOfDifferentSkusForRule rule = null;
            Action create = () =>
            {
                rule = new PairOfDifferentSkusForRule(new TestCartFactory(), sku1, sku2, batchPrice);
            };

            if (sku1 == null || sku2 == null)
            {
                Assert.Throws<ArgumentNullException>(create);
                return;
            }

            create();
            Assert.NotNull(rule.Sku1);
            Assert.NotNull(rule.Sku2);
            Assert.True(rule.TotalPrice > 0);
        }

        [Theory]
        [InlineData(100, 200, 150, 2, 2)]
        [InlineData(100, 200, 150, 1, 1)]
        public void WhenSameAmountOfBothSkuTypesThenAllAreBatched(
            decimal basePrice1, decimal basePrice2,
            decimal batchPrice,
            int itemsNumberToAdd1, int itemsNumberToAdd2)
        {
            if (itemsNumberToAdd1 != itemsNumberToAdd2)
            {
                throw new TestClassException("Invalid configuration");
            }

            int batchesNumber = itemsNumberToAdd1;

            var sku1 = new Sku("A", basePrice1);
            var sku2 = new Sku("B", basePrice2);

            var cart = new StandardCart();
            cart.Add(sku1, itemsNumberToAdd1);
            cart.Add(sku2, itemsNumberToAdd2);
            Assert.Equal(itemsNumberToAdd1 + itemsNumberToAdd2, cart.Count);
            Assert.Equal(2, cart.Count());

            var rule = new PairOfDifferentSkusForRule(new TestCartFactory(), sku1, sku2, batchPrice);
            var newCart = rule.Evaluate(cart);
            Assert.Equal(batchesNumber, newCart.Count);

            Action<SkuCartEntry> batchChecker = entry =>
            {
                entry.CheckCartEntryWasProcessedByRule();
                Assert.Equal(1, entry.Quantity);
                Assert.Equal(batchPrice, entry.Price);
            };
            Assert.Collection(newCart, Enumerable.Repeat(batchChecker, batchesNumber).ToArray());
        }

        [Theory]
        [InlineData(100, 200, 150, 3, 2)] // 1 residual
        [InlineData(100, 200, 150, 2, 3)] // 1 residual
        public void WhenNotSameAmountOfBothSkuTypesThenSomeResidualsAreLeft(
            decimal basePrice1, decimal basePrice2,
            decimal batchPrice,
            int itemsNumberToAdd1, int itemsNumberToAdd2)
        {
            if (itemsNumberToAdd1 == itemsNumberToAdd2)
            {
                throw new TestClassException("Invalid configuration");
            }

            int batchesNumber = Math.Min(itemsNumberToAdd1, itemsNumberToAdd2);
            int residualsNumber = Math.Max(itemsNumberToAdd1, itemsNumberToAdd2) - batchesNumber;

            var sku1 = new Sku("A", basePrice1);
            var sku2 = new Sku("B", basePrice2);

            var cart = new StandardCart();
            cart.Add(sku1, itemsNumberToAdd1);
            cart.Add(sku2, itemsNumberToAdd2);
            Assert.Equal(itemsNumberToAdd1 + itemsNumberToAdd2, cart.Count);
            Assert.Equal(2, cart.Count());

            var rule = new PairOfDifferentSkusForRule(new TestCartFactory(), sku1, sku2, batchPrice);
            var newCart = rule.Evaluate(cart);
            Assert.Equal(batchesNumber + residualsNumber, newCart.Count);

            Action<SkuCartEntry> batchChecker = entry =>
            {
                entry.CheckCartEntryWasProcessedByRule();
                Assert.Equal(1, entry.Quantity);
                Assert.Equal(batchPrice, entry.Price);
            };
            Action<SkuCartEntry> residualChecker = entry =>
            {
                entry.CheckCartEntryWasNotProcessedByRule();
                Assert.Equal(1, entry.Quantity);
                Assert.True(entry.Price == basePrice1 || entry.Price == basePrice2);
            };
            Assert.Collection(newCart,
                Enumerable.Repeat(batchChecker, batchesNumber)
                    .Concat(Enumerable.Repeat(residualChecker, residualsNumber))
                    .ToArray());
        }

        [Theory]
        [InlineData(100, 200, 150, 3, 0)]
        [InlineData(100, 200, 150, 0, 3)]
        public void WhenOneAmountOfSkuBatchableTypesIsZeroThenNoBatchesAreCreated(
            decimal basePrice1, decimal basePrice2,
            decimal batchPrice,
            int itemsNumberToAdd1, int itemsNumberToAdd2)
        {
            if (itemsNumberToAdd1 > 0 && itemsNumberToAdd2 > 0)
            {
                throw new TestClassException("Invalid configuration");
            }

            var sku1 = new Sku("A", basePrice1);
            var sku2 = new Sku("B", basePrice2);

            var cart = new StandardCart();
            if (itemsNumberToAdd1 > 0)
            {
                cart.Add(sku1, itemsNumberToAdd1);
            }
            if (itemsNumberToAdd2 > 0)
            {
                cart.Add(sku2, itemsNumberToAdd2);
            }
            Assert.Equal(itemsNumberToAdd1 + itemsNumberToAdd2, cart.Count);
            Assert.Single(cart);

            var rule = new PairOfDifferentSkusForRule(new TestCartFactory(), sku1, sku2, batchPrice);
            var newCart = rule.Evaluate(cart);
            Assert.Equal(itemsNumberToAdd1 + itemsNumberToAdd2, newCart.Count);

            Action<SkuCartEntry> residualChecker = entry =>
            {
                entry.CheckCartEntryWasNotProcessedByRule();
                Assert.Equal(1, entry.Quantity);
                Assert.True(entry.Price == basePrice1 || entry.Price == basePrice2);
            };
            Assert.Collection(newCart, Enumerable.Repeat(residualChecker, itemsNumberToAdd1 + itemsNumberToAdd2)
                .ToArray());
        }

        [Theory]
        [InlineData(100, 200, 150, 3, 2)] // With residuals
        [InlineData(100, 200, 150, 2, 3)] // With residuals
        [InlineData(100, 200, 0, 2, 3)] // With residuals
        [InlineData(100, 200, 150, 3, 3)] // No residuals
        [InlineData(100, 200, 0, 3, 3)] // No residuals
        public void CorrectTotalPriceWhenBatching(
            decimal basePrice1, decimal basePrice2,
            decimal batchPrice,
            int itemsNumberToAdd1, int itemsNumberToAdd2)
        {
            int batchesNumber = Math.Min(itemsNumberToAdd1, itemsNumberToAdd2);
            int residualsNumber = Math.Max(itemsNumberToAdd1, itemsNumberToAdd2) - batchesNumber;
            bool residualsAreOfSku1 = itemsNumberToAdd1 - itemsNumberToAdd2 > 0;

            var sku1 = new Sku("A", basePrice1);
            var sku2 = new Sku("B", basePrice2);

            var cart = new StandardCart();
            cart.Add(sku1, itemsNumberToAdd1);
            cart.Add(sku2, itemsNumberToAdd2);
            Assert.Equal(itemsNumberToAdd1 + itemsNumberToAdd2, cart.Count);
            Assert.Equal(2, cart.Count());

            var rule = new PairOfDifferentSkusForRule(new TestCartFactory(), sku1, sku2, batchPrice);
            var newCart = rule.Evaluate(cart);
            Assert.Equal(
                basePrice1 * residualsNumber * (residualsAreOfSku1 ? 1 : 0) +
                basePrice2 * residualsNumber * (residualsAreOfSku1 ? 0 : 1) +
                batchPrice * batchesNumber, 
                newCart.Total);
        }

        #region Types

        private class TestCartFactory : ICartFactory
        {
            public ICart Create()
            {
                return new StandardCart();
            }
        }

        #endregion
    }
}
