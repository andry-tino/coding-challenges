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

        [Fact]
        public void PairOfDifferentSkusForRule_Two()
        {
            var testContext = new TestContext();

            var skuA = testContext.CreateNewSku("A", 100);
            var skuB = testContext.CreateNewSku("B", 200);
            var skuC = testContext.CreateNewSku("C", 300);
            var skuD = testContext.CreateNewSku("D", 400);

            var cart = testContext.CartFactory.Create();
            cart.Add(skuA, 2);
            cart.Add(skuB, 3);
            cart.Add(skuC, 2);
            cart.Add(skuD, 3);
            cart.TestTotal(2 * 100 + 3 * 200 + 2 * 300 + 3 * 400);

            var pipeline = new FaultTolerantPromotionPipeline();
            pipeline.AddRule(new PairOfDifferentSkusForRule(testContext.CartFactory, skuA, skuB, 100));
            pipeline.AddRule(new PairOfDifferentSkusForRule(testContext.CartFactory, skuC, skuD, 200));
            var newCart = pipeline.Apply(cart);

            newCart.TestTotal(2 * 100 + 1 * 200 + 2 * 200 + 1 * 400);
        }
    }
}
