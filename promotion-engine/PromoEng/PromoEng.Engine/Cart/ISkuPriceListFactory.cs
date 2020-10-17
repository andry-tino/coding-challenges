using System;
using System.Collections.Generic;

namespace PromoEng.Engine
{
    /// <summary>
    /// Describes a factory for <see cref="IDictionary{Sku, decimal}"/>.
    /// </summary>
    public interface ISkuPriceListFactory
    {
        /// <summary>
        /// Generate a price list.
        /// </summary>
        /// <returns>A price list.</returns>
        IDictionary<Sku, decimal> Create();
    }
}
