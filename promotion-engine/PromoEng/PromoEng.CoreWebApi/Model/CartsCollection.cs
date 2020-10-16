using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents a structure capable of holding information about carts.
    /// </summary>
    internal class CartsCollection : IInMemoryCollection<CartInfo>
    {
        private IDictionary<string, CartInfo> carts;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartsCollection"/> class.
        /// </summary>
        public CartsCollection()
        {
            this.carts = new ConcurrentDictionary<string, CartInfo>();
        }

        /// <inheritdoc/>
        public void Add(CartInfo item)
        {
            var added = this.carts.TryAdd(item.Id, item);

            if (!added)
            {
                throw new ArgumentException(nameof(item), "Cart already exists in collection, cannot add");
            }
        }

        /// <inheritdoc/>
        public CartInfo Retrieve(string key)
        {
            if (this.carts.TryGetValue(key, out CartInfo foundCart))
            {
                return foundCart;
            }

            return null;
        }

        /// <inheritdoc/>
        public CartInfo Remove(string key)
        {
            if (this.carts.Remove(key, out CartInfo removedCart))
            {
                return removedCart;
            }

            return null;
        }
    }
}
