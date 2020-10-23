using System;

using Xunit;

using PromoEng.Engine.Rules;
using PromoEng.Testability;

namespace PromoEng.Engine.Scenarios
{
    /// <summary>
    /// Cover scenarios where different rule types are used in the pipeline.
    /// </summary>
    public class DifferentRulesScenarios
    {
        private const decimal skuAUnitPrice = 50;
        private const decimal skuBUnitPrice = 30;
        private const decimal skuCUnitPrice = 20;
        private const decimal skuDUnitPrice = 15;
        private const decimal rule1BatchPrice = 130;
        private const decimal rule2BatchPrice = 45;
        private const decimal rule3BatchPrice = 30;

        [Fact]
        public void ExampleScenarioA()
        {
            var testContext = new TestContext();

            (IPromotionPipeline pipeline, Sku skuA, Sku skuB, Sku skuC, Sku skuD) =
                this.CreateTestFullContext(testContext);

            var cart = testContext.CartFactory.Create();
            cart.Add(skuA, 1);
            cart.Add(skuB, 1);
            cart.Add(skuC, 1);
            cart.TestTotal(1 * skuAUnitPrice + 1 * skuBUnitPrice + 1 * skuCUnitPrice);

            var newCart = pipeline.Apply(cart);

            var newTotal = 1 * skuAUnitPrice + 1 * skuBUnitPrice + 1 * skuCUnitPrice;
            newCart.TestTotal(newTotal);
            Assert.Equal(100, newTotal);
        }

        [Fact]
        public void ExampleScenarioB()
        {
            var testContext = new TestContext();

            (IPromotionPipeline pipeline, Sku skuA, Sku skuB, Sku skuC, Sku skuD) =
                this.CreateTestFullContext(testContext);

            var cart = testContext.CartFactory.Create();
            cart.Add(skuA, 5);
            cart.Add(skuB, 5);
            cart.Add(skuC, 1);
            cart.TestTotal(5 * skuAUnitPrice + 5 * skuBUnitPrice + 1 * skuCUnitPrice);

            var newCart = pipeline.Apply(cart);

            var newTotal = 1 * rule1BatchPrice + 2 * skuAUnitPrice +
                2 * rule2BatchPrice + 1 * skuBUnitPrice +
                1 * skuCUnitPrice;
            newCart.TestTotal(newTotal);
            Assert.Equal(370, newTotal);
        }

        [Fact]
        public void ExampleScenarioC()
        {
            var testContext = new TestContext();

            (IPromotionPipeline pipeline, Sku skuA, Sku skuB, Sku skuC, Sku skuD) =
                this.CreateTestFullContext(testContext);

            var cart = testContext.CartFactory.Create();
            cart.Add(skuA, 3);
            cart.Add(skuB, 5);
            cart.Add(skuC, 1);
            cart.Add(skuD, 1);
            cart.TestTotal(3 * skuAUnitPrice + 5 * skuBUnitPrice + 1 * skuCUnitPrice + 1 * skuDUnitPrice);

            var newCart = pipeline.Apply(cart);

            var newTotal = 1 * rule1BatchPrice +
                2 * rule2BatchPrice + 1 * skuBUnitPrice +
                1 * rule3BatchPrice;
            newCart.TestTotal(newTotal);
            Assert.Equal(280, newTotal);
        }

        private (IPromotionPipeline pipeline, Sku skuA, Sku skuB, Sku skuC, Sku skuD)
            CreateTestFullContext(TestContext testContext)
        {
            var skuA = testContext.CreateNewSku("A", skuAUnitPrice);
            var skuB = testContext.CreateNewSku("B", skuBUnitPrice);
            var skuC = testContext.CreateNewSku("C", skuCUnitPrice);
            var skuD = testContext.CreateNewSku("D", skuDUnitPrice);

            var pipeline = new FaultTolerantPromotionPipeline();
            pipeline.AddRule(new CollectionOfSameSkuForRule(testContext.CartFactory, skuA, 3, rule1BatchPrice));
            pipeline.AddRule(new CollectionOfSameSkuForRule(testContext.CartFactory, skuB, 2, rule2BatchPrice));
            pipeline.AddRule(new PairOfDifferentSkusForRule(testContext.CartFactory, skuC, skuD, rule3BatchPrice));

            return (pipeline, skuA, skuB, skuC, skuD);
        }
    }
}
