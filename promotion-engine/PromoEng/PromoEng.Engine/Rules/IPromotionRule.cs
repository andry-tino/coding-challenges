using System;

namespace PromoEng.Engine
{
    /// <summary>
    /// Describes a a promotion rule.
    /// </summary>
    public interface IPromotionRule
    {
        /// <summary>
        /// Evaluates a <see cref="ICart"/> and applies this promotion rule attempting to produce
        /// a new <see cref="ICart"/> with additional promotions on top of the existing ones.
        /// </summary>
        /// <param name="originalCart">The original <see cref="ICart"/> to start from.</param>
        /// <returns>A new <see cref="ICart"/> with the updated set of rules in place.</returns>
        ICart Evaluate(ICart originalCart);
    }
}
