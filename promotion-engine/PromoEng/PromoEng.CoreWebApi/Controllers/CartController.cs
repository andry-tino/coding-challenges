using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using PromoEng.Engine;

namespace PromoEng.CoreWebApi.Controllers
{
    /// <summary>
    /// Controller responsible for handling requests to cart handling.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ILogger<CartController> logger;
        private readonly IInMemoryCollection<CartInfo> dataSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartController"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="dataSource">The database to use.</param>
        public CartController(ILogger<CartController> logger, IInMemoryCollection<CartInfo> dataSource)
        {
            this.logger = logger;
            this.dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        }

        /// <summary>
        /// Retrieves all carts.
        /// </summary>
        /// <returns>A collection of all carts' info.</returns>
        /// <remarks>
        /// We do not return the content to avoid big payloads, there is no actual
        /// value in sending such dense info here. If the content of some carts is
        /// to be inspected, single calls are to be made targeting each one of them.
        /// </remarks>
        [HttpGet()]
        public IEnumerable<CartInfo> GetAll()
        {
            return new CartInfo[0];
        }

        /// <summary>
        /// Retrieves a cart's content.
        /// </summary>
        /// <param name="id">The id of the cart to use.</param>
        /// <returns>The content of the cart.</returns>
        [HttpGet("{id}")]
        public CartContent GetCart(string id)
        {
            return new CartContent();
        }

        /// <summary>
        /// Creates a new cart.
        /// </summary>
        /// <returns>The info of the newly created cart.</returns>
        [HttpPost]
        public CartInfo Create()
        {
            return new CartInfo();
        }

        /// <summary>
        /// Performs the checkout on a cart.
        /// </summary>
        /// <param name="id">The id of the cart to use.</param>
        /// <returns>The info of the cart that was checked out.</returns>
        [HttpPost("{id}")]
        public CartInfo Checkout(string id)
        {
            return new CartInfo();
        }

        /// <summary>
        /// Adds one or more <see cref="Sku"/> to a cart.
        /// </summary>
        /// <param name="id">The id of the cart to use.</param>
        /// <returns>An object describing the operation outcome.</returns>
        [HttpPut("{id}")]
        public CartOperationInfo Add(string id)
        {
            return new CartOperationInfo();
        }

        /// <summary>
        /// Attempts to delete a cart.
        /// </summary>
        /// <param name="id">The id of the cart.</param>
        /// <returns>The cart just deleted.</returns>
        /// <remarks>
        /// A checked-out cart cannot be deleted, it must stay in the books.
        /// </remarks>
        [HttpDelete("{id}")]
        public CartInfo Delete(string id)
        {
            return new CartInfo();
        }
    }
}
