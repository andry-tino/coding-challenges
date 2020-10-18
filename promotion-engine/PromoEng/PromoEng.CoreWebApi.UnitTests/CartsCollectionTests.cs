using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using PromoEng.Testability;

namespace PromoEng.CoreWebApi.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="CartsCollection"/>.
    /// </summary>
    public class CartsCollectionTests
    {
        [Fact]
        public void WhenCreatedThenCollectionIsEmpty()
        {
            var cartsCollection = new TestCartsCollection();
            Assert.Equal(0, cartsCollection.Count);
        }

        [Fact]
        public void CartsCollectionEntryUsesTheIdInTheInfoHeader()
        {
            var testContext = new TestContext();

            var cartInfo = new CartInfo();
            var cartEntry = new CartsCollection.CartsCollectionEntry(cartInfo,
                testContext.CartFactory.Create());

            Assert.Equal(cartEntry.Id, cartInfo.Id);
        }

        [Fact]
        public void WhenAddingNullThenNoChangesInCollection()
        {
            var cartsCollection = new TestCartsCollection();
            cartsCollection.Add(null);

            Assert.Equal(0, cartsCollection.Count);
        }

        [Fact]
        public void WhenAddingAnExistingCartThenExceptionIsThrown()
        {
            var testContext = new TestContext();

            var cartsCollection = new TestCartsCollection();
            var id = "001";
            var cartInfo1 = new TestCartInfo(id);
            var cartInfo2 = new TestCartInfo(id);
            var cartEntry1 = new CartsCollection.CartsCollectionEntry(cartInfo1,
                testContext.CartFactory.Create());
            var cartEntry2 = new CartsCollection.CartsCollectionEntry(cartInfo2,
                testContext.CartFactory.Create());

            cartsCollection.Add(cartEntry1);

            Assert.Throws<ArgumentException>(() =>
            {
                cartsCollection.Add(cartEntry2);
            });
        }

        [Fact]
        public void AddCartEntryToCollection()
        {
            var testContext = new TestContext();

            var cartsCollection = new TestCartsCollection();
            var cartEntry = new CartsCollection.CartsCollectionEntry(new CartInfo(),
                testContext.CartFactory.Create());

            cartsCollection.Add(cartEntry);

            Assert.Equal(1, cartsCollection.Count);
        }

        [Fact]
        public void WhenRetrievingNullKeyThenNullIsReturned()
        {
            Assert.Null(new TestCartsCollection().Retrieve(null));
        }

        [Fact]
        public void WhenRetrievingNonExistingKeyThenNullIsReturned()
        {
            Assert.Null(new TestCartsCollection().Retrieve("001"));
        }

        [Fact]
        public void RetrieveCartEntryFromCollection()
        {
            var testContext = new TestContext();

            var cartsCollection = new TestCartsCollection();
            var cartEntry = new CartsCollection.CartsCollectionEntry(new CartInfo(),
                testContext.CartFactory.Create());

            cartsCollection.Add(cartEntry);
            var retrievedCartEntry = cartsCollection.Retrieve(cartEntry.Id);

            Assert.True((object)cartEntry == (object)retrievedCartEntry);
        }

        [Fact]
        public void WhenCollectionIsEmptyThenRetrieveAllReturnsEmptyCollection()
        {
            Assert.Empty(new TestCartsCollection().Retrieve());
        }

        [Fact]
        public void RetrieveAll()
        {
            var testContext = new TestContext();

            var cartsCollection = new TestCartsCollection();
            var cartEntry1 = new CartsCollection.CartsCollectionEntry(new CartInfo(),
                testContext.CartFactory.Create());
            var cartEntry2 = new CartsCollection.CartsCollectionEntry(new CartInfo(),
                testContext.CartFactory.Create());

            cartsCollection.Add(cartEntry1);
            cartsCollection.Add(cartEntry2);
            var retrievedCartEntries = cartsCollection.Retrieve();

            Assert.Equal(2, retrievedCartEntries.Count());
        }

        [Fact]
        public void WhenRemovingNullThenNoChangesInCollectionAndNullIsReturned()
        {
            var cartsCollection = new TestCartsCollection();

            Assert.Equal(0, cartsCollection.Count);
            Assert.Null(cartsCollection.Remove(null));
            Assert.Equal(0, cartsCollection.Count);
        }

        [Fact]
        public void WhenRemovingNonExistingCartThenNoChangesInCollectionAndNullIsReturned()
        {
            var cartsCollection = new TestCartsCollection();

            Assert.Equal(0, cartsCollection.Count);
            Assert.Null(cartsCollection.Remove("001"));
            Assert.Equal(0, cartsCollection.Count);
        }

        [Fact]
        public void RemoveCartEntryFromCollection()
        {
            var testContext = new TestContext();

            var cartsCollection = new TestCartsCollection();
            var cartEntry = new CartsCollection.CartsCollectionEntry(new CartInfo(),
                testContext.CartFactory.Create());

            cartsCollection.Add(cartEntry);
            Assert.Equal(1, cartsCollection.Count);

            var removedCartEntry = cartsCollection.Remove(cartEntry.Id);
            Assert.True((object)cartEntry == (object)removedCartEntry);
            Assert.Equal(0, cartsCollection.Count);
        }

        #region Types

        private class TestCartsCollection : CartsCollection
        {
            public int Count => this.carts.Count;
        }

        private class TestCartInfo : CartInfo
        {
            public TestCartInfo(string forcedId) : base()
            {
                this.Id = forcedId;
            }
        }

        #endregion
    }
}
