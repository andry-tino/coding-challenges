using System;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents a cart.
    /// </summary>
    public class CartInfo : CartOperationInfo
    {
        /// <summary>
        /// Gets the unique identifier of this cart.
        /// </summary>
        public string Id => Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets a value indicating whether the cart has been checked out or not.
        /// </summary>
        public bool CheckedOut { get; set; }
    }
}
