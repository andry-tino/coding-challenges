﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PromoEng.Engine
{
    /// <summary>
    /// Represents a cart: a structure able to hold information about SKUs which are being purchased.
    /// </summary>
    public class StandardCart : ICart
    {
        private ICollection<SkuCartEntry> items;
        private decimal? total;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardCart"/> class.
        /// </summary>
        public StandardCart()
        {
            this.items = new List<SkuCartEntry>();
            this.total = null;
        }

        /// <inheritdoc/>
        public int Count => this.items.Sum(entry => entry.Quantity);

        /// <inheritdoc/>
        public void Add(Sku sku, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException(nameof(quantity), "Quantity must be 1 or more");
            }

            this.Add(new SkuCartEntry()
            {
                Sku = sku,
                Price = sku.UnitPrice * quantity,
                Quantity = quantity
            });
        }

        /// <inheritdoc/>
        public void Add(SkuCartEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            this.items.Add(entry);

            this.ResetTotal();
        }

        /// <inheritdoc/>
        public ICart Merge(ICart other)
        {
            return Merge(this, other);
        }

        /// <inheritdoc/>
        public IEnumerator<SkuCartEntry> GetEnumerator() => this.items.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.items.GetEnumerator();

        /// <summary>
        /// Converts the cart into a string representation providing all the info
        /// about the cart.
        /// </summary>
        /// <returns>A <see cref="string"/>.</returns>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <inheritdoc/>
        public object Clone()
        {
            var cart = new StandardCart();
            foreach (var item in this.items)
            {
                cart.Add(item.Clone() as SkuCartEntry);
            }

            return cart;
        }

        /// <inheritdoc/>
        public decimal Total
        {
            get
            {
                if (this.total == null)
                {
                    this.ComputeTotal();
                }

                return this.total.Value;
            }
        }

        private void ComputeTotal()
        {
            decimal total = 0;
            foreach (var item in this.items)
            {
                total += item.Price;
            }

            this.total = total;
        }

        private void ResetTotal() => this.total = null;

        private static ICart Merge(ICart cart1, ICart cart2)
        {
            ICart mergeCart = new StandardCart();
            Action<ICart, ICart> merger = (ICart src, ICart dst) =>
            {
                foreach (var entry in src)
                {
                    dst.Add(entry);
                }
            };

            merger(cart1, mergeCart);
            merger(cart2, mergeCart);

            return mergeCart;
        }
    }
}
