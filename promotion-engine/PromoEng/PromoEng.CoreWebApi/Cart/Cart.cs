using System;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Describes a cart.
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// Gets the unique identifier of this cart.
        /// </summary>
        public string Id => Guid.NewGuid().ToString();
    }
}
