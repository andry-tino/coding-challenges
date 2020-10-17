using System;

namespace PromoEng.Engine
{
    /// <summary>
    /// Groups utility functions targeting <see cref="ICart"/>.
    /// </summary>
    internal static class CartExtensions
    {
        /// <summary>
        /// Prints the cart for invoicing purposes.
        /// </summary>
        /// <param name="cart">The <see cref="ICart"/> to print.</param>
        /// <returns>A <see cref="string"/>.</returns>
        public static string PrintCart(this ICart cart)
        {
            string n = Environment.NewLine;
            string nn = $"{n}{n}";
            string output = string.Empty;

            output += $"Cart info ({cart.Quantity} SKU){n}";
            output += $"==={nn}";

            foreach (var item in cart)
            {
                output += $"- {item.ToString()} => {item.Price}{n}";
            }

            output += $"---{n}";
            output += $"Total: {cart.Total}{n}";

            return output;
        }
    }
}
