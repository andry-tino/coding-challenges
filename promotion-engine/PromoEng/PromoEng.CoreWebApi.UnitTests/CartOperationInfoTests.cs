using System;

using Xunit;

namespace PromoEng.CoreWebApi.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="CartOperationInfo{T}"/>.
    /// </summary>
    public class CartOperationInfoTests
    {
        [Fact]
        public void WhenCreatedWithErrorAndNoExceptionThenExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cartOperationInfo = new CartOperationInfo<string>(CartOperationType.Read,
                    CartOperationStatus.Error);
            });
        }

        [Fact]
        public void WhenCreatedWithoutErrorAndExceptionThenExceptionIsThrown()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var cartOperationInfo = new CartOperationInfo<string>(CartOperationType.Read,
                    CartOperationStatus.Successful, new Exception());
            });
        }
    }
}
