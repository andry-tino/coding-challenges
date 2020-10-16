using System;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents an object providing information on a cart operation.
    /// </summary>
    public class CartOperationInfo // TODO: remove this and use a middleware to add headers for this info
    {
        /// <summary>
        /// Gets or sets the type of operation.
        /// </summary>
        public CartOperationType Type { get; set; }

        /// <summary>
        /// Gets or sets the status of the operation.
        /// </summary>
        public CartOperationStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the error which occurred.
        /// </summary>
        /// <remarks>
        /// When <see cref="Status"/> is not successful, then this
        /// field should have a non-null value.
        /// </remarks>
        public Exception Exception { get; set; }
    }

    /// <summary>
    /// Describes a cart operation type.
    /// </summary>
    public enum CartOperationType
    {
        Create = 0,
        Read = 1,
        Update = 2,
        Delete = 3
    }

    /// <summary>
    /// Describes the outcome of a cart operation.
    /// </summary>
    public enum CartOperationStatus
    {
        Successful = 0,
        Error = 1
    }
}
