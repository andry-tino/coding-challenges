using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// A collection of methods to perform different operations.
    /// </summary>
    public static class Algorithms
    {
        /// <summary>
        /// Sorts a sequence of integers.
        /// </summary>
        /// <param name="values">The values to sort.</param>
        /// <returns>The (sorted) sequence of values.</returns>
        public static async Task<IEnumerable<int>> SortIntegers(this IEnumerable<int> values)
        {
            if (values == null)
            {
                return null;
            }

            return await Task<IEnumerable<int>>.Run(() => 
            {
                var sortedValues = new List<int>(values);
                sortedValues.Sort();

                return sortedValues;
            });
        }
    }
}
