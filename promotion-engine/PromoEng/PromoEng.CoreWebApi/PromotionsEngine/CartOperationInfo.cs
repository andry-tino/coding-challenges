using System;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents an object providing information on a cart operation.
    /// </summary>
    public class CartOperationInfo<T> where T : class
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

        /// <summary>
        /// The content of the operation.
        /// </summary>
        public T Body { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartOperationInfo"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="exception">
        /// The <see cref="Exception"/> related to the failure.
        /// Important: if <paramref name="status"/> is <see cref="CartOperationStatus.Error"/> then this
        /// field must not be <code>null</code>, otherwise <see cref="ArgumentNullException"/> will be thrown.
        /// </param>
        public CartOperationInfo(CartOperationType type, CartOperationStatus status, Exception exception = null)
        {
            this.Type = type;
            this.Status = status;
            this.Exception = exception;

            if (exception == null && status == CartOperationStatus.Error)
            {
                throw new ArgumentNullException(nameof(exception),
                    "Necessary and sufficient condition for exception to be present is that the message is set as error");
            }
        }
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
