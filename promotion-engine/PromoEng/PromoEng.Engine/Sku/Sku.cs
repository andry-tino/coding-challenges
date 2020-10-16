using System;
using System.Diagnostics.CodeAnalysis;

namespace PromoEng.Engine
{
    /// <summary>
    /// Represents a Stock Keeping Unit.
    /// </summary>
    public class Sku : IComparable<Sku>
    {
        /// <summary>
        /// Gets the unique identifier of the SKU.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the friendly name of the SKU.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sku"/> class.
        /// </summary>
        /// <param name="id">The unique identifier of the SKU.</param>
        /// <param name="name">The friendly name of the SKU.</param>
        public Sku(string id, string name = null)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            if (string.IsNullOrEmpty(id) || string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("The identifier must be a valid string", nameof(id));
            }

            this.Name = name ?? id;
        }

        /// <inheritdoc/>
        public int CompareTo([AllowNull] Sku other)
        {
            return this.Id == other.Id ? 0 : -1;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"SKU '{this.Name}' ({this.Id})";
        }
    }
}
