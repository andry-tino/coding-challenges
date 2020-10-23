using System;
using System.Collections.Generic;

using Xunit;

namespace PromoEng.CoreWebApi.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="CartInfo"/>.
    /// </summary>
    public class CartInfoTests
    {
        [Fact]
        public void WhenCreatedThenNewIdIsAutomaticallyGenerated()
        {
            var cartInfo = new CartInfo();
            Assert.NotNull(cartInfo.Id);
        }

        [Fact]
        public void WhenCreatedThenCartIsMarkedAsNotCheckedout()
        {
            var cartInfo = new CartInfo();
            Assert.False(cartInfo.CheckedOut);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(30)]
        [InlineData(50)]
        public void IdsAreUnique(int count)
        {
            IDictionary<string, CartInfo> cartInfos = new Dictionary<string, CartInfo>();

            for (int i = 0; i < count; i++)
            {
                var cartInfo = new CartInfo();

                Assert.False(cartInfos.TryGetValue(cartInfo.Id, out CartInfo duplicate));

                cartInfos.Add(cartInfo.Id, cartInfo);
            }
        }
    }
}
