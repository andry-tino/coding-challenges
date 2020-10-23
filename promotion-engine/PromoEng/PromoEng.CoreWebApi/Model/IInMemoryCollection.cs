using System;
using System.Collections.Generic;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Describes an entity which can be identified by a <see cref="string"/>.
    /// </summary>
    public interface IUniqueResource
    {
        /// <summary>
        /// Gets the unique identifier
        /// </summary>
        string Id { get; }
    }

    /// <summary>
    /// Describes an object capable of storing unique resources in memory.
    /// </summary>
    public interface IInMemoryCollection<T> where T : IUniqueResource
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
        /// Retrieves all the objects in the collection.
        /// </summary>
        /// <returns>The collection of objects, if no objects an empty collection.</returns>
        IEnumerable<T> Retrieve();

        /// <summary>
        /// Removes an object from the collection.
        /// </summary>
        /// <param name="key">The key to use.</param>
        T Remove(string key);
    }
}
