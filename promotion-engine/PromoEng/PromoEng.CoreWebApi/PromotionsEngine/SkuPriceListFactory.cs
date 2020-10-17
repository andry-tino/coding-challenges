using System;
using System.Collections.Generic;

using PromoEng.Engine;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Factory to create price lists.
    /// </summary>
    public class SkuPriceListFactory : ISkuPriceListFactory
    {
        private readonly IEnumerable<SkuOption> skuOptions;

        public SkuPriceListFactory(IEnumerable<SkuOption> skuOptions)
        {
            this.skuOptions = skuOptions ?? throw new ArgumentNullException(nameof(skuOptions));
        }

        /// <inheritdoc/>
        public IDictionary<Sku, decimal> Create()
        {
            var pricelist = new Dictionary<Sku, decimal>();
            foreach (var skuOption in this.skuOptions)
            {
                pricelist.TryAdd(new Sku(skuOption.Id, skuOption.Name), skuOption.UnitPrice);
            }

            return pricelist;
        }
    }
}
