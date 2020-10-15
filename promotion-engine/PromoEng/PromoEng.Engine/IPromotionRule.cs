using System;

namespace PromoEng.Engine
{
    /// <summary>
    /// Describes a a promotion rule.
    /// </summary>
    public interface IPromotionRule
    {
        /// <summary>
        /// Evaluates a <see cref="Cart"/> and applies this promotion rule attempting to produce
        /// a new <see cref="Cart"/> with additional promotions on top of the existing ones.
        /// </summary>
        /// <param name="originalCart">The original <see cref="Cart"/> to start from.</param>
        /// <returns>A new <see cref="Cart"/> with the updated set of rules in place.</returns>
        Cart Evaluate(Cart originalCart);
    }
}
