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
        public void WhenCartDoesNotExistThenGetCartReturnsNull()
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
        public void WhenCheckoutNonExistingCartThenNoChangesInCollection()
        {

        }

        [Fact]
        public void WhenCheckoutCartThenCartMarkedInCollectionAndNotRemoved()
        {

        }

        [Fact]
        public void WhenAddingToNonExistingCartThenNoChangesInCollection()
        {

        }

        [Fact]
        public void WhenAddingToCartThenCartIsUpdatedInCollection()
        {

        }

        [Fact]
        public void WhenDeletingNonExistingCartThenNoChangesInCollection()
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
