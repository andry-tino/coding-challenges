using System;
using System.Collections.Generic;

using PromoEng.Engine;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents a cart content.
    /// </summary>
    public class CartContent : CartInfo
    {
        /// <summary>
        /// Gets or sets the collection of <see cref="Sku"/>.
        /// </summary>
        public IEnumerable<Sku> Skus { get; set; }

        /// <summary>
        /// Gets or sets the total value of the cart at checkout.
        /// </summary>
        public decimal Total { get; set; }
    }
}
