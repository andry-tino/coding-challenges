using System;
using System.Linq;

using Xunit;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="StandardCart"/>.
    /// </summary>
    public class StandardCartTests
    {
        [Fact]
        public void WhenCreatedThenCartIsEmpty()
        {
            Assert.Equal(0, new StandardCart().Count);
        }

        [Fact]
        public void WhenAddingASkuThenQuantityIsUnit()
        {
            ICart cart = new StandardCart();
            cart.Add(new Sku("A", 0));

            Assert.Equal(1, cart.First().Quantity);
        }

        [Fact]
        public void WhenAddingASkuThenPriceIsSameAsUnit()
        {
            ICart cart = new StandardCart();
            decimal price = 100;
            cart.Add(new Sku("A", price));

            Assert.Equal(price, cart.First().Price);
        }

        [Fact]
        public void WhenAddingASkuWithQuantityThenPriceIsUnitByQuantity()
        {
            ICart cart = new StandardCart();
            decimal price = 100;
            int quantity = 3;
            cart.Add(new Sku("A", price), quantity);

            Assert.Equal(price * quantity, cart.First().Price);
        }

        [Fact]
        public void TotalIsTheSumOfAllEntriesPrice()
        {
            decimal priceA = 100;
            decimal priceB = 50;
            decimal priceC = 20;

            ICart cart = new StandardCart();
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

            ICart cart = new StandardCart();
            cart.Add(new Sku("A", 0));
            cart.Add(new SkuCartEntry() { Sku = new Sku("B", 0), Quantity = quantityB });
            cart.Add(new SkuCartEntry() { Sku = new Sku("C", 0), Quantity = quantityC });

            Assert.Equal(quantityA + quantityB + quantityC, cart.Count);
        }

        [Fact]
        public void WhenAddingItemsThenTotalChanges()
        {
            ICart cart = new StandardCart();
            Assert.Equal(0, cart.Total);

            cart.Add(new Sku("A", 10));
            Assert.NotEqual(0, cart.Total);
        }

        [Fact]
        public void WhenMergingTwoEmptyCartsThenResultingCartIsEmpty()
        {
            ICart cart1 = new StandardCart();
            ICart cart2 = new StandardCart();

            ICart mergedCart = cart1.Merge(cart2);

            Assert.Equal(0, mergedCart.Count);
        }

        [Fact]
        public void WhenMergingWithAnEmptyCartThenResultingCartIsSameAsOriginal()
        {
            ICart cart = new StandardCart();
            var id = "A";
            cart.Add(new Sku(id, 0));

            ICart mergedCart = cart.Merge(new StandardCart());

            Assert.Equal(cart.Count, mergedCart.Count);
        }

        [Fact]
        public void MergeIsCommutative()
        {
            var idA = "A";
            var idB = "B";
            var idC = "C";

            ICart cart1 = new StandardCart();
            cart1.Add(new Sku(idA, 0));

            ICart cart2 = new StandardCart();
            cart2.Add(new Sku(idB, 0));
            cart2.Add(new Sku(idC, 0));

            ICart mergedCart1 = cart1.Merge(cart2);
            ICart mergedCart2 = cart2.Merge(cart1);

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

            ICart cart1 = new StandardCart();
            cart1.Add(new Sku(idA, 0));

            ICart cart2 = new StandardCart();
            cart2.Add(new Sku(idB, 0));
            cart2.Add(new Sku(idC, 0));

            ICart mergedCart = cart1.Merge(cart2);

            Assert.Equal(3, mergedCart.Count);
            Assert.Equal(1, mergedCart.Count(entry => entry.Sku.Id == idA));
            Assert.Equal(1, mergedCart.Count(entry => entry.Sku.Id == idB));
            Assert.Equal(1, mergedCart.Count(entry => entry.Sku.Id == idC));
        }

        [Fact]
        public void CountWithEntriesHavingNonUnitQuantities()
        {
            ICart cart = new StandardCart();
            cart.Add(new SkuCartEntry()
            { 
                Sku = new Sku("A", 0),
                Quantity = 2
            });
            cart.Add(new Sku("B", 0));

            Assert.Equal(3, cart.Count);
        }
    }
}
