using System;
using System.Collections.Generic;
using System.Text;

namespace PromoEng.Engine.Rules
{
    /// <summary>
    /// Represents a promotion rule to group a certain number of items of the same SKU and have a set price for them.
    /// </summary>
    public class CollectionOfSameSkuForRule : IPromotionRule
    {
        /// <summary>
        /// 
        /// </summary>
        public string SkuId { get; }

        /// <summary>
        /// 
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// 
        /// </summary>
        public float TotalPrice { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skuId"></param>
        /// <param name="quantity"></param>
        /// <param name="totalPrice"></param>
        public CollectionOfSameSkuForRule(string skuId, int quantity, float totalPrice)
        {
            this.SkuId = skuId ?? throw new ArgumentNullException(nameof(skuId));
            this.Quantity = Math.Abs(quantity);
            this.TotalPrice = Math.Abs(totalPrice);
        }

        public Cart Evaluate(Cart originalCart)
        {
            throw new NotImplementedException();
        }
    }
}
