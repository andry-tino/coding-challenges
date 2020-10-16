using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using PromoEng.Engine;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="PromotionPipeline"/>.
    /// </summary>
    public class PromotionPipelineTests
    {
        [Fact]
        public void WhenCartIsNullThenNullCartIsReturned()
        {
            Assert.Null(new PromotionPipeline().Apply(null));
        }

        [Fact]
        public void RulesAreExecutedInOrder()
        {
            var pipeline = new TestPromotionPipeline();
            var ruleA = new PromotionRuleA();
            var ruleB = new PromotionRuleB();
            pipeline.AddRule(ruleA);
            pipeline.AddRule(ruleB);

            pipeline.Apply(new Cart());

            Assert.True(pipeline.RunHistory.Any());
            Assert.Equal(2, pipeline.RunHistory.Count);
            Assert.True((object)pipeline.RunHistory[0] == (object)ruleA);
            Assert.True((object)pipeline.RunHistory[1] == (object)ruleB);
        }

        #region Types

        private class TestPromotionPipeline : PromotionPipeline
        {
            public IList<IPromotionRule> RunHistory { get; private set; }

            public TestPromotionPipeline() : base()
            {
                this.RunHistory = new List<IPromotionRule>();
            }

            protected override Cart RunRule(IPromotionRule rule, Cart cart)
            {
                this.RunHistory.Add(rule);
                return base.RunRule(rule, cart);
            }
        }

        private class PromotionRuleA : IPromotionRule
        {
            public Cart Evaluate(Cart originalCart)
            {
                return originalCart.Clone() as Cart;
            }
        }

        private class PromotionRuleB : IPromotionRule
        {
            public Cart Evaluate(Cart originalCart)
            {
                return originalCart.Clone() as Cart;
            }
        }

        #endregion
    }
}