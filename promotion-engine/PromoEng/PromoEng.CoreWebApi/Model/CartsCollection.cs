using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

using PromoEng.Engine;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents a structure capable of holding information about carts.
    /// </summary>
    public class CartsCollection : IInMemoryCollection<CartsCollection.CartsCollectionEntry>
    {
        protected IDictionary<string, CartsCollectionEntry> carts;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartsCollection"/> class.
        /// </summary>
        public CartsCollection()
        {
            this.carts = new ConcurrentDictionary<string, CartsCollectionEntry>();
        }

        /// <inheritdoc/>
        public void Add(CartsCollectionEntry item)
        {
            if (item == null)
            {
                return;
            }

            var added = this.carts.TryAdd(item.Id, item);

            if (!added)
            {
                throw new ArgumentException(nameof(item), "Cart already exists in collection, cannot add");
            }
        }

        /// <inheritdoc/>
        public CartsCollectionEntry Retrieve(string key)
        {
            if (this.carts.TryGetValue(key ?? string.Empty, out CartsCollectionEntry foundCart))
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
            if (this.carts.Remove(key ?? string.Empty, out CartsCollectionEntry removedCart))
            {
                return removedCart;
            }

            return null;
        }

        #region Types

        /// <summary>
        /// Represents an entry in the <see cref="CartsCollection"/>.
        /// </summary>
        public class CartsCollectionEntry : IUniqueResource
        {
            /// <summary>
            /// Gets the cart info header.
            /// The cart itself does not come with identification quantities by design,
            /// that responsability has to be taken by whatever other component using it.
            /// </summary>
            public CartInfo Info { get; private set; }

            /// <summary>
            /// Gets or sets the actual cart.
            /// </summary>
            public ICart Cart { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="CartsCollectionEntry"/> class.
            /// </summary>
            /// <param name="info">The info header.</param>
            /// <param name="cart">The cart object.</param>
            public CartsCollectionEntry(CartInfo info, ICart cart)
            {
                this.Info = info ?? throw new ArgumentNullException(nameof(info));
                this.Cart = cart ?? throw new ArgumentNullException(nameof(cart));
            }

            /// <inheritdoc/>
            public string Id => this.Info.Id;
        }

        #endregion
    }
}
