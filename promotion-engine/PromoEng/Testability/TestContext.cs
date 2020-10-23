using System;
using System.Collections.Generic;

using PromoEng.Engine;

namespace PromoEng.Testability
{
    public class TestContext
    {
        public IDictionary<Sku, decimal> PriceList { get; }

        public ICartFactory CartFactory { get; }

        public TestContext()
        {
            this.PriceList = new Dictionary<Sku, decimal>();
            this.CartFactory = new TestCartFactory(this.PriceList);
        }

        public Sku CreateNewSku(string id, decimal price)
        {
            var sku = new Sku(id);
            this.PriceList.Add(new KeyValuePair<Sku, decimal>(sku, price));

            return sku;
        }
    }
}
