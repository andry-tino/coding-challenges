using System;
using System.Collections.Generic;
using System.Text;

namespace PromoEng.Engine.Rules
{
    /// <summary>
    /// Represents a promotion rule to group two different SKUs (one item each) and have a set price for them.
    /// </summary>
    public class PairOfDifferentSkusForRule : IPromotionRule
    {
        /// <summary>
        /// 
        /// </summary>
        public string Sku1Id { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Sku2Id { get; }

        /// <summary>
        /// 
        /// </summary>
        public float TotalPrice { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sku1Id"></param>
        /// <param name="sku2Id"></param>
        /// <param name="totalPrice"></param>
        public PairOfDifferentSkusForRule(string sku1Id, string sku2Id, float totalPrice)
        {
            this.Sku1Id = sku1Id ?? throw new ArgumentNullException(nameof(sku1Id));
            this.Sku2Id = sku2Id ?? throw new ArgumentNullException(nameof(sku2Id));
            this.TotalPrice = Math.Abs(totalPrice);
        }

        public Cart Evaluate(Cart originalCart)
        {
            throw new NotImplementedException();
        }
    }
}
