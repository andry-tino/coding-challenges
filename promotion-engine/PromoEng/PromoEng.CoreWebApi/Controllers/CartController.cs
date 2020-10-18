using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        private readonly IInMemoryCollection<CartsCollection.CartsCollectionEntry> dataSource;
        private readonly ICartFactory cartFactory;
        private readonly IDictionary<Sku, decimal> priceList;
        private readonly IPromotionPipeline promotionPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="CartController"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="dataSource">The database to use.</param>
        /// <param name="cartFactory">The cart factory to use.</param>
        /// <param name="priceList">The price list holding the list of <see cref="Sku"/> and their prices.</param>
        /// <param name="promotionPipeline">The pipeline to use.</param>
        public CartController(ILogger<CartController> logger, 
            IInMemoryCollection<CartsCollection.CartsCollectionEntry> dataSource,
            ICartFactory cartFactory,
            IDictionary<Sku, decimal> priceList,
            IPromotionPipeline promotionPipeline)
        {
            this.logger = logger;
            this.dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
            this.cartFactory = cartFactory ?? throw new ArgumentNullException(nameof(cartFactory));
            this.priceList = priceList ?? throw new ArgumentNullException(nameof(priceList));
            this.promotionPipeline = promotionPipeline ?? throw new ArgumentNullException(nameof(promotionPipeline));
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
        public CartOperationInfo<IEnumerable<CartInfo>> GetAll()
        {
            return new CartOperationInfo<IEnumerable<CartInfo>>(CartOperationType.Read, CartOperationStatus.Successful)
            {
                Body = this.dataSource.Retrieve().Select(entry => entry.Info)
            };
        }

        /// <summary>
        /// Retrieves a cart's content.
        /// </summary>
        /// <param name="id">The id of the cart to use.</param>
        /// <returns>The content of the cart.</returns>
        [HttpGet("{id}")]
        public CartOperationInfo<CartContent> GetCart(string id)
        {
            var cartEntry = this.dataSource.Retrieve(id);
            if (cartEntry == null)
            {
                return new CartOperationInfo<CartContent>(CartOperationType.Read, CartOperationStatus.Error,
                    new Exception($"Could not retrieve cart with id '{id}'"));
            }

            return new CartOperationInfo<CartContent>(CartOperationType.Read, CartOperationStatus.Successful)
            {
                Body = new CartContent(cartEntry.Info)
                {
                    CartEntries = cartEntry.Cart.Select(entry => new CartContent.CartEntrySummary()
                    {
                        SkuId = entry.Sku.Id,
                        Price = entry.Price
                    }),
                    Total = cartEntry.Cart.Total
                }
            };
        }

        /// <summary>
        /// Creates a new cart.
        /// </summary>
        /// <returns>The info of the newly created cart.</returns>
        [HttpPost]
        public CartOperationInfo<CartInfo> Create()
        {
            var cartInfo = new CartInfo();
            var cartEntry = new CartsCollection.CartsCollectionEntry(cartInfo, this.cartFactory.Create());

            try
            {
                this.dataSource.Add(cartEntry);
            }
            catch (Exception e)
            {
                this.logger?.LogError($"Could not create new cart: {cartInfo.Id}");
                return new CartOperationInfo<CartInfo>(CartOperationType.Create, CartOperationStatus.Error, e);
            }

            this.logger?.LogInformation($"Created new cart: {cartInfo.Id}");

            return new CartOperationInfo<CartInfo>(CartOperationType.Create, CartOperationStatus.Successful)
            {
                Body = cartInfo
            };
        }

        /// <summary>
        /// Performs the checkout on a cart.
        /// </summary>
        /// <param name="id">The id of the cart to use.</param>
        /// <returns>The info of the cart that was checked out.</returns>
        [HttpPost("{id}")]
        public CartOperationInfo<CartInfo> Checkout(string id)
        {
            var cartEntry = this.dataSource.Retrieve(id);
            if (cartEntry == null)
            {
                this.logger?.LogError($"Cannot checkout, cart {id} could not be found");
                return new CartOperationInfo<CartInfo>(CartOperationType.Update, CartOperationStatus.Error,
                    new Exception($"Could not retrieve cart with id '{id}'"));
            }

            if (cartEntry.Info.CheckedOut)
            {
                this.logger?.LogError($"Cannot checkout, cart {id} is already checked out");
                return new CartOperationInfo<CartInfo>(CartOperationType.Update, CartOperationStatus.Error,
                    new Exception($"Cart with id '{id}' was already checked out"));
            }

            if (cartEntry.Cart.Quantity == 0)
            {
                this.logger?.LogError($"Cannot checkout, cart {id} is empty");
                return new CartOperationInfo<CartInfo>(CartOperationType.Update, CartOperationStatus.Error,
                    new Exception($"Cart with id '{id}' is empty"));
            }

            cartEntry.Info.CheckedOut = true;
            this.logger?.LogInformation($"Cart {cartEntry.Info.Id} was checked out successfully");

            return new CartOperationInfo<CartInfo>(CartOperationType.Update, CartOperationStatus.Successful)
            {
                Body = cartEntry.Info
            };
        }

        /// <summary>
        /// Adds one or more <see cref="Sku"/> to a cart.
        /// </summary>
        /// <param name="id">The id of the cart to use.</param>
        /// <param name="skuId">The id of the <see cref="Sku"/> to add.</param>
        /// <param name="quantity">The quantity to add of the specified SKU.</param>
        /// <returns>An object describing the operation outcome.</returns>
        [HttpPut("{id}")]
        public CartOperationInfo<CartInfo> Add(string id, string skuId, int? quantity)
        {
            if (skuId == null)
            {
                this.logger?.LogError($"Cannot add to cart {id}: no SKU id provided");
                return new CartOperationInfo<CartInfo>(CartOperationType.Update, CartOperationStatus.Error,
                    new Exception($"To add to a cart a valid SKU id is required"));
            }

            var sku = this.priceList.Keys.FirstOrDefault(sku => sku.Id == skuId);
            if (sku == null)
            {
                this.logger?.LogError($"Cannot add {(quantity.HasValue ? quantity.Value : 1)}x SKU {skuId} to cart {id}: SKU not found");
                return new CartOperationInfo<CartInfo>(CartOperationType.Update, CartOperationStatus.Error,
                    new Exception($"SKU with id '{skuId}' could not be found"));
            }

            var cartEntry = this.dataSource.Retrieve(id);
            if (cartEntry == null)
            {
                this.logger?.LogError($"Cannot add {(quantity.HasValue ? quantity.Value : 1)}x SKU {skuId} to cart {id}: cart not found");
                return new CartOperationInfo<CartInfo>(CartOperationType.Update, CartOperationStatus.Error,
                    new Exception($"Could not retrieve cart with id '{id}'"));
            }

            if (cartEntry.Info.CheckedOut)
            {
                this.logger?.LogError($"Cannot add {(quantity.HasValue ? quantity.Value : 1)}x SKU {skuId} to cart {id}: cart is checked out");
                return new CartOperationInfo<CartInfo>(CartOperationType.Update, CartOperationStatus.Error,
                    new Exception($"Cart with id '{id}' is checked out, operation forbidden"));
            }

            // Execute the add and modify the cart
            cartEntry.Cart.Add(sku, quantity.HasValue ? quantity.Value : 1);
            this.logger?.LogInformation($"Added {(quantity.HasValue ? quantity.Value : 1)}x SKU {skuId} to cart {id}");

            // Every time we modify the cart, we rerun the pipeline and update the cart
            this.ApplyPipeline(cartEntry);

            return new CartOperationInfo<CartInfo>(CartOperationType.Update, CartOperationStatus.Successful)
            {
                Body = cartEntry.Info
            };
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
        public CartOperationInfo<string> Delete(string id)
        {
            var cartEntry = this.dataSource.Remove(id);
            if (cartEntry == null)
            {
                this.logger?.LogError($"Cannot delete cart {id}: cart not found");
                return new CartOperationInfo<string>(CartOperationType.Delete, CartOperationStatus.Error,
                    new Exception($"Could not retrieve cart with id '{id}'"));
            }

            if (cartEntry.Info.CheckedOut)
            {
                this.logger?.LogError($"Cannot delete cart {id}: cart is checked out");
                return new CartOperationInfo<string>(CartOperationType.Delete, CartOperationStatus.Error,
                    new Exception($"Cart with id '{id}' is checked out, operation forbidden"));
            }

            this.logger?.LogInformation($"Delete cart {id}");

            return new CartOperationInfo<string>(CartOperationType.Delete, CartOperationStatus.Successful)
            {
                Body = "deleted"
            };
        }

        private void ApplyPipeline(CartsCollection.CartsCollectionEntry cartEntry)
        {
            Task.Run(() =>
            {
                decimal oldTotal = cartEntry.Cart.Total;
                cartEntry.Cart = this.promotionPipeline.Apply(cartEntry.Cart);
                decimal newTotal = cartEntry.Cart.Total;

                this.logger?.LogInformation($"Cart {cartEntry.Id} updated through pipeline: {oldTotal} => {newTotal}");

                var faultTolerantPipeline = this.promotionPipeline as FaultTolerantPromotionPipeline;
                if (faultTolerantPipeline != null && faultTolerantPipeline.LastApplyExceptions.Any())
                {
                    foreach (var errorReport in faultTolerantPipeline.LastApplyExceptions)
                    {
                        this.logger?.LogError($"Cart {cartEntry.Id} updated with errors: rule '{errorReport.Item1}' faulted with '{errorReport.Item2.Message}'");
                    }
                }
            });
        }
    }
}
