using System;
using System.Linq;

using Xunit;

using PromoEng.Testability;
using PromoEng.Engine.Rules;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Common unit tests for all <see cref="IPromotionRule"/>.
    /// These tests target common behavior that all rules must comply to.
    /// </summary>
    public class PromotionRuleTests
    {
        [Theory]
        [InlineData(PromotionRuleType.BatchOfSameForFixedPrice)]
        [InlineData(PromotionRuleType.PairOfDifferentForFixedPrice)]
        public void RulesApplicationGeneratesNonStrictlyMonotonicDescendingQuantityTrend(PromotionRuleType type)
        {
            var testContext = new TestContext();

            IPromotionRule rule = new PromotionRuleFactory(testContext).Create(type);
            ICart cart = new CartFactory(testContext).Create(type);
            int originalQuantity = cart.Quantity;

            ICart newCart = rule.Evaluate(cart);
            int newQuantity = newCart.Quantity;

            Assert.True (newQuantity <= originalQuantity);
        }

        [Theory]
        [InlineData(PromotionRuleType.BatchOfSameForFixedPrice)]
        [InlineData(PromotionRuleType.PairOfDifferentForFixedPrice)]
        public void RulesApplicationChangesNothingWhenAllEntriesAreAlreadyMarkedAsHandled(PromotionRuleType type)
        {
            var testContext = new TestContext();

            IPromotionRule rule = new PromotionRuleFactory(testContext).Create(type);
            ICart cart = new CartFactory(testContext).Create(type);

            var invariantPromotionRule = new InvariantPromotionRule(testContext.CartFactory);
            ICart cartWithNoPromotableItems = invariantPromotionRule.Evaluate(cart);
            decimal originalPrice = cartWithNoPromotableItems.Total;
            int originalQuantity = cartWithNoPromotableItems.Quantity;
            int originalCount = cartWithNoPromotableItems.Count;

            ICart newCart = rule.Evaluate(cartWithNoPromotableItems);
            decimal newPrice = newCart.Total;
            int newQuantity = newCart.Quantity;
            int newCount = newCart.Count;

            Assert.Equal(originalPrice, newPrice);
            Assert.Equal(originalQuantity, newQuantity);
            Assert.Equal(originalCount, newCount);
        }

        [Theory]
        [InlineData(PromotionRuleType.BatchOfSameForFixedPrice)]
        [InlineData(PromotionRuleType.PairOfDifferentForFixedPrice)]
        public void RulesApplicationGeneratesNonStrictlyMonotonicDescendingPriceTrend(PromotionRuleType type)
        {
            var testContext = new TestContext();

            IPromotionRule rule = new PromotionRuleFactory(testContext).Create(type);
            ICart cart = new CartFactory(testContext).Create(type);
            decimal originalPrice = cart.Total;

            ICart newCart = rule.Evaluate(cart);
            decimal newPrice = newCart.Total;

            Assert.True(newPrice <= originalPrice);
        }

        #region Types

        public enum PromotionRuleType
        {
            BatchOfSameForFixedPrice,
            PairOfDifferentForFixedPrice
        }

        private class PromotionRuleFactory
        {
            public const int BatchOfSameForFixedPriceSize = 2;
            public const string SkuAId = "A";
            public const string SkuBId = "B";

            private readonly TestContext testContext;

            public PromotionRuleFactory(TestContext testContext)
            {
                this.testContext = testContext;
            }

            public IPromotionRule Create(PromotionRuleType type)
            {
                // Guarantee that the pricelist in the context contains the
                // sku(s) that is/are needed to activate the rule
                switch (type)
                {
                    case PromotionRuleType.PairOfDifferentForFixedPrice:
                        return new PairOfDifferentSkusForRule(this.testContext.CartFactory,
                            testContext.CreateNewSku(SkuAId, 100),
                            testContext.CreateNewSku(SkuBId, 200),
                            100);

                    default:
                    case PromotionRuleType.BatchOfSameForFixedPrice:
                        return new CollectionOfSameSkuForRule(this.testContext.CartFactory,
                            testContext.CreateNewSku(SkuAId, 100), BatchOfSameForFixedPriceSize, 50);
                }
            }
        }

        /// <summary>
        /// Operates in combo with <see cref="PromotionRuleFactory"/> to make sure the created
        /// cart has the necessary <see cref="Sku"/> to activate the specified rule.
        /// </summary>
        private class CartFactory
        {
            private readonly TestContext testContext;

            public CartFactory(TestContext testContext)
            {
                this.testContext = testContext;
            }

            public ICart Create(PromotionRuleType type)
            {
                // Guarantee that the cart in the context contains the
                // sku(s) that is/are needed to activate the rule
                var cart = this.testContext.CartFactory.Create();

                switch (type)
                {
                    case PromotionRuleType.PairOfDifferentForFixedPrice:
                        cart.Add(testContext.PriceList.First(entry => entry.Key.Id == PromotionRuleFactory.SkuAId).Key, 1);
                        cart.Add(testContext.PriceList.First(entry => entry.Key.Id == PromotionRuleFactory.SkuBId).Key, 1);
                        break;

                    default:
                    case PromotionRuleType.BatchOfSameForFixedPrice:
                        cart.Add(testContext.PriceList.First(entry => entry.Key.Id == PromotionRuleFactory.SkuAId).Key,
                            PromotionRuleFactory.BatchOfSameForFixedPriceSize);
                        break;
                }

                return cart;
            }
        }

        /// <summary>
        /// A dummy class to create a cart with the same exact entries of the original but all marked as handled.
        /// This rule applies a null promotion on each item.
        /// </summary>
        private class InvariantPromotionRule : IPromotionRule
        {
            private readonly ICartFactory cartFactory;

            public InvariantPromotionRule(ICartFactory cartFactory)
            {
                this.cartFactory = cartFactory;
            }

            public ICart Evaluate(ICart originalCart)
            {
                var newCart = this.cartFactory.Create();
                foreach (SkuCartEntry entry in originalCart)
                {
                    var clonedEntry = entry.Clone() as SkuCartEntry;
                    entry.PromotionRuleId = "InvariantPromotion";
                    entry.Description = "Test promotion applied";

                    newCart.Add(entry);
                }

                return newCart;
            }
        }

        #endregion
    }
}
