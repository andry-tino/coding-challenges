using System;
using System.Linq;

using Xunit;

using PromoEng.Testability;

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
            var testContext = new TestContext();

            Assert.Equal(0, testContext.CartFactory.Create().Count);
        }

        [Fact]
        public void WhenAddingASkuThenQuantityIsUnit()
        {
            var testContext = new TestContext();

            ICart cart = testContext.CartFactory.Create();
            cart.Add(testContext.CreateNewSku("A", 100));

            Assert.Equal(1, cart.First().Quantity);
        }

        [Fact]
        public void WhenAddingASkuThenPriceIsSameAsItsUnitPrice()
        {
            var testContext = new TestContext();

            ICart cart = testContext.CartFactory.Create();
            Sku sku = testContext.CreateNewSku("A", 100);
            cart.Add(sku);

            Assert.Equal(testContext.PriceList[sku], cart.First().Price);
        }

        [Fact]
        public void WhenAddingASkuWithQuantityThenPriceIsUnitByQuantity()
        {
            var testContext = new TestContext();

            ICart cart = testContext.CartFactory.Create();
            Sku sku = testContext.CreateNewSku("A", 100);
            int quantity = 3;
            cart.Add(sku, quantity);

            Assert.Equal(testContext.PriceList[sku] * quantity, cart.First().Price);
        }

        [Fact]
        public void WhenAddingASkuWithZeroUnitPriceThenTotalDoesNotChange()
        {
            var testContext = new TestContext();

            ICart cart = testContext.CartFactory.Create();

            cart.Add(testContext.CreateNewSku("A", 100));
            var total1 = cart.Total;
            Assert.NotEqual(0, total1);

            cart.Add(testContext.CreateNewSku("Z", 0));
            var total2 = cart.Total;
            Assert.Equal(total1, total2);
        }

        [Fact]
        public void TotalIsTheSumOfAllEntriesPrice()
        {
            var testContext = new TestContext();

            Sku sku1 = testContext.CreateNewSku("A", 100);
            Sku sku2 = testContext.CreateNewSku("B", 200);
            Sku sku3 = testContext.CreateNewSku("C", 300);

            ICart cart = testContext.CartFactory.Create();
            cart.Add(sku1);
            cart.Add(sku2);
            cart.Add(sku3);

            Assert.Equal(testContext.PriceList[sku1] + testContext.PriceList[sku2] + testContext.PriceList[sku3], cart.Total);
        }

        [Fact]
        public void CountIsTheSumOfAllEntriesQuantity()
        {
            var testContext = new TestContext();

            Sku sku1 = testContext.CreateNewSku("A", 100);
            Sku sku2 = testContext.CreateNewSku("B", 200);
            Sku sku3 = testContext.CreateNewSku("C", 300);
            int quantity1 = 1;
            int quantity2 = 2;
            int quantity3 = 3;

            ICart cart = testContext.CartFactory.Create();
            cart.Add(sku1);
            cart.Add(new SkuCartEntry() { Sku = sku2, Quantity = quantity2 });
            cart.Add(new SkuCartEntry() { Sku = sku3, Quantity = quantity3 });

            Assert.Equal(quantity1 + quantity2 + quantity3, cart.Count);
        }

        [Fact]
        public void WhenAddingItemsThenTotalChanges()
        {
            var testContext = new TestContext();

            ICart cart = testContext.CartFactory.Create();
            Assert.Equal(0, cart.Total);

            cart.Add(testContext.CreateNewSku("A", 100));
            Assert.NotEqual(0, cart.Total);
        }

        [Fact]
        public void WhenMergingTwoEmptyCartsThenResultingCartIsEmpty()
        {
            var testContext = new TestContext();

            ICart cart1 = testContext.CartFactory.Create();
            ICart cart2 = testContext.CartFactory.Create();

            ICart mergedCart = cart1.Merge(cart2);

            Assert.Equal(0, mergedCart.Count);
        }

        [Fact]
        public void WhenMergingWithAnEmptyCartThenResultingCartHasSameTotalAsOriginal()
        {
            var testContext = new TestContext();

            ICart cart = testContext.CartFactory.Create();
            cart.Add(testContext.CreateNewSku("A", 100));

            ICart mergedCart = cart.Merge(testContext.CartFactory.Create());

            Assert.Equal(cart.Count, mergedCart.Count);
        }

        [Fact]
        public void MergeIsCommutative()
        {
            var testContext = new TestContext();

            Sku sku1 = testContext.CreateNewSku("A", 100);
            Sku sku2 = testContext.CreateNewSku("B", 200);
            Sku sku3 = testContext.CreateNewSku("C", 300);

            ICart cart1 = testContext.CartFactory.Create();
            cart1.Add(sku1);

            ICart cart2 = testContext.CartFactory.Create();
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
            var testContext = new TestContext();

            Sku sku1 = testContext.CreateNewSku("A", 100);
            Sku sku2 = testContext.CreateNewSku("B", 200);
            Sku sku3 = testContext.CreateNewSku("C", 300);

            ICart cart1 = testContext.CartFactory.Create();
            cart1.Add(sku1);

            ICart cart2 = testContext.CartFactory.Create();
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
            var testContext = new TestContext();

            ICart cart = testContext.CartFactory.Create();
            cart.Add(new SkuCartEntry()
            { 
                Sku = testContext.CreateNewSku("A", 100),
                Quantity = 2
            });
            cart.Add(testContext.CreateNewSku("B", 200));

            Assert.Equal(3, cart.Count);
        }

        [Fact]
        public void WhenSkuIsNotInPricelistThenAddRaisesException()
        {
            var testContext = new TestContext();

            ICart cart = testContext.CartFactory.Create();

            decimal registeredSkuPrice = 100;
            var registeredSku = testContext.CreateNewSku("A", registeredSkuPrice);

            cart.Add(registeredSku);
            Assert.Equal(registeredSkuPrice, cart.Total);

            var unregisteredSku = new Sku("U");
            Assert.Throws<StandardCart.SkuNotFoundInPriceListException>(() =>
            {
                cart.Add(unregisteredSku); // Adding SKU but no price registered in pricelist
            });
        }
    }
}
