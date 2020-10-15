using System;
using System.Linq;

namespace PromoEng.Engine.Rules
{
    /// <summary>
    /// Represents a promotion rule to group a certain number of items of the same SKU and have a set price for them.
    /// </summary>
    public class CollectionOfSameSkuForRule : IPromotionRule
    {
        /// <summary>
        /// Gets the unique identifier of this rule.
        /// </summary>
        public static string RuleId = typeof(CollectionOfSameSkuForRule).Name;

        /// <summary>
        /// Gets the <see cref="PromoEng.Engine.Sku"/>.
        /// </summary>
        public Sku Sku { get; }

        /// <summary>
        /// Gets the quantity of the same amount of items of the <see cref="PromoEng.Engine.Sku"/>.
        /// </summary>
        public int Quantity { get; }

        /// <summary>
        /// Gets the price at which the specified amount of items of the
        /// specified <see cref="PromoEng.Engine.Sku"/> will be sold for.
        /// </summary>
        public decimal TotalPrice { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionOfSameSkuForRule"/> class.
        /// </summary>
        /// <param name="sku">The <see cref="PromoEng.Engine.Sku"/>.</param>
        /// <param name="quantity">The quantity of the same amount of items of the <see cref="PromoEng.Engine.Sku"/>.</param>
        /// <param name="totalPrice">The overall price to assign to the batch.</param>
        public CollectionOfSameSkuForRule(Sku sku, int quantity, decimal totalPrice)
        {
            this.Sku = sku ?? throw new ArgumentNullException(nameof(sku));
            this.Quantity = quantity != 0
                ? Math.Abs(quantity)
                : throw new ArgumentException(nameof(quantity), "Quantity cannot be zero");
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
            Func<Cart.SkuCartEntry, bool> candidateCondition =
                (entry) => entry.Sku.CompareTo(this.Sku) == 0 && entry.PromotionRuleId == null;
            
            // Get all candidate entries that can be batched
            int candidatesCount = originalCart
                .Where(entry => candidateCondition(entry))
                .Sum(entry => entry.Quantity);

            // Copy all the non-candidates to new cart
            foreach (var entry in originalCart)
            {
                if (!candidateCondition(entry))
                {
                    cart.Add(entry.Clone() as Cart.SkuCartEntry);
                }
            }

            // Batch candidates in new cart
            int batchesCount = candidatesCount / this.Quantity; // Integer division
            int residualsCount = candidatesCount % this.Quantity;
            // Batch what we can
            for (int i = 0; i < batchesCount; i++)
            {
                // Do not add a single batch containing all batchable entries because we want to keep
                // trackable the batching in the final cart description when printing
                cart.Add(new Cart.SkuCartEntry()
                {
                    Sku = this.Sku,
                    Price = this.TotalPrice,
                    Quantity = 1,
                    PromotionRuleId = RuleId,
                    Description = $"Batch 1 x {this.Quantity} of {this.Sku.Name} for special price: {this.TotalPrice}"
                });
            }
            // Add the remaining as non-batched
            for (int i = 0; i < residualsCount; i++)
            {
                cart.Add(this.Sku);
            }

            return cart;
        }
    }
}
