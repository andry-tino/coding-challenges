using System;

using Xunit;

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

        }

        [Fact]
        public void WhenCollectionIsNotEmptyThenGetAllReturnsInfoCollection()
        {

        }

        [Fact]
        public void WhenCartDoesNotExistThenGetCartReturnsExceptionInMessage()
        {

        }

        [Fact]
        public void WhenCartExistsThenGetCartReturnsCartContent()
        {

        }

        [Fact]
        public void WhenCreatingCartThenNewCartIsAddedToCollection()
        {

        }

        [Fact]
        public void WhenCheckoutNonExistingCartThenExceptionIsReturnedInMessage()
        {

        }

        [Fact]
        public void WhenCheckoutAlreadyCheckedoutCartThenExceptionIsReturnedInMessage()
        {

        }

        [Fact]
        public void WhenCheckoutEmptyCartThenExceptionIsReturnedInMessage()
        {

        }

        [Fact]
        public void WhenCheckoutCartThenCartMarkedInCollectionAndNotRemoved()
        {

        }

        [Fact]
        public void WhenAddingAndNoSkuIdSpecifiedThenExceptionIsReturnedInMessage()
        {

        }

        [Fact]
        public void WhenAddingToNonExistingCartThenExceptionIsReturnedInMessage()
        {

        }

        [Fact]
        public void WhenAddingToCartAndSkuCouldNotBeFoundThenExceptionIsReturnedInMessage()
        {

        }

        [Fact]
        public void WhenAddingToCheckedOutCartThenExceptionIsReturnedInMessage()
        {

        }

        [Fact]
        public void WhenAddingToCartThenCartIsUpdatedInCollection()
        {

        }

        [Fact]
        public void WhenDeletingNonExistingCartThenExceptionIsReturnedInMessage()
        {

        }

        [Fact]
        public void WhenDeletingCartThenCollectionShrinks()
        {

        }

        [Fact]
        public void CannotDeleteCheckedOutCart()
        {

        }
    }
}
