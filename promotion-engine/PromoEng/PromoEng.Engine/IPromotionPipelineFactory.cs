using System;

namespace PromoEng.Engine
{
    /// <summary>
    /// Describes a component capable of generating an instance
    /// implementing <see cref="IPromotionPipeline"/>.
    /// </summary>
    public interface IPromotionPipelineFactory
    {
        /// <summary>
        /// Creates a pipeline.
        /// </summary>
        /// <returns>A <see cref="IPromotionPipeline"/>.</returns>
        IPromotionPipeline Create();
    }
}
