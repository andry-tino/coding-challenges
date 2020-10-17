using System;

namespace PromoEng.Engine
{
    /// <summary>
    /// Describes a pipeline used to process a collection of
    /// <see cref="IPromotionRule"/> against a <see cref="ICart"/>.
    /// </summary>
    public interface IPromotionPipeline
    {
        /// <summary>
        /// Applies the pipeline to a <see cref="ICart"/>.
        /// </summary>
        /// <param name="cart">The cart to which the pipeline should be applied to.</param>
        /// <returns>A new <see cref="ICart"/>. If no rules couls be applied, the same cart is returned.</returns>
        ICart Apply(ICart cart);
    }
}
