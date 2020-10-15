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
        /// Gets the identifier of the first <see cref="Sku"/>.
        /// </summary>
        public string Sku1Id { get; }

        /// <summary>
        /// Gets the identifier of the second <see cref="Sku"/>.
        /// </summary>
        public string Sku2Id { get; }

        /// <summary>
        /// Gets the total price to assign to the couple.
        /// </summary>
        public float TotalPrice { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PairOfDifferentSkusForRule"/> class.
        /// </summary>
        /// <param name="sku1Id">The identifier of the first <see cref="Sku"/>.</param>
        /// <param name="sku2Id">The identifier of the second <see cref="Sku"/>.</param>
        /// <param name="totalPrice">The total price to assign to the couple.</param>
        public PairOfDifferentSkusForRule(string sku1Id, string sku2Id, float totalPrice)
        {
            this.Sku1Id = sku1Id ?? throw new ArgumentNullException(nameof(sku1Id));
            this.Sku2Id = sku2Id ?? throw new ArgumentNullException(nameof(sku2Id));
            this.TotalPrice = Math.Abs(totalPrice);
        }

        /// <inheritdoc/>
        public Cart Evaluate(Cart originalCart)
        {
            if (originalCart == null)
            {
                return null;
            }

            var cart = new Cart();
            foreach (var item in originalCart)
            {

            }

            return cart;
        }
    }
}
