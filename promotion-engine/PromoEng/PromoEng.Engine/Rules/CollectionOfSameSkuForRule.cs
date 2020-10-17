using System;

namespace PromoEng.Engine.Rules
{
    /// <summary>
    /// Represents a promotion rule to group a certain number of items of the same SKU and have a set price for them.
    /// </summary>
    public class CollectionOfSameSkuForRule : IPromotionRule
    {
        private readonly ICartFactory cartFactory;

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
        /// <param name="cartFactory">The <see cref="ICartFactory"/> to use to generate a cart.</param>
        /// <param name="sku">The <see cref="PromoEng.Engine.Sku"/>.</param>
        /// <param name="quantity">The quantity of the same amount of items of the <see cref="PromoEng.Engine.Sku"/>.</param>
        /// <param name="totalPrice">The overall price to assign to the batch.</param>
        public CollectionOfSameSkuForRule(ICartFactory cartFactory, Sku sku, int quantity, decimal totalPrice)
        {
            this.cartFactory = cartFactory ?? throw new ArgumentNullException(nameof(cartFactory));
            this.Sku = sku ?? throw new ArgumentNullException(nameof(sku));
            this.Quantity = quantity != 0
                ? Math.Abs(quantity)
                : throw new ArgumentException(nameof(quantity), "Quantity cannot be zero");
            this.TotalPrice = Math.Abs(totalPrice);
        }

        /// <inheritdoc/>
        public ICart Evaluate(ICart originalCart)
        {
            if (originalCart == null)
            {
                return null;
            }

            var cart = this.cartFactory.Create();

            Func<SkuCartEntry, bool> candidateCondition =
                (entry) => entry.Sku.CompareTo(this.Sku) == 0 && entry.PromotionRuleId == null;

            // Get all candidate entries that can be batched
            // And copy all the non-candidates to new cart
            int candidatesCount = 0;
            for (int i = 0, l = originalCart.Count; i < l; i++)
            {
                if (candidateCondition(originalCart[i]))
                {
                    // Candidate: keep note of the quantity
                    candidatesCount += originalCart[i].Quantity;
                    continue;
                }

                // Non candidate: transfer to new cart
                cart.Add(originalCart[i].Clone() as SkuCartEntry);
            }

            // Batch candidates in new cart
            int batchesCount = candidatesCount / this.Quantity; // Integer division
            int residualsCount = candidatesCount % this.Quantity;
            // Batch what we can
            for (int i = 0; i < batchesCount; i++)
            {
                // Do not add a single batch containing all batchable entries because we want to keep
                // trackable the batching in the final cart description when printing
                cart.Add(new SkuCartEntry()
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
