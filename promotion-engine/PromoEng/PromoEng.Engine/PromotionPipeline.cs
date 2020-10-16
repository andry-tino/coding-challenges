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
        /// Applies the pipeline to a <see cref="ICart"/>.
        /// </summary>
        /// <param name="cart">The cart to which the pipeline should be applied to.</param>
        public ICart Apply(ICart cart)
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
        /// <param name="rule">The rule to add.</param>
        /// <remarks>
        /// The order through which the rules added are important
        /// and can change the final <see cref="StandardCart"/> returned.
        /// </remarks>
        public void AddRule(IPromotionRule rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            this.rules.Add(rule);
        }

        protected virtual ICart RunRule(IPromotionRule rule, ICart cart)
        {
            return rule.Evaluate(cart);
        }
    }
}
