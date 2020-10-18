using System;

namespace PromoEng.Engine.Rules
{
    /// <summary>
    /// Represents a promotion rule to group two different SKUs (one item each) and have a set price for them.
    /// </summary>
    public class PairOfDifferentSkusForRule : IPromotionRule
    {
        private readonly ICartFactory cartFactory;

        /// <summary>
        /// Gets the unique identifier of this rule.
        /// </summary>
        public static string RuleId = typeof(PairOfDifferentSkusForRule).Name;

        /// <summary>
        /// Gets the first <see cref="Sku"/>.
        /// </summary>
        public Sku Sku1 { get; }

        /// <summary>
        /// Gets the second <see cref="Sku"/>.
        /// </summary>
        public Sku Sku2 { get; }

        /// <summary>
        /// Gets the total price to assign to the couple.
        /// </summary>
        public decimal TotalPrice { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PairOfDifferentSkusForRule"/> class.
        /// </summary>
        /// <param name="cartFactory">The <see cref="ICartFactory"/> to use to generate a cart.</param>
        /// <param name="sku1">The first <see cref="Sku"/>.</param>
        /// <param name="sku2">The second <see cref="Sku"/>.</param>
        /// <param name="totalPrice">The total price to assign to the couple.</param>
        public PairOfDifferentSkusForRule(ICartFactory cartFactory, Sku sku1, Sku sku2, decimal totalPrice)
        {
            this.cartFactory = cartFactory ?? throw new ArgumentNullException(nameof(cartFactory));
            this.Sku1 = sku1 ?? throw new ArgumentNullException(nameof(sku1));
            this.Sku2 = sku2 ?? throw new ArgumentNullException(nameof(sku2));
            this.TotalPrice = Math.Abs(totalPrice);

            if (this.Sku1.CompareTo(this.Sku2) == 0)
            {
                throw new ArgumentException("Cannot use the same SKU in the rule", nameof(sku2));
            }
        }

        /// <inheritdoc/>
        public ICart Evaluate(ICart originalCart)
        {
            if (originalCart == null)
            {
                return null;
            }

            var cart = this.cartFactory.Create();

            Func<SkuCartEntry, bool> candidateConditionSku1 =
                (entry) => entry.Sku.CompareTo(this.Sku1) == 0 && entry.PromotionRuleId == null;
            Func<SkuCartEntry, bool> candidateConditionSku2 =
                (entry) => entry.Sku.CompareTo(this.Sku2) == 0 && entry.PromotionRuleId == null;

            // Get all candidate entries that can be batched
            // And copy all the non-candidates to new cart
            int candidatesSku1Count = 0;
            int candidatesSku2Count = 0;
            for (int i = 0, l = originalCart.Count; i < l; i++)
            {
                if (candidateConditionSku1(originalCart[i]))
                {
                    // Candidate: keep note of the quantity
                    candidatesSku1Count += originalCart[i].Quantity;
                    continue;
                }
                if (candidateConditionSku2(originalCart[i]))
                {
                    // Candidate: keep note of the quantity
                    candidatesSku2Count += originalCart[i].Quantity;
                    continue;
                }

                // Non candidate: transfer to new cart
                cart.Add(originalCart[i].Clone() as SkuCartEntry);
            }

            // Batch candidates in new cart
            int batchesCount = Math.Min(candidatesSku1Count, candidatesSku2Count);
            int residualsCount = Math.Max(candidatesSku1Count, candidatesSku2Count) - batchesCount;
            Sku residualSku = candidatesSku1Count - candidatesSku2Count > 0 ? this.Sku1 : this.Sku2;
            // Batch what we can
            var cominedSku = new SkusCombinator().Combine(this.Sku1, this.Sku2);
            for (int i = 0; i < batchesCount; i++)
            {
                // Do not add a single batch containing all batchable entries because we want to keep
                // trackable the batching in the final cart description when printing
                cart.Add(new SkuCartEntry()
                {
                    Sku = cominedSku,
                    Price = this.TotalPrice,
                    Quantity = 1,
                    PromotionRuleId = RuleId,
                    Description = $"Batch 1 x 1 of {this.Sku1.Name} and 1 of {this.Sku2.Name} SKUs for special price: {this.TotalPrice}"
                });
            }
            // Add the remaining as non-batched
            for (int i = 0; i < residualsCount; i++)
            {
                cart.Add(residualSku);
            }

            return cart;
        }
    }
}
