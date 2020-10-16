using System;
using System.Collections.Generic;

namespace PromoEng.Engine
{
    /// <summary>
    /// Represents the cascaded application of different <see cref="IPromotionRule"/>.
    /// </summary>
    /// <remarks>
    /// This pipeline is capable of executing all the rules in order in a fault-tolerant way.
    /// If a rule fails, that pipeline stage is discarded and the next rule is evaluated
    /// with a cart reverted to its previous state.
    /// </remarks>
    public class FaultTolerantPromotionPipeline : IPromotionPipeline
    {
        private IList<IPromotionRule> rules;

        /// <summary>
        /// Gets the list of exception that occurred last
        /// time <see cref="IPromotionPipeline.Apply(ICart)"/> was called.
        /// If the application executed with no errors, this will be an empty list.
        /// </summary>
        public IList<Tuple<IPromotionRule, Exception>> LastApplyExceptions { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultTolerantPromotionPipeline"/> class.
        /// </summary>
        public FaultTolerantPromotionPipeline()
        {
            this.rules = new List<IPromotionRule>();
            this.LastApplyExceptions = new List<Tuple<IPromotionRule, Exception>>();
        }

        /// <inheritdoc/>
        public ICart Apply(ICart cart)
        {
            if (cart == null)
            {
                return null;
            }

            // Reset the exception collection
            this.LastApplyExceptions = new List<Tuple<IPromotionRule, Exception>>();

            ICart result = cart;
            foreach (var rule in this.rules)
            {
                ICart stageResult = null;
                try
                {
                    stageResult = this.RunRule(rule, result);
                    result = stageResult;
                }
                catch (Exception e)
                {
                    // Log exception and do not update result to allow continuation to the next
                    // rule with the last cart from the latest succeeded rule
                    this.LastApplyExceptions.Add(new Tuple<IPromotionRule, Exception>(rule, e));
                }
            }

            return result;
        }

        /// <summary>
        /// Adds a rule to the pipeline structure.
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        /// <remarks>
        /// The order through which the rules are added is important
        /// and can change the final total price.
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
