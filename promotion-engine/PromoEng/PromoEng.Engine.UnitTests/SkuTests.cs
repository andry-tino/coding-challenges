using System;

using Xunit;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="Sku"/>.
    /// </summary>
    public class SkuTests
    {
        [Fact]
        public void WhenIdIsEmptyThenException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var sku = new Sku("", 0);
            });
        }

        [Fact]
        public void WhenIdIsWhitespaceThenException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var sku = new Sku(" ", 0);
            });
        }

        [Fact]
        public void WhenIdIsWhitespacesThenException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var sku = new Sku("  ", 0);
            });
        }

        [Fact]
        public void WhenIdIsNullThenException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var sku = new Sku(null, 0);
            });
        }

        [Fact]
        public void WhenNoFriendlyNameIsProvidedThenIdIsUsed()
        {
            var sku = new Sku("A", 0);
            Assert.Equal(sku.Id, sku.Name);
        }

        [Fact]
        public void UnitPriceIsAPositiveQuantity()
        {
            float price = -20;
            var sku = new Sku("A", price);
            Assert.Equal(Math.Abs(price), sku.UnitPrice);
        }
    }
}
