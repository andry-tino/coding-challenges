using System;
using System.Collections.Generic;

using PromoEng.Engine;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents a cart content.
    /// </summary>
    public class CartContent
    {
        /// <summary>
        /// The info header of the cart.
        /// </summary>
        public CartInfo Info { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="Sku"/>.
        /// </summary>
        public IEnumerable<CartEntrySummary> CartEntries { get; set; }

        /// <summary>
        /// Gets or sets the total value of the cart at checkout.
        /// </summary>
        /// <remarks>
        /// The total can be calculated by the client, however the server
        /// will perform that operation for the client's convenience.
        /// </remarks>
        public decimal Total { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartContent"/> class.
        /// </summary>
        /// <param name="Info">The info header associated to the cart.</param>
        public CartContent(CartInfo info)
        {
            this.Info = info ?? throw new ArgumentNullException(nameof(info));
        }

        #region Types

        /// <summary>
        /// Represents a concise summary of a cart entry.
        /// </summary>
        public class CartEntrySummary
        {
            /// <summary>
            /// The unique identifier of the <see cref="Sku"/> being purchased.
            /// </summary>
            public string SkuId { get; set; }

            /// <summary>
            /// The actual price associated to this <see cref="Sku"/> comprehensive of
            /// whatever promotion possibly present.
            /// </summary>
            public decimal Price { get; set; }
        }

        #endregion
    }
}
