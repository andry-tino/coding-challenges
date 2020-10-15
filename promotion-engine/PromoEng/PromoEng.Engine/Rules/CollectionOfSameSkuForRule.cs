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
        /// Gets the identifier of the <see cref="Sku"/>.
        /// </summary>
        public string SkuId { get; }

        /// <summary>
        /// Gets the quantity of the same amount of items of the <see cref="Sku"/>.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Gets the price at which the specified amount of items of the
        /// specified <see cref="Sku"/> will be sold for.
        /// </summary>
        public float TotalPrice { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionOfSameSkuForRule"/> class.
        /// </summary>
        /// <param name="skuId">The identifier of the <see cref="Sku"/>.</param>
        /// <param name="quantity">The quantity of the same amount of items of the <see cref="Sku"/>.</param>
        /// <param name="totalPrice">The overall price to assign to the batch.</param>
        public CollectionOfSameSkuForRule(string skuId, int quantity, float totalPrice)
        {
            this.SkuId = skuId ?? throw new ArgumentNullException(nameof(skuId));
            this.Quantity = Math.Abs(quantity);
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
