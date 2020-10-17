using System;
using System.Linq;

using Xunit;
using Xunit.Sdk;

using PromoEng.Engine.Rules;
using PromoEng.Testability;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="CollectionOfSameSkuForRule"/>.
    /// </summary>
    public class CollectionOfSameSkuForRuleTests
    {
        [Theory]
        [InlineData(false, 50, 2)]
        [InlineData(true, 50, 2)]
        [InlineData(false, -50, 2)]
        [InlineData(false, 50, -2)]
        [InlineData(false, -50, -2)]
        public void CreateRule(bool nullSku, decimal batchPrice, int batchQuantity)
        {
            var testContext = new TestContext();

            Sku sku = nullSku ? null : testContext.CreateNewSku("A", 100);
            CollectionOfSameSkuForRule rule = null;
            Action create = () =>
            {
                rule = new CollectionOfSameSkuForRule(testContext.CartFactory, sku, batchQuantity, batchPrice);
            };

            if (sku == null)
            {
                Assert.Throws<ArgumentNullException>(create);
                return;
            }

            if (batchQuantity == 0)
            {
                Assert.Throws<ArgumentException>(create);
                return;
            }

            create();
            Assert.NotNull(rule.Sku);
            Assert.True(rule.Quantity > 0);
            Assert.True(rule.TotalPrice > 0);
        }

        [Theory]
        [InlineData(100, 50, 2, 4)]
        [InlineData(100, 50, 3, 9)]
        public void WhenRightAmountOfEntriesIsBatchableThenNoResidualsAreLeft(
            decimal basePrice, decimal batchPrice, int batchQuantity, int itemsNumberToAdd)
        {
            var testContext = new TestContext();

            if (itemsNumberToAdd % batchQuantity != 0)
            {
                throw new TestClassException("Invalid configuration");
            }

            int batchesNumber = itemsNumberToAdd / batchQuantity;

            var sku = testContext.CreateNewSku("A", basePrice);

            var cart = testContext.CartFactory.Create();
            cart.Add(sku, itemsNumberToAdd);
            Assert.Equal(itemsNumberToAdd, cart.Quantity);
            Assert.Single(cart);

            var rule = new CollectionOfSameSkuForRule(testContext.CartFactory, sku, batchQuantity, batchPrice);
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
        [InlineData(100, 50, 2, 3)] // 1 residual
        [InlineData(100, 50, 3, 8)] // 2 residuals
        public void WhenNonRightAmountOfEntriesIsBatchableThenSomeResidualsAreLeft(
            decimal basePrice, decimal batchPrice, int batchQuantity, int itemsNumberToAdd)
        {
            var testContext = new TestContext();

            int residualsNumber = itemsNumberToAdd % batchQuantity;
            if (residualsNumber == 0)
            {
                throw new TestClassException("Invalid configuration");
            }

            int batchesNumber = itemsNumberToAdd / batchQuantity;

            var sku = testContext.CreateNewSku("A", basePrice);

            var cart = testContext.CartFactory.Create();
            cart.Add(sku, itemsNumberToAdd);
            Assert.Equal(itemsNumberToAdd, cart.Quantity);
            Assert.Single(cart);

            var rule = new CollectionOfSameSkuForRule(testContext.CartFactory, sku, batchQuantity, batchPrice);
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
                Assert.Equal(basePrice, entry.Price);
            };
            Assert.Collection(newCart,
                Enumerable.Repeat(batchChecker, batchesNumber)
                    .Concat(Enumerable.Repeat(residualChecker, residualsNumber))
                    .ToArray());
        }

        [Theory]
        [InlineData(100, 50, 3, 2)]
        [InlineData(100, 50, 4, 1)]
        public void WhenNonSufficientAmountOfEntriesIsBatchableThenNoBatchesAreCreated(
            decimal basePrice, decimal batchPrice, int batchQuantity, int itemsNumberToAdd)
        {
            var testContext = new TestContext();

            if (itemsNumberToAdd >= batchQuantity)
            {
                throw new TestClassException("Invalid configuration");
            }

            var sku = testContext.CreateNewSku("A", basePrice);

            var cart = testContext.CartFactory.Create();
            cart.Add(sku, itemsNumberToAdd);
            Assert.Equal(itemsNumberToAdd, cart.Quantity);
            Assert.Single(cart);

            var rule = new CollectionOfSameSkuForRule(testContext.CartFactory, sku, batchQuantity, batchPrice);
            var newCart = rule.Evaluate(cart);
            Assert.Equal(itemsNumberToAdd, newCart.Quantity);

            Action<SkuCartEntry> residualChecker = entry =>
            {
                entry.CheckCartEntryWasNotProcessedByRule();
                Assert.Equal(1, entry.Quantity);
                Assert.Equal(basePrice, entry.Price);
            };
            Assert.Collection(newCart, Enumerable.Repeat(residualChecker, itemsNumberToAdd).ToArray());
        }

        [Theory]
        [InlineData(100, 50, 3, 2)] // With residuals
        [InlineData(100, 50, 4, 2)] // Without residuals
        [InlineData(100, 0, 3, 2)] // With residuals
        [InlineData(100, 0, 4, 2)] // Without residuals
        public void CorrectTotalPriceWhenBatching(
            decimal basePrice, decimal batchPrice, int batchQuantity, int itemsNumberToAdd)
        {
            var testContext = new TestContext();

            int residualsNumber = itemsNumberToAdd % batchQuantity;
            int batchesNumber = itemsNumberToAdd / batchQuantity;

            var sku = testContext.CreateNewSku("A", basePrice);

            var cart = testContext.CartFactory.Create();
            cart.Add(sku, itemsNumberToAdd);
            Assert.Equal(basePrice * itemsNumberToAdd, cart.Total);

            var rule = new CollectionOfSameSkuForRule(testContext.CartFactory, sku, batchQuantity, batchPrice);
            var newCart = rule.Evaluate(cart);
            Assert.Equal(basePrice * residualsNumber + batchPrice * batchesNumber, newCart.Total);
        }
    }
}
