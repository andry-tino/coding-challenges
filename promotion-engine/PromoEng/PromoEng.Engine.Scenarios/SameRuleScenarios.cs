using System;

using Xunit;

using PromoEng.Engine.Rules;

namespace PromoEng.Engine.Scenarios
{
    /// <summary>
    /// Cover scenarios where the same rule type is used in the pipeline.
    /// </summary>
    public class SameRuleScenarios
    {
        [Fact]
        public void CollectionOfSameSkuForRule_Two()
        {
            var skuA = new Sku("A", 100);
            var skuB = new Sku("B", 200);

            var cart = new StandardCart();
            cart.Add(skuA, 2);
            cart.Add(skuB, 3);
            cart.TestTotal(2*100 + 3*200);

            var pipeline = new PromotionPipeline();
            pipeline.AddRule(new CollectionOfSameSkuForRule(new TestCartFactory(), skuA, 2, 50));
            pipeline.AddRule(new CollectionOfSameSkuForRule(new TestCartFactory(), skuB, 2, 100));
            var newCart = pipeline.Apply(cart);

            newCart.TestTotal(1*50 + 1*100 + 200);
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
