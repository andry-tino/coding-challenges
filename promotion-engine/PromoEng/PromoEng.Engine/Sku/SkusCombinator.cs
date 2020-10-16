using System;
using System.Linq;

namespace PromoEng.Engine
{
    /// <summary>
    /// Rules might be in need of replacing a group of entries in the
    /// <see cref="ICart"/> with one entry which represents a batch
    /// discount or similar. Use this class to generate the batching
    /// <see cref="Sku"/>.
    /// </summary>
    /// <remarks>
    /// This should not be used to batch more entries of the same <see cref="Sku"/>.
    /// Use it only when batching different <see cref="Sku"/>.
    /// </remarks>
    public class SkusCombinator
    {
        /// <summary>
        /// Combines more <see cref="Sku"/> into one.
        /// </summary>
        /// <param name="skus"></param>
        /// <returns></returns>
        public Sku Combine(params Sku[] skus)
        {
            if (skus == null)
            {
                throw new ArgumentNullException(nameof(skus));
            }
            if (skus.Length == 0)
            {
                throw new ArgumentException("Cannot combine an empty collection of SKUs", nameof(skus));
            }

            if (skus.Length == 1)
            {
                return skus[0];
            }

            return new Sku(
                skus.Select(sku => sku.Id).Aggregate((id1, id2) => $"{id1}&{id2}"),
                skus.Sum(sku => sku.UnitPrice),
                skus.Select(sku => sku.Name).Aggregate((name1, name2) => $"{name1} + {name2}"));
        }
    }
}
