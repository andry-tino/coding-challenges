using System;
using System.Collections.Generic;

using PromoEng.Engine;

namespace PromoEng.CoreWebApi.Model
{
    /// <summary>
    /// Factory for creating instances of <see cref="ICart"/> in the web core app.
    /// </summary>
    public class CartFactory : ICartFactory
    {
        private readonly IDictionary<Sku, decimal> priceList;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartFactory"/>.
        /// </summary>
        /// <param name="priceList"></param>
        public CartFactory(IDictionary<Sku, decimal> priceList)
        {
            this.priceList = priceList ?? throw new ArgumentNullException(nameof(priceList));
        }

        /// <inheritdoc/>
        public ICart Create() => new StandardCart(this.priceList);
    }
}
