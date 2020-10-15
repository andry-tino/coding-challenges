using System;
using System.Collections.Generic;

namespace PromoEng.Engine
{
    /// <summary>
    /// Represents the cascaded application of different <see cref="IPromotionRule"/>.
    /// </summary>
    public class PromotionPipeline
    {
        private IList<IPromotionRule> rules;

        public PromotionPipeline()
        {
            this.rules = new List<IPromotionRule>();
        }

        /// <summary>
        /// Applies the pipeline to a <see cref="Cart"/>.
        /// </summary>
        /// <param name="cart">The cart to which the pipeline should be applied to.</param>
        public Cart Apply(Cart cart)
        {
            if (cart == null)
            {
                return null;
            }

            var result = cart;
            foreach (var rule in this.rules)
            {
                result = this.RunRule(rule, result);
            }

            return result;
        }

        /// <summary>
        /// Adds a rule to the pipeline structure.
        /// </summary>
        /// <param name="rule"></param>
        public void AddRule(IPromotionRule rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            this.rules.Add(rule);
        }

        protected virtual Cart RunRule(IPromotionRule rule, Cart cart)
        {
            return rule.Evaluate(cart);
        }
    }
}
