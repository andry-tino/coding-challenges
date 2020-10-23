using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using PromoEng.Engine;

namespace PromoEng.CoreWebApi.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="SkuPriceListFactory"/>.
    /// </summary>
    public class SkuPriceListFactoryTests
    {
        [Fact]
        public void WhenCreatedWithNullInputThenExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var priceListFactory = new SkuPriceListFactory(null);
            });
        }

        [Fact]
        public void WhenCreatedWithEmptyInputThenEmptyPriceListIsCreated()
        {
            var priceListFactory = new SkuPriceListFactory(new SkuOption[0]);
            var priceList = priceListFactory.Create();

            Assert.Empty(priceList);
        }

        [Theory]
        [InlineData("A", "SKU A", 100)]
        [InlineData("A", null, 100)]
        public void CreatePriceList(string id, string name, decimal unitPrice)
        {
            var priceListFactory = new SkuPriceListFactory(new[]
            {
                new SkuOption() { Id = id, Name = name, UnitPrice = unitPrice }
            });
            var priceList = priceListFactory.Create();

            Assert.NotEmpty(priceList);
            Assert.Equal(1, priceList.Count);

            Assert.Equal(1, priceList.Keys.Count(sku => sku.Id == id));
            Assert.Equal(unitPrice, priceList.First(pair => pair.Key.Id == id).Value);
        }
    }
}
