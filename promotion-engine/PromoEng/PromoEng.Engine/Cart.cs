using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PromoEng.Engine
{
    /// <summary>
    /// Represents a cart: a structure able to hold information about SKUs which are being purchased.
    /// </summary>
    public class Cart : IEnumerable<Cart.SkuCartEntry>, ICloneable
    {
        private ICollection<SkuCartEntry> items;
        private decimal? total;

        /// <summary>
        /// Initializes a new instance of the <see cref="Cart"/> class.
        /// </summary>
        public Cart()
        {
            this.items = new List<SkuCartEntry>();
            this.total = null;
        }

        /// <summary>
        /// Gets the total number of SKUs in the cart.
        /// </summary>
        public int Count => this.items.Sum(entry => entry.Quantity);

        /// <summary>
        /// Adds one <see cref="Sku"/> to the cart.
        /// </summary>
        /// <param name="sku">The <see cref="Sku"/> to add.</param>
        /// <param name="quantity">The number of items to add.</param>
        public void Add(Sku sku, int quantity = 1)
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

        /// <summary>
        /// Adds one <see cref="SkuCartEntry"/> to the cart.
        /// </summary>
        /// <param name="sku">The <see cref="SkuCartEntry"/> to add.</param>
        public void Add(SkuCartEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            this.items.Add(entry);

            this.ResetTotal();
        }

        /// <summary>
        /// Merge this cart with another one.
        /// </summary>
        /// <param name="other">The other cart to merge</param>
        /// <returns>A new <see cref="Cart"/> obtained by mergin this and the one provided.</returns>
        /// <example>
        /// Use the merge functionality to apply different levels of flexibility
        /// in the handling of carts. There might be cases where certain rules (in a specific order) will
        /// be applied only to a certain set of items. While, for another set, a different set of rules
        /// should be considered. By doing so, it is effectively possible to apply different pipelines to
        /// different portionas of the cart by having isolated carts that could be merged at the end.
        /// </example>
        public Cart Merge(Cart other)
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
            var cart = new Cart();
            foreach (var item in this.items)
            {
                cart.Add(item.Clone() as SkuCartEntry);
            }

            return cart;
        }

        /// <summary>
        /// Gets the total price for checkout.
        /// </summary>
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

        private static Cart Merge(Cart cart1, Cart cart2)
        {
            var mergeCart = new Cart();
            Action<Cart, Cart> merger = (Cart src, Cart dst) =>
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

        #region Types

        /// <summary>
        /// Represents an additional information added to an <see cref="Sku"/> in the cart.
        /// </summary>
        public class SkuCartEntry : ICloneable
        {
            /// <summary>
            /// Gets or sets the SKU associated to this entry.
            /// </summary>
            public Sku Sku { get; set; }

            /// <summary>
            /// Gets or sets the quantity for the entry.
            /// </summary>
            public int Quantity { get; set; }

            /// <summary>
            /// Gets or sets the overall entry price.
            /// </summary>
            /// <remarks>
            /// This price will override <see cref="Sku.UnitPrice"/>.
            /// When the <see cref="Cart"/> has an entry, this field will
            /// show the actual price assigned to it. If this value is different
            /// from <see cref="Sku.UnitPrice"/>, it means that a rule was applied
            /// to change the price. This value is the one kept into account
            /// when computing <see cref="Cart.Total"/>.
            /// </remarks>
            public decimal Price { get; set; }

            /// <summary>
            /// Gets or sets the <see cref="IPromotionRule"/> identifier responsible for this entry.
            /// When this is <code>null</code>, then the entry has not been processed by a rule.
            /// </summary>
            public string PromotionRuleId { get; set; }

            /// <summary>
            /// Gets or sets a description associated to this entry.
            /// </summary>
            public string Description { get; set; }

            /// <inheritdoc/>
            public object Clone()
            {
                return new SkuCartEntry()
                {
                    Sku = this.Sku, // Keep the reference to the SKU
                    Quantity = this.Quantity,
                    Price = this.Price,
                    PromotionRuleId = this.PromotionRuleId,
                    Description = this.Description
                };
            }
        }

        #endregion
    }
}
