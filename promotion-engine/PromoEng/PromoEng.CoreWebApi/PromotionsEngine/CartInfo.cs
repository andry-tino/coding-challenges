using System;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents a cart info.
    /// A <see cref="PromoEng.Engine.ICart"/> is a mere structure to
    /// hold <see cref="PromoEng.Engine.Sku"/>.
    /// This structure is used to identify a specific cart and to identify
    /// its status in a registry collecting more carts,
    /// </summary>
    public class CartInfo
    {
        /// <summary>
        /// Gets the unique identifier of this cart.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the cart has been checked out or not.
        /// </summary>
        public bool CheckedOut { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartInfo"/> class.
        /// </summary>
        public CartInfo()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
