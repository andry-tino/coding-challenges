using System;

using Xunit;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="SkuCartEntry"/>.
    /// </summary>
    public class SkuCartEntryTests
    {
        [Fact]
        public void CloneKeepsReferenceToTheSameSku()
        {
            var skuentry = new SkuCartEntry()
            {
                Sku = new Sku("A"),
                Price = 10,
                Quantity = 1,
                PromotionRuleId = "Rule x",
                Description = "Description"
            };
            var cloned = skuentry.Clone() as SkuCartEntry;

            Assert.True((object)skuentry.Sku == (object)cloned.Sku);
        }

        [Fact]
        public void CloneCreatesDifferentInstance()
        {
            var skuentry = new SkuCartEntry()
            {
                Sku = new Sku("A"),
                Price = 10,
                Quantity = 1,
                PromotionRuleId = "Rule x",
                Description = "Description"
            };
            var cloned = skuentry.Clone() as SkuCartEntry;

            Assert.False((object)skuentry == (object)cloned);
        }

        [Theory]
        [InlineData(false, 10, 1, null, null)]
        [InlineData(false, 10, 1, "Rule x", null)]
        [InlineData(false, 10, 1, null, "Description")]
        [InlineData(false, 10, 1, "Rule x", "Description")]
        [InlineData(true, 10, 1, null, null)]
        [InlineData(true, 10, 1, "Rule x", null)]
        [InlineData(true, 10, 1, null, "Description")]
        [InlineData(true, 10, 1, "Rule x", "Description")]
        public void ToStringReturnsANonEmptyString(bool nullSku, decimal price,
            int quantity, string promotionRule, string description)
        {
            Assert.False(string.IsNullOrEmpty(new SkuCartEntry()
            {
                Sku = nullSku ? null : new Sku("A"),
                Price = price,
                Quantity = quantity,
                PromotionRuleId = promotionRule,
                Description = description
            }.ToString()));
        }
    }
}
