﻿using System;
using System.Linq;

using Xunit;
using Xunit.Sdk;

using PromoEng.Engine.Rules;
using PromoEng.Testability;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="PairOfDifferentSkusForRule"/>.
    /// </summary>
    public class PairOfDifferentSkusForRuleTests
    {
        [Fact]
        public void WhenSameSkuIsUsedThenExceptionIsThrownAtConstruction()
        {
            var testContext = new TestContext();

            var sku = testContext.CreateNewSku("A", 100);

            Assert.Throws<ArgumentException>(() =>
            {
                var rule = new PairOfDifferentSkusForRule(testContext.CartFactory, sku, sku, 100);
            });
        }

        [Theory]
        [InlineData(false, false, 50)]
        [InlineData(false, false, -50)]
        [InlineData(true, false, 50)]
        [InlineData(false, true, 50)]
        [InlineData(true, true, 50)]
        public void CreateRule(bool nullSku1, bool nullSku2, decimal batchPrice)
        {
            var testContext = new TestContext();

            Sku sku1 = nullSku1 ? null : testContext.CreateNewSku("A", 0);
            Sku sku2 = nullSku2 ? null : testContext.CreateNewSku("B", 0);
            PairOfDifferentSkusForRule rule = null;
            Action create = () =>
            {
                rule = new PairOfDifferentSkusForRule(testContext.CartFactory, sku1, sku2, batchPrice);
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
            var testContext = new TestContext();

            if (itemsNumberToAdd1 != itemsNumberToAdd2)
            {
                throw new TestClassException("Invalid configuration");
            }

            int batchesNumber = itemsNumberToAdd1;

            var sku1 = testContext.CreateNewSku("A", basePrice1);
            var sku2 = testContext.CreateNewSku("B", basePrice2);

            var cart = testContext.CartFactory.Create();
            cart.Add(sku1, itemsNumberToAdd1);
            cart.Add(sku2, itemsNumberToAdd2);
            Assert.Equal(itemsNumberToAdd1 + itemsNumberToAdd2, cart.Quantity);
            Assert.Equal(2, cart.Count());

            var rule = new PairOfDifferentSkusForRule(testContext.CartFactory, sku1, sku2, batchPrice);
            var newCart = rule.Evaluate(cart);
            Assert.Equal(batchesNumber, newCart.Quantity);

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
            var testContext = new TestContext();

            if (itemsNumberToAdd1 == itemsNumberToAdd2)
            {
                throw new TestClassException("Invalid configuration");
            }

            int batchesNumber = Math.Min(itemsNumberToAdd1, itemsNumberToAdd2);
            int residualsNumber = Math.Max(itemsNumberToAdd1, itemsNumberToAdd2) - batchesNumber;

            var sku1 = testContext.CreateNewSku("A", basePrice1);
            var sku2 = testContext.CreateNewSku("B", basePrice2);

            var cart = testContext.CartFactory.Create();
            cart.Add(sku1, itemsNumberToAdd1);
            cart.Add(sku2, itemsNumberToAdd2);
            Assert.Equal(itemsNumberToAdd1 + itemsNumberToAdd2, cart.Quantity);
            Assert.Equal(2, cart.Count());

            var rule = new PairOfDifferentSkusForRule(testContext.CartFactory, sku1, sku2, batchPrice);
            var newCart = rule.Evaluate(cart);
            Assert.Equal(batchesNumber + residualsNumber, newCart.Quantity);

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
            var testContext = new TestContext();

            if (itemsNumberToAdd1 > 0 && itemsNumberToAdd2 > 0)
            {
                throw new TestClassException("Invalid configuration");
            }

            var sku1 = testContext.CreateNewSku("A", basePrice1);
            var sku2 = testContext.CreateNewSku("B", basePrice2);

            var cart = testContext.CartFactory.Create();
            if (itemsNumberToAdd1 > 0)
            {
                cart.Add(sku1, itemsNumberToAdd1);
            }
            if (itemsNumberToAdd2 > 0)
            {
                cart.Add(sku2, itemsNumberToAdd2);
            }
            Assert.Equal(itemsNumberToAdd1 + itemsNumberToAdd2, cart.Quantity);
            Assert.Single(cart);

            var rule = new PairOfDifferentSkusForRule(testContext.CartFactory, sku1, sku2, batchPrice);
            var newCart = rule.Evaluate(cart);
            Assert.Equal(itemsNumberToAdd1 + itemsNumberToAdd2, newCart.Quantity);

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
            var testContext = new TestContext();

            int batchesNumber = Math.Min(itemsNumberToAdd1, itemsNumberToAdd2);
            int residualsNumber = Math.Max(itemsNumberToAdd1, itemsNumberToAdd2) - batchesNumber;
            bool residualsAreOfSku1 = itemsNumberToAdd1 - itemsNumberToAdd2 > 0;

            var sku1 = testContext.CreateNewSku("A", basePrice1);
            var sku2 = testContext.CreateNewSku("B", basePrice2);

            var cart = testContext.CartFactory.Create();
            cart.Add(sku1, itemsNumberToAdd1);
            cart.Add(sku2, itemsNumberToAdd2);
            Assert.Equal(itemsNumberToAdd1 + itemsNumberToAdd2, cart.Quantity);
            Assert.Equal(2, cart.Count());

            var rule = new PairOfDifferentSkusForRule(testContext.CartFactory, sku1, sku2, batchPrice);
            var newCart = rule.Evaluate(cart);
            Assert.Equal(
                basePrice1 * residualsNumber * (residualsAreOfSku1 ? 1 : 0) +
                basePrice2 * residualsNumber * (residualsAreOfSku1 ? 0 : 1) +
                batchPrice * batchesNumber, 
                newCart.Total);
        }
    }
}
