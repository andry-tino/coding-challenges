using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

using PromoEng.Engine;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents a structure capable of holding information about carts.
    /// </summary>
    public class CartsCollection : IInMemoryCollection<CartsCollection.CartsCollectionEntry>
    {
        private IDictionary<string, CartsCollectionEntry> carts;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartsCollection"/> class.
        /// </summary>
        public CartsCollection()
        {
            this.carts = new ConcurrentDictionary<string, CartsCollectionEntry>();
        }

        /// <inheritdoc/>
        public void Add(string key, CartsCollectionEntry item)
        {
            var added = this.carts.TryAdd(key, item);

            if (!added)
            {
                throw new ArgumentException(nameof(item), "Cart already exists in collection, cannot add");
            }
        }

        /// <inheritdoc/>
        public CartsCollectionEntry Retrieve(string key)
        {
            if (this.carts.TryGetValue(key, out CartsCollectionEntry foundCart))
            {
                return foundCart;
            }

            return null;
        }

        /// <inheritdoc/>
        public IEnumerable<CartsCollectionEntry> Retrieve() => this.carts.Values;

        /// <inheritdoc/>
        public CartsCollectionEntry Remove(string key)
        {
            if (this.carts.Remove(key, out CartsCollectionEntry removedCart))
            {
                return removedCart;
            }

            return null;
        }

        #region Types

        public class CartsCollectionEntry
        {
            /// <summary>
            /// Gets or sets the cart info header.
            /// </summary>
            public CartInfo Info { get; set; }

            /// <summary>
            /// Gets or sets the actual cart.
            /// </summary>
            public ICart Cart { get; set; }
        }

        #endregion
    }
}
