using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using PromoEng.Engine;
using PromoEng.Testability;

namespace PromoEng.CoreWebApi.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="PromotionPipelineFactory"/>.
    /// </summary>
    public class PromotionPipelineFactoryTests
    {
        [Theory]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        public void WhenCreatedWithEitherInputNullThenExceptionIsThrown(
            bool nullInput, bool nullCartFactory, bool nullSkus)
        {
            var testContext = new TestContext();

            var input = nullInput ? null : new List<PromotionRuleOption>();
            var cartFactory = nullCartFactory ? null : testContext.CartFactory;
            var skus = nullSkus ? null : testContext.PriceList.Keys;

            Assert.Throws<ArgumentNullException>(() =>
            {
                var promotionPipelineFactory = new PromotionPipelineFactory(input, cartFactory, skus);
            });
        }

        [Fact]
        public void WhenCreatedWithEMptyInputThenEmptyPipelineIsCreated()
        {
            var testContext = new TestContext();

            var promotionPipelineFactory = new TestPromotionPipelineFactory(
                new List<PromotionRuleOption>(), testContext.CartFactory, testContext.PriceList.Keys);
            Assert.Empty(promotionPipelineFactory.PromotionRules);

            var pipeline = promotionPipelineFactory.Create();

            Assert.Empty(promotionPipelineFactory.PromotionRules);
        }

        [Fact]
        public void CreatePipelineWithRules()
        {
            var testContext = new TestContext();
            var sku1Id = "A";
            var sku2Id = "B";
            testContext.CreateNewSku(sku1Id, 100);
            testContext.CreateNewSku(sku2Id, 200);

            var promotionRules = new List<PromotionRuleOption>();
            promotionRules.Add(new PromotionRuleOption()
            {
                Id = PromotionPipelineFactory.CollectionOfSameSkuForRuleId,
                Param1 = "A",
                Param2 = "2",
                Param3 = "50"
            });
            promotionRules.Add(new PromotionRuleOption()
            {
                Id = PromotionPipelineFactory.PairOfDifferentSkusForRuleId,
                Param1 = "A",
                Param2 = "B",
                Param3 = "100"
            });

            var promotionPipelineFactory = new TestPromotionPipelineFactory(
                promotionRules, testContext.CartFactory, testContext.PriceList.Keys);

            var pipeline = promotionPipelineFactory.Create();

            Assert.NotEmpty(promotionPipelineFactory.PromotionRules);
            Assert.Equal(2, promotionPipelineFactory.PromotionRules.Count);
        }

        #region Types

        private class TestPromotionPipelineFactory : PromotionPipelineFactory
        {
            public ICollection<IPromotionRule> PromotionRules { get; private set; }

            public TestPromotionPipelineFactory(IList<PromotionRuleOption> promotionRuleOptions,
            ICartFactory cartFactory, IEnumerable<Sku> skus)
                : base(promotionRuleOptions, cartFactory, skus)
            {
                this.PromotionRules = new List<IPromotionRule>();
            }

            protected override IPromotionRule CreateRule(PromotionRuleOption promotionRuleOption)
            {
                var rule = base.CreateRule(promotionRuleOption);
                this.PromotionRules.Add(rule);

                return rule;
            }
        }

        #endregion
    }
}
