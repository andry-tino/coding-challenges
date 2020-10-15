using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PromoEng.Engine
{
    /// <summary>
    /// Represents a cart: a structure able to hold information about SKUs which are being purchased.
    /// </summary>
    public class Cart : ICollection<Sku>, ICloneable
    {
        private ICollection<KeyValuePair<Sku, SkuCartEntry>> items;

        public Cart()
        {
            this.items = new List<KeyValuePair<Sku, SkuCartEntry>>();
        }

        /// <summary>
        /// Gets the SKUs in the cart.
        /// </summary>
        public IEnumerable<Sku> Skus => this.items.Select(item => item.Key);

        /// <inheritdoc/>
        public int Count => this.items.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public void Add(Sku sku) => this.items.Add(new KeyValuePair<Sku, SkuCartEntry>(sku, new SkuCartEntry()
        {
            Price = sku.UnitPrice,
            Quantity = 1
        }));

        /// <inheritdoc/>
        public void Clear() => this.items.Clear();

        /// <inheritdoc/>
        public bool Contains(Sku sku) => this.items.Any(item => item.Key.CompareTo(sku) == 0);

        /// <inheritdoc/>
        public void CopyTo(Sku[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public IEnumerator<Sku> GetEnumerator() => this.Skus.GetEnumerator();

        /// <inheritdoc/>
        public bool Remove(Sku sku)
        {
            foreach (var item in this.items)
            {
                if ((object)item.Key == (object)sku)
                {
                    this.items.Remove(item);
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.Skus.GetEnumerator();

        /// <inheritdoc/>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <inheritdoc/>
        public object Clone()
        {
            throw new NotImplementedException();
        }

        #region Types

        /// <summary>
        /// Represents an additional information added to an <see cref="Sku"/> in the cart.
        /// </summary>
        internal class SkuCartEntry
        {
            public int Quantity { get; set; }

            public float Price { get; set; }

            public string PromotionRuleId { get; set; }
        }

        #endregion
    }
}
