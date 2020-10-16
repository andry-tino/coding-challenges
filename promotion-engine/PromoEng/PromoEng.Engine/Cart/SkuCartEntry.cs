using System;

namespace PromoEng.Engine
{
    /// <summary>
    /// Represents an additional information added to an <see cref="Sku"/> in the cart.
    /// </summary>
    public class SkuCartEntry : ICloneable
    {
        /// <summary>
        /// Gets or sets the SKU associated to this entry.
        /// </summary>
        public Sku Sku { get; set; }

        /// <summary>
        /// Gets or sets the quantity for the entry.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the overall entry price.
        /// </summary>
        /// <remarks>
        /// This price will override <see cref="Sku.UnitPrice"/>.
        /// When the <see cref="StandardCart"/> has an entry, this field will
        /// show the actual price assigned to it. If this value is different
        /// from <see cref="Sku.UnitPrice"/>, it means that a rule was applied
        /// to change the price. This value is the one kept into account
        /// when computing <see cref="StandardCart.Total"/>.
        /// </remarks>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IPromotionRule"/> identifier responsible for this entry.
        /// When this is <code>null</code>, then the entry has not been processed by a rule.
        /// </summary>
        public string PromotionRuleId { get; set; }

        /// <summary>
        /// Gets or sets a description associated to this entry.
        /// </summary>
        public string Description { get; set; }

        /// <inheritdoc/>
        public object Clone()
        {
            return new SkuCartEntry()
            {
                Sku = this.Sku, // Keep the reference to the SKU
                Quantity = this.Quantity,
                Price = this.Price,
                PromotionRuleId = this.PromotionRuleId,
                Description = this.Description
            };
        }
    }
}
