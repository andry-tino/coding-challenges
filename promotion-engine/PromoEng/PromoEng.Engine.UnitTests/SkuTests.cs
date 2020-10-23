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
                var sku = new Sku("");
            });
        }

        [Fact]
        public void WhenIdIsWhitespaceThenException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var sku = new Sku(" ");
            });
        }

        [Fact]
        public void WhenIdIsWhitespacesThenException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                var sku = new Sku("  ");
            });
        }

        [Fact]
        public void WhenIdIsNullThenException()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var sku = new Sku(null);
            });
        }

        [Fact]
        public void WhenNoFriendlyNameIsProvidedThenIdIsUsed()
        {
            var sku = new Sku("A");
            Assert.Equal(sku.Id, sku.Name);
        }
    }
}
