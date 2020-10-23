using System;

using Xunit;

namespace PromoEng.Engine.Scenarios
{
    /// <summary>
    /// Class collecting test utilities.
    /// </summary>
    internal static class TestUtilities
    {
        /// <summary>
        /// Checks the cart total is expected.
        /// </summary>
        /// <param name="cart">The cart to check.</param>
        /// <param name="expected">The expected value for <see cref="ICart.Total"/>.</param>
        public static void TestTotal(this ICart cart, decimal expected)
        {
            Assert.Equal(expected, cart.Total);
        }
    }
}
