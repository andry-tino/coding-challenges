using System;

using Xunit;

using PromoEng.Engine.Rules;
using PromoEng.Testability;

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
            var testContext = new TestContext();

            var skuA = testContext.CreateNewSku("A", 100);
            var skuB = testContext.CreateNewSku("B", 200);

            var cart = testContext.CartFactory.Create();
            cart.Add(skuA, 2);
            cart.Add(skuB, 3);
            cart.TestTotal(2*100 + 3*200);

            var pipeline = new FaultTolerantPromotionPipeline();
            pipeline.AddRule(new CollectionOfSameSkuForRule(testContext.CartFactory, skuA, 2, 50));
            pipeline.AddRule(new CollectionOfSameSkuForRule(testContext.CartFactory, skuB, 2, 100));
            var newCart = pipeline.Apply(cart);

            newCart.TestTotal(1*50 + 1*100 + 200);
        }
    }
}
