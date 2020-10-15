using System;
using System.Linq;

using Xunit;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="Cart"/>.
    /// </summary>
    public class CartTests
    {
        [Fact]
        public void WhenCreatedThenCartIsEmpty()
        {
            Assert.Equal(0, new Cart().Count);
        }

        [Fact]
        public void WhenAddingASkuThenQuantityIsUnit()
        {
            var cart = new Cart();
            cart.Add(new Sku("A", 0));

            Assert.Equal(1, cart.First().Quantity);
        }

        [Fact]
        public void WhenAddingASkuThenPriceIsSameAsUnit()
        {
            var cart = new Cart();
            decimal price = 100;
            cart.Add(new Sku("A", price));

            Assert.Equal(price, cart.First().Price);
        }

        [Fact]
        public void TotalIsTheSumOfAllEntriesPrice()
        {
            decimal priceA = 100;
            decimal priceB = 50;
            decimal priceC = 20;

            var cart = new Cart();
            cart.Add(new Sku("A", priceA));
            cart.Add(new Sku("B", priceB));
            cart.Add(new Sku("C", priceC));

            Assert.Equal(priceA + priceB + priceC, cart.Total);
        }

        [Fact]
        public void CountIsTheSumOfAllEntriesQuantity()
        {
            int quantityA = 1;
            int quantityB = 2;
            int quantityC = 3;

            var cart = new Cart();
            cart.Add(new Sku("A", 0));
            cart.Add(new Cart.SkuCartEntry() { Sku = new Sku("B", 0), Quantity = quantityB });
            cart.Add(new Cart.SkuCartEntry() { Sku = new Sku("C", 0), Quantity = quantityC });

            Assert.Equal(quantityA + quantityB + quantityC, cart.Count);
        }

        [Fact]
        public void WhenAddingItemsThenTotalChanges()
        {
            var cart = new Cart();
            Assert.Equal(0, cart.Total);

            cart.Add(new Sku("A", 10));
            Assert.NotEqual(0, cart.Total);
        }

        [Fact]
        public void WhenMergingTwoEmptyCartsThenResultingCartIsEmpty()
        {
            var cart1 = new Cart();
            var cart2 = new Cart();

            var mergedCart = cart1.Merge(cart2);

            Assert.Equal(0, mergedCart.Count);
        }

        [Fact]
        public void WhenMergingWithAnEmptyCartThenResultingCartIsSameAsOriginal()
        {
            var cart = new Cart();
            var id = "A";
            cart.Add(new Sku(id, 0));

            var mergedCart = cart.Merge(new Cart());

            Assert.Equal(cart.Count, mergedCart.Count);
        }

        [Fact]
        public void MergeIsCommutative()
        {
            var idA = "A";
            var idB = "B";
            var idC = "C";

            var cart1 = new Cart();
            cart1.Add(new Sku(idA, 0));

            var cart2 = new Cart();
            cart2.Add(new Sku(idB, 0));
            cart2.Add(new Sku(idC, 0));

            var mergedCart1 = cart1.Merge(cart2);
            var mergedCart2 = cart2.Merge(cart1);

            Assert.Equal(mergedCart1.Count, mergedCart2.Count);
            Assert.Equal(1, mergedCart1.Count(entry => entry.Sku.Id == idA));
            Assert.Equal(1, mergedCart1.Count(entry => entry.Sku.Id == idB));
            Assert.Equal(1, mergedCart1.Count(entry => entry.Sku.Id == idC));
            Assert.Equal(1, mergedCart2.Count(entry => entry.Sku.Id == idA));
            Assert.Equal(1, mergedCart2.Count(entry => entry.Sku.Id == idB));
            Assert.Equal(1, mergedCart2.Count(entry => entry.Sku.Id == idC));
        }

        [Fact]
        public void MergingTwoCarts()
        {
            var idA = "A";
            var idB = "B";
            var idC = "C";

            var cart1 = new Cart();
            cart1.Add(new Sku(idA, 0));

            var cart2 = new Cart();
            cart2.Add(new Sku(idB, 0));
            cart2.Add(new Sku(idC, 0));

            var mergedCart = cart1.Merge(cart2);

            Assert.Equal(3, mergedCart.Count);
            Assert.Equal(1, mergedCart.Count(entry => entry.Sku.Id == idA));
            Assert.Equal(1, mergedCart.Count(entry => entry.Sku.Id == idB));
            Assert.Equal(1, mergedCart.Count(entry => entry.Sku.Id == idC));
        }

        [Fact]
        public void CountWithEntriesHavingNonUnitQuantities()
        {
            var cart = new Cart();
            cart.Add(new Cart.SkuCartEntry()
            { 
                Sku = new Sku("A", 0),
                Quantity = 2
            });
            cart.Add(new Sku("B", 0));

            Assert.Equal(3, cart.Count);
        }
    }
}
