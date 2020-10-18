using System;
using System.Collections.Generic;

using Xunit;

using PromoEng.Testability;
using PromoEng.Engine;

namespace PromoEng.CoreWebApi.Controllers.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="CartController"/>
    /// </summary>
    public class CartControllerTests
    {
        [Fact]
        public void WhenCollectionIsEmptyThenGetAllReturnsEmptyArray()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new CartsCollection();
            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.GetAll();

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Successful, response.Status);
            Assert.Empty(response.Body);
        }

        [Fact]
        public void WhenCollectionIsNotEmptyThenGetAllReturnsInfoCollection()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new CartsCollection();
            inMemoryCollection.Add(new CartsCollection.CartsCollectionEntry(
                new CartInfo(), testContext.CartFactory.Create()));

            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.GetAll();

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Successful, response.Status);
            Assert.NotEmpty(response.Body);
            Assert.Single(response.Body);
        }

        [Fact]
        public void WhenCartDoesNotExistThenGetCartReturnsExceptionInMessage()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new CartsCollection();
            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.GetCart("001");

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Error, response.Status);
            Assert.NotNull(response.Exception);
            Assert.Null(response.Body);
        }

        [Fact]
        public void WhenCartExistsThenGetCartReturnsCartContent()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new CartsCollection();
            var cartEntry = new CartsCollection.CartsCollectionEntry(
                new CartInfo(), testContext.CartFactory.Create());
            inMemoryCollection.Add(cartEntry);

            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.GetCart(cartEntry.Id);

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Successful, response.Status);
            Assert.Null(response.Exception);
            Assert.NotNull(response.Body);
            Assert.NotNull(response.Body.Info);
            Assert.NotNull(response.Body.CartEntries);
        }

        [Fact]
        public void WhenCreatingCartThenNewCartIsAddedToCollection()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new TestCartsCollection();
            Assert.Equal(0, inMemoryCollection.Count);

            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Create();

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Successful, response.Status);
            Assert.NotNull(response.Body);
            Assert.Equal(1, inMemoryCollection.Count);
        }

        [Fact]
        public void WhenCheckoutNonExistingCartThenExceptionIsReturnedInMessage()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new TestCartsCollection();
            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Checkout("001");

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Error, response.Status);
            Assert.NotNull(response.Exception);
            Assert.Null(response.Body);
        }

        [Fact]
        public void WhenCheckoutAlreadyCheckedoutCartThenExceptionIsReturnedInMessage()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new CartsCollection();
            var cartEntry = new CartsCollection.CartsCollectionEntry(
                new CartInfo(), testContext.CartFactory.Create());
            cartEntry.Info.CheckedOut = true;
            inMemoryCollection.Add(cartEntry);

            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Checkout(cartEntry.Id);

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Error, response.Status);
            Assert.NotNull(response.Exception);
            Assert.Null(response.Body);
        }

        [Fact]
        public void WhenCheckoutEmptyCartThenExceptionIsReturnedInMessage()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new CartsCollection();
            var cartEntry = new CartsCollection.CartsCollectionEntry(
                new CartInfo(), testContext.CartFactory.Create());
            inMemoryCollection.Add(cartEntry);

            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Checkout(cartEntry.Id);

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Error, response.Status);
            Assert.NotNull(response.Exception);
            Assert.Null(response.Body);
        }

        [Fact]
        public void WhenCheckoutCartThenCartMarkedInCollectionAndNotRemoved()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new CartsCollection();
            var cartEntry = new CartsCollection.CartsCollectionEntry(
                new CartInfo(), testContext.CartFactory.Create());
            cartEntry.Cart.Add(testContext.CreateNewSku("A", 100));
            inMemoryCollection.Add(cartEntry);

            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Checkout(cartEntry.Id);

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Successful, response.Status);
            Assert.NotNull(response.Body);
            Assert.True(cartEntry.Info.CheckedOut);
        }

        [Fact]
        public void WhenAddingAndNoSkuIdSpecifiedThenExceptionIsReturnedInMessage()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new CartsCollection();
            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Add("001", null, null);

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Error, response.Status);
            Assert.NotNull(response.Exception);
            Assert.Null(response.Body);
        }

        [Fact]
        public void WhenAddingToNonExistingCartThenExceptionIsReturnedInMessage()
        {
            var testContext = new TestContext();

            var sku = testContext.CreateNewSku("A", 100);

            var inMemoryCollection = new CartsCollection();
            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Add("001", sku.Id, null);

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Error, response.Status);
            Assert.NotNull(response.Exception);
            Assert.Null(response.Body);
        }

        [Fact]
        public void WhenAddingToCartAndSkuCouldNotBeFoundThenExceptionIsReturnedInMessage()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new CartsCollection();
            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Add("001", "A", null);

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Error, response.Status);
            Assert.NotNull(response.Exception);
            Assert.Null(response.Body);
        }

        [Fact]
        public void WhenAddingToCheckedOutCartThenExceptionIsReturnedInMessage()
        {
            var testContext = new TestContext();

            var sku = testContext.CreateNewSku("A", 100);

            var inMemoryCollection = new CartsCollection();
            var cartEntry = new CartsCollection.CartsCollectionEntry(
                new CartInfo(), testContext.CartFactory.Create());
            cartEntry.Info.CheckedOut = true;
            inMemoryCollection.Add(cartEntry);

            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Add(cartEntry.Id, sku.Id, null);

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Error, response.Status);
            Assert.NotNull(response.Exception);
            Assert.Null(response.Body);
        }

        [Fact]
        public void WhenAddingToCartThenCartIsUpdatedInCollection()
        {
            var testContext = new TestContext();

            var sku = testContext.CreateNewSku("A", 100);

            var inMemoryCollection = new CartsCollection();
            var cartEntry = new CartsCollection.CartsCollectionEntry(
                new CartInfo(), testContext.CartFactory.Create());
            inMemoryCollection.Add(cartEntry);
            Assert.Empty(cartEntry.Cart);

            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Add(cartEntry.Id, sku.Id, null);

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Successful, response.Status);
            Assert.NotNull(response.Body);
            Assert.NotEmpty(cartEntry.Cart);
        }

        [Fact]
        public void WhenDeletingNonExistingCartThenExceptionIsReturnedInMessage()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new CartsCollection();
            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Delete("001");

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Error, response.Status);
            Assert.NotNull(response.Exception);
            Assert.Null(response.Body);
        }

        [Fact]
        public void WhenDeletingCartThenCollectionShrinks()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new TestCartsCollection();
            var cartEntry = new CartsCollection.CartsCollectionEntry(
                new CartInfo(), testContext.CartFactory.Create());
            inMemoryCollection.Add(cartEntry);
            Assert.NotEqual(0, inMemoryCollection.Count);

            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Delete(cartEntry.Id);

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Successful, response.Status);
            Assert.NotNull(response.Body);
            Assert.Equal(0, inMemoryCollection.Count);
        }

        [Fact]
        public void CannotDeleteCheckedOutCart()
        {
            var testContext = new TestContext();

            var inMemoryCollection = new CartsCollection();
            var cartEntry = new CartsCollection.CartsCollectionEntry(
                new CartInfo(), testContext.CartFactory.Create());
            cartEntry.Info.CheckedOut = true;
            inMemoryCollection.Add(cartEntry);
            var controller = new CartController(null, inMemoryCollection,
                testContext.CartFactory, testContext.PriceList, new NeutralPipeline());

            var response = controller.Delete(cartEntry.Id);

            Assert.NotNull(response);
            Assert.Equal(CartOperationStatus.Error, response.Status);
            Assert.NotNull(response.Exception);
            Assert.Null(response.Body);
        }

        #region Types

        private class NeutralPipeline : IPromotionPipeline
        {
            public ICart Apply(ICart cart)
            {
                return cart;
            }
        }

        private class TestCartsCollection : CartsCollection
        {
            public int Count => this.carts.Count;
        }

        #endregion
    }
}
