using System;

namespace PromoEng.Engine
{
    /// <summary>
    /// Describes a factory for <see cref="ICart"/>.
    /// </summary>
    public interface ICartFactory
    {
        /// <summary>
        /// Creates an <see cref="ICart"/>.
        /// </summary>
        /// <returns>An <see cref="ICart"/>.</returns>
        ICart Create();
    }
}
