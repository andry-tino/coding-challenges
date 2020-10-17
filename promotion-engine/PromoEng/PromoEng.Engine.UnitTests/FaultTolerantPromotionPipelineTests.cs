using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using PromoEng.Testability;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="FaultTolerantPromotionPipeline"/>.
    /// </summary>
    public class FaultTolerantPromotionPipelineTests
    {
        [Fact]
        public void WhenCartIsNullThenNullCartIsReturned()
        {
            Assert.Null(new FaultTolerantPromotionPipeline().Apply(null));
        }

        [Fact]
        public void WhenAddingNullRuleThenExceptionIsThrown()
        {
            var pipeline = new TestPromotionPipeline();

            Assert.Throws<ArgumentNullException>(() =>
            {
                pipeline.AddRule(null);
            });
        }

        [Fact]
        public void RulesAreExecutedInOrder()
        {
            var testContext = new TestContext();

            var pipeline = new TestPromotionPipeline();
            var ruleA = new PromotionRuleA();
            var ruleB = new PromotionRuleB();
            pipeline.AddRule(ruleA);
            pipeline.AddRule(ruleB);

            pipeline.Apply(testContext.CartFactory.Create());

            Assert.True(pipeline.RunHistory.Any());
            Assert.Equal(2, pipeline.RunHistory.Count);
            Assert.True((object)pipeline.RunHistory[0] == (object)ruleA);
            Assert.True((object)pipeline.RunHistory[1] == (object)ruleB);
        }

        [Fact]
        public void WhenNoRulesThenExceptionsListIsEmpty()
        {
            var testContext = new TestContext();

            var pipeline = new FaultTolerantPromotionPipeline();

            ICart cart = pipeline.Apply(testContext.CartFactory.Create());

            Assert.NotNull(cart);
            Assert.Empty(pipeline.LastApplyExceptions);
        }

        [Fact]
        public void WhenApplicationWithNoErrorsThenExceptionsListIsEmpty()
        {
            var testContext = new TestContext();

            var pipeline = new FaultTolerantPromotionPipeline();
            pipeline.AddRule(new PromotionRuleA());
            pipeline.AddRule(new PromotionRuleB());

            ICart cart = pipeline.Apply(testContext.CartFactory.Create());

            Assert.NotNull(cart);
            Assert.Empty(pipeline.LastApplyExceptions);
        }

        [Fact]
        public void WhenApplicationWithErrorsThenExceptionsListIsNotEmpty()
        {
            var testContext = new TestContext();

            var pipeline = new FaultTolerantPromotionPipeline();
            pipeline.AddRule(new PromotionRuleA());
            pipeline.AddRule(new FaultyPromotionRule());

            ICart cart = pipeline.Apply(testContext.CartFactory.Create());

            Assert.NotNull(cart);
            Assert.NotEmpty(pipeline.LastApplyExceptions);
        }

        [Fact]
        public void WhenApplicationWithErrorThenExceptionsListReportsFaultyRule()
        {
            var testContext = new TestContext();

            var pipeline = new FaultTolerantPromotionPipeline();
            var ruleA = new PromotionRuleA();
            var ruleB = new FaultyPromotionRule();
            pipeline.AddRule(ruleA);
            pipeline.AddRule(ruleB);

            ICart cart = pipeline.Apply(testContext.CartFactory.Create());

            Assert.NotNull(cart);
            Assert.NotEmpty(pipeline.LastApplyExceptions);
            Assert.Collection(pipeline.LastApplyExceptions, errorEntry =>
                {
                    Assert.True((object)errorEntry.Item1 == (object)ruleB);
                    Assert.IsType<TestException>(errorEntry.Item2);
                });
        }

        [Fact]
        public void WhenApplicationWithErrorsThenExceptionsListReportsFaultyRules()
        {
            var testContext = new TestContext();

            var pipeline = new FaultTolerantPromotionPipeline();
            var ruleA = new PromotionRuleA();
            var ruleB = new FaultyPromotionRule();
            var ruleC = new PromotionRuleA();
            var ruleD = new FaultyPromotionRule();
            pipeline.AddRule(ruleA);
            pipeline.AddRule(ruleB);
            pipeline.AddRule(ruleC);
            pipeline.AddRule(ruleD);

            ICart cart = pipeline.Apply(testContext.CartFactory.Create());

            Assert.NotNull(cart);
            Assert.NotEmpty(pipeline.LastApplyExceptions);

            Assert.Collection(pipeline.LastApplyExceptions,
                errorEntry =>
                {
                    Assert.True((object)errorEntry.Item1 == (object)ruleB);
                    Assert.IsType<TestException>(errorEntry.Item2);
                },
                errorEntry =>
                {
                    Assert.True((object)errorEntry.Item1 == (object)ruleD);
                    Assert.IsType<TestException>(errorEntry.Item2);
                });
        }

        [Fact]
        public void WhenNextRuleFaultsThenPreviousRulesActionIsPreserved()
        {
            var testContext = new TestContext();

            var pipeline = new FaultTolerantPromotionPipeline();
            var sku = testContext.CreateNewSku("S", 0);
            var ruleA = new SideEffectsPromotionRule(sku);
            var ruleB = new FaultyPromotionRule();
            pipeline.AddRule(ruleA);
            pipeline.AddRule(ruleB);

            ICart cart = pipeline.Apply(testContext.CartFactory.Create());

            Assert.NotNull(cart);
            Assert.NotEmpty(pipeline.LastApplyExceptions);
            Assert.Collection(cart, entry =>
            {
                Assert.True((object)entry.Sku == (object)sku);
            });
        }

        [Fact]
        public void WhenPreviousRuleFaultsThenNextRulesActionIsPreserved()
        {
            var testContext = new TestContext();

            var pipeline = new FaultTolerantPromotionPipeline();
            var sku = testContext.CreateNewSku("S", 0);
            var ruleA = new FaultyPromotionRule();
            var ruleB = new SideEffectsPromotionRule(sku);
            pipeline.AddRule(ruleA);
            pipeline.AddRule(ruleB);

            ICart cart = pipeline.Apply(testContext.CartFactory.Create());

            Assert.NotNull(cart);
            Assert.NotEmpty(pipeline.LastApplyExceptions);
            Assert.Collection(cart, entry =>
            {
                Assert.True((object)entry.Sku == (object)sku);
            });
        }

        [Fact]
        public void WhenAllRulesFailThenReturnedCartIsTheSameAsOriginal()
        {
            var testContext = new TestContext();

            var pipeline = new FaultTolerantPromotionPipeline();
            var ruleA = new FaultyPromotionRule();
            var ruleB = new FaultyPromotionRule();
            pipeline.AddRule(ruleA);
            pipeline.AddRule(ruleB);

            ICart originalCart = testContext.CartFactory.Create();
            ICart newCart = pipeline.Apply(originalCart);

            Assert.NotNull(newCart);
            Assert.True((object)newCart == (object)originalCart);
        }

        #region Types

        private class TestPromotionPipeline : FaultTolerantPromotionPipeline
        {
            public IList<IPromotionRule> RunHistory { get; private set; }

            public TestPromotionPipeline() : base()
            {
                this.RunHistory = new List<IPromotionRule>();
            }

            protected override ICart RunRule(IPromotionRule rule, ICart cart)
            {
                this.RunHistory.Add(rule);
                return base.RunRule(rule, cart);
            }
        }

        private class PromotionRuleA : IPromotionRule
        {
            public ICart Evaluate(ICart originalCart)
            {
                return originalCart.Clone() as ICart;
            }
        }

        private class PromotionRuleB : IPromotionRule
        {
            public ICart Evaluate(ICart originalCart)
            {
                return originalCart.Clone() as ICart;
            }
        }

        private class SideEffectsPromotionRule : IPromotionRule
        {
            private readonly Sku sku;
            public SideEffectsPromotionRule(Sku sku)
            {
                this.sku = sku;
            }

            public ICart Evaluate(ICart originalCart)
            {
                var newCart = originalCart.Clone() as ICart;
                newCart.Add(this.sku);

                return newCart;
            }
        }

        private class FaultyPromotionRule : IPromotionRule
        {
            public ICart Evaluate(ICart originalCart)
            {
                throw new TestException();
            }
        }

        private class TestException : Exception
        {
        }

        #endregion
    }
}
