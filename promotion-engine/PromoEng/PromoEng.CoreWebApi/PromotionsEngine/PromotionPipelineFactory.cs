using System;
using System.Collections.Generic;
using System.Linq;

using PromoEng.Engine;
using PromoEng.Engine.Rules;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents a factory responsible for creating <see cref="IPromotionPipeline"/>.
    /// </summary>
    public class PromotionPipelineFactory : IPromotionPipelineFactory
    {
        public const string CollectionOfSameSkuForRuleId = "CollectionOfSameSkuFor";
        public const string PairOfDifferentSkusForRuleId = "PairOfDifferentSkusForRuleId";

        private readonly IList<PromotionRuleOption> promotionRuleOptions;
        private readonly ICartFactory cartFactory;
        private readonly IEnumerable<Sku> skus;

        /// <summary>
        /// Initializes a new instance of the <see cref="PromotionPipelineFactory"/> class.
        /// </summary>
        /// <param name="cartFactory">The cart factory to use.</param>
        public PromotionPipelineFactory(IList<PromotionRuleOption> promotionRuleOptions,
            ICartFactory cartFactory, IEnumerable<Sku> skus)
        {
            this.promotionRuleOptions = promotionRuleOptions ?? throw new ArgumentNullException(nameof(promotionRuleOptions));
            this.cartFactory = cartFactory ?? throw new ArgumentNullException(nameof(cartFactory));
            this.skus = skus ?? throw new ArgumentNullException(nameof(skus));
        }

        /// <inheritdoc/>
        public IPromotionPipeline Create()
        {
            var pipeline = new FaultTolerantPromotionPipeline();

            foreach (var promotionRuleOption in this.promotionRuleOptions)
            {
                var promotionRule = this.CreateRule(promotionRuleOption);
                if (promotionRule == null)
                {
                    throw new InvalidOperationException($"No promotion rule corresponding to '{promotionRuleOption.Id}'");
                }

                pipeline.AddRule(promotionRule);
            }

            return pipeline;
        }

        protected virtual IPromotionRule CreateRule(PromotionRuleOption promotionRuleOption)
        {
            if (promotionRuleOption.Id == CollectionOfSameSkuForRuleId)
            {
                // When we are generating the CollectionOfSameSkuForRule:
                // - Param1 is the SKU id
                // - Param2 is the quantity
                // - Param3 is the assigned price
                string skuId = ParamAsString(promotionRuleOption.Param1);
                int quantity = ParamAsInt(promotionRuleOption.Param2);
                decimal price = ParamAsDecimal(promotionRuleOption.Param3);

                Sku sku = this.skus.FirstOrDefault(sku => sku.Id == skuId);
                if (sku == null)
                {
                    throw new InvalidOperationException($"Could not find SKU '{skuId}' when creating rule '{promotionRuleOption.Id}'");
                }

                return new CollectionOfSameSkuForRule(this.cartFactory, sku, quantity, price);
            }

            if (promotionRuleOption.Id == PairOfDifferentSkusForRuleId)
            {
                // When we are generating the PairOfDifferentSkusForRule:
                // - Param1 is the first SKU id
                // - Param2 is the second SKU id
                // - Param3 is the assigned price
                string sku1Id = ParamAsString(promotionRuleOption.Param1);
                string sku2Id = ParamAsString(promotionRuleOption.Param2);
                decimal price = ParamAsDecimal(promotionRuleOption.Param3);

                Sku sku1 = this.skus.FirstOrDefault(sku => sku.Id == sku1Id);
                if (sku1 == null)
                {
                    throw new InvalidOperationException($"Could not find SKU '{sku1Id}' when creating rule '{promotionRuleOption.Id}'");
                }

                Sku sku2 = this.skus.FirstOrDefault(sku => sku.Id == sku2Id);
                if (sku2 == null)
                {
                    throw new InvalidOperationException($"Could not find SKU '{sku2Id}' when creating rule '{promotionRuleOption.Id}'");
                }

                return new PairOfDifferentSkusForRule(this.cartFactory, sku1, sku2, price);
            }

            return null;
        }

        private static string ParamAsString(string paramValue)
        {
            return paramValue as string ?? throw new InvalidCastException($"Could not retrieve string from '{paramValue}'");
        }

        private static int ParamAsInt(string paramValue)
        {
            bool succeeded = int.TryParse(paramValue, out int integerValue);
            if (!succeeded)
            {
                throw new InvalidCastException($"Could not retrieve integer from '{paramValue}'");
            }

            return integerValue;
        }

        private static decimal ParamAsDecimal(string paramValue)
        {
            bool succeeded = decimal.TryParse(paramValue, out decimal decimalValue);
            if (!succeeded)
            {
                throw new InvalidCastException($"Could not retrieve decimal from '{paramValue}'");
            }

            return decimalValue;
        }
    }
}
