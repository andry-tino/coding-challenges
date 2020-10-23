using System;
using System.Collections.Generic;

using PromoEng.Engine;

namespace PromoEng.Testability
{
    public class TestCartFactory : ICartFactory
    {
        private readonly IDictionary<Sku, decimal> priceList;

        public TestCartFactory(IDictionary<Sku, decimal> priceList)
        {
            this.priceList = priceList;
        }

        public ICart Create()
        {
            return new StandardCart(this.priceList);
        }
    }
}
