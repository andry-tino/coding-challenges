using System;
using System.Collections.Generic;

namespace PromoEng.Engine
{
    /// <summary>
    /// Describes a cart.
    /// </summary>
    public interface ICart : IEnumerable<SkuCartEntry>, ICloneable
    {
        /// <summary>
        /// Gets the total number of SKUs in the cart.
        /// </summary>
        /// <remarks>
        /// As a design choice, the interface does not capture any behavior related to pricing of <see cref="Sku"/>.
        /// Each single specific implementation will deal with this in its own way.
        /// </remarks>
        int Count { get; }

        /// <summary>
        /// Adds one <see cref="Sku"/> to the cart.
        /// </summary>
        /// <param name="sku">The <see cref="Sku"/> to add.</param>
        /// <param name="quantity">The number of items to add.</param>
        void Add(Sku sku, int quantity = 1);

        /// <summary>
        /// Adds one <see cref="SkuCartEntry"/> to the cart.
        /// </summary>
        /// <param name="sku">The <see cref="SkuCartEntry"/> to add.</param>
        void Add(SkuCartEntry entry);

        /// <summary>
        /// Merge this cart with another one.
        /// </summary>
        /// <param name="other">The other cart to merge</param>
        /// <returns>A new <see cref="ICart"/> obtained by mergin this and the one provided.</returns>
        /// <example>
        /// Use the merge functionality to apply different levels of flexibility
        /// in the handling of carts. There might be cases where certain rules (in a specific order) will
        /// be applied only to a certain set of items. While, for another set, a different set of rules
        /// should be considered. By doing so, it is effectively possible to apply different pipelines to
        /// different portionas of the cart by having isolated carts that could be merged at the end.
        /// </example>
        ICart Merge(ICart other);

        /// <summary>
        /// Gets the total price for checkout.
        /// </summary>
        decimal Total { get; }
    }
}
