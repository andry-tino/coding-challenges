using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="StandardCart"/>.
    /// </summary>
    public class StandardCartTests
    {
        private static Sku skuA = new Sku("A");
        private static Sku skuB = new Sku("B");
        private static Sku skuC = new Sku("C");
        private static Sku skuZ = new Sku("Z");
        private static IDictionary<Sku, decimal> priceList = new Dictionary<Sku, decimal>(new[]
        {
            new KeyValuePair<Sku, decimal>(skuA, 100),
            new KeyValuePair<Sku, decimal>(skuB, 200),
            new KeyValuePair<Sku, decimal>(skuC, 300),
            new KeyValuePair<Sku, decimal>(skuZ, 0)
        });

        [Fact]
        public void WhenCreatedThenCartIsEmpty()
        {
            Assert.Equal(0, new StandardCart(priceList).Count);
        }

        [Fact]
        public void WhenAddingASkuThenQuantityIsUnit()
        {
            ICart cart = new StandardCart(priceList);
            cart.Add(skuA);

            Assert.Equal(1, cart.First().Quantity);
        }

        [Fact]
        public void WhenAddingASkuThenPriceIsSameAsItsUnitPrice()
        {
            ICart cart = new StandardCart(priceList);
            Sku sku = skuA;
            cart.Add(sku);

            Assert.Equal(priceList[sku], cart.First().Price);
        }

        [Fact]
        public void WhenAddingASkuWithQuantityThenPriceIsUnitByQuantity()
        {
            ICart cart = new StandardCart(priceList);
            Sku sku = skuA;
            int quantity = 3;
            cart.Add(sku, quantity);

            Assert.Equal(priceList[sku] * quantity, cart.First().Price);
        }

        [Fact]
        public void WhenAddingASkuWithZeroUnitPriceThenTotalDoesNotChange()
        {
            ICart cart = new StandardCart(priceList);

            cart.Add(skuA);
            var total1 = cart.Total;
            Assert.NotEqual(0, total1);

            cart.Add(skuZ);
            var total2 = cart.Total;
            Assert.Equal(total1, total2);
        }

        [Fact]
        public void TotalIsTheSumOfAllEntriesPrice()
        {
            Sku sku1 = skuA;
            Sku sku2 = skuB;
            Sku sku3 = skuC;

            ICart cart = new StandardCart(priceList);
            cart.Add(sku1);
            cart.Add(sku2);
            cart.Add(sku3);

            Assert.Equal(priceList[sku1] + priceList[sku2] + priceList[sku3], cart.Total);
        }

        [Fact]
        public void CountIsTheSumOfAllEntriesQuantity()
        {
            Sku sku1 = skuA;
            Sku sku2 = skuB;
            Sku sku3 = skuC;
            int quantity1 = 1;
            int quantity2 = 2;
            int quantity3 = 3;

            ICart cart = new StandardCart(priceList);
            cart.Add(sku1);
            cart.Add(new SkuCartEntry() { Sku = sku2, Quantity = quantity2 });
            cart.Add(new SkuCartEntry() { Sku = sku3, Quantity = quantity3 });

            Assert.Equal(quantity1 + quantity2 + quantity3, cart.Count);
        }

        [Fact]
        public void WhenAddingItemsThenTotalChanges()
        {
            ICart cart = new StandardCart(priceList);
            Assert.Equal(0, cart.Total);

            cart.Add(skuA);
            Assert.NotEqual(0, cart.Total);
        }

        [Fact]
        public void WhenMergingTwoEmptyCartsThenResultingCartIsEmpty()
        {
            ICart cart1 = new StandardCart(priceList);
            ICart cart2 = new StandardCart(priceList);

            ICart mergedCart = cart1.Merge(cart2);

            Assert.Equal(0, mergedCart.Count);
        }

        [Fact]
        public void WhenMergingWithAnEmptyCartThenResultingCartHasSameTotalAsOriginal()
        {
            ICart cart = new StandardCart(priceList);
            cart.Add(skuA);

            ICart mergedCart = cart.Merge(new StandardCart(priceList));

            Assert.Equal(cart.Count, mergedCart.Count);
        }

        [Fact]
        public void MergeIsCommutative()
        {
            Sku sku1 = skuA;
            Sku sku2 = skuB;
            Sku sku3 = skuC;

            ICart cart1 = new StandardCart(priceList);
            cart1.Add(sku1);

            ICart cart2 = new StandardCart(priceList);
            cart2.Add(sku2);
            cart2.Add(sku3);

            ICart mergedCart1 = cart1.Merge(cart2);
            ICart mergedCart2 = cart2.Merge(cart1);

            Assert.Equal(mergedCart1.Count, mergedCart2.Count);
            Assert.Equal(1, mergedCart1.Count(entry => entry.Sku == sku1));
            Assert.Equal(1, mergedCart1.Count(entry => entry.Sku == sku2));
            Assert.Equal(1, mergedCart1.Count(entry => entry.Sku == sku3));
            Assert.Equal(1, mergedCart2.Count(entry => entry.Sku == sku1));
            Assert.Equal(1, mergedCart2.Count(entry => entry.Sku == sku2));
            Assert.Equal(1, mergedCart2.Count(entry => entry.Sku == sku3));
        }

        [Fact]
        public void MergingTwoCarts()
        {
            Sku sku1 = skuA;
            Sku sku2 = skuB;
            Sku sku3 = skuC;

            ICart cart1 = new StandardCart(priceList);
            cart1.Add(sku1);

            ICart cart2 = new StandardCart(priceList);
            cart2.Add(sku2);
            cart2.Add(sku3);

            ICart mergedCart = cart1.Merge(cart2);

            Assert.Equal(3, mergedCart.Count);
            Assert.Equal(1, mergedCart.Count(entry => entry.Sku == sku1));
            Assert.Equal(1, mergedCart.Count(entry => entry.Sku == sku2));
            Assert.Equal(1, mergedCart.Count(entry => entry.Sku == sku3));
        }

        [Fact]
        public void CountWithEntriesHavingNonUnitQuantities()
        {
            ICart cart = new StandardCart(priceList);
            cart.Add(new SkuCartEntry()
            { 
                Sku = skuA,
                Quantity = 2
            });
            cart.Add(skuB);

            Assert.Equal(3, cart.Count);
        }
    }
}
