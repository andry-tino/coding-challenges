using System;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Describes an object capable of storing data in memory.
    /// </summary>
    public interface IInMemoryCollection<T>
    {
        /// <summary>
        /// Adds an object to the collection.
        /// </summary>
        /// <param name="item">The item to add.</param>
        void Add(T item);

        /// <summary>
        /// Retrieves an object from the collection.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <returns>The found object, or <code>null</code> otherwise.</returns>
        T Retrieve(string key);

        /// <summary>
        /// Removes an object from the collection.
        /// </summary>
        /// <param name="key">The key to use.</param>
        T Remove(string key);
    }
}
