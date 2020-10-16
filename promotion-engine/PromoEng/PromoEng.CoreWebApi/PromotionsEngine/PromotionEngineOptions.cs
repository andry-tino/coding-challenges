using System;
using System.Collections.Generic;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents a configuration object for setting up the server.
    /// </summary>
    internal class PromotionEngineOptions
    {
        /// <summary>
        /// The configuration key in the settings file.
        /// </summary>
        public const string Position = "PromotionEngine";

        /// <summary>
        /// Gets or sets the collection of promotion rules
        /// </summary>
        public IEnumerable<PromotionRuleOption> Rules { get; set; }

        /// <summary>
        /// Gets or sets the collection of available SKUs.
        /// </summary>
        public IEnumerable<string> Skus { get; set; }
    }

    /// <summary>
    /// Represents a configuration key for one promotion rule.
    /// </summary>
    internal class PromotionRuleOption
    {
        /// <summary>
        /// Gets or sets the name (identifier) of the rule.
        /// </summary>
        public string Name { get; set; }
    }

    /// <summary>
    /// Represents a configuration key for one SKU.
    /// </summary>
    internal class SkuOption
    {
        /// <summary>
        /// Gets or sets the identifier of the SKU.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the friendly name of the SKU.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the unit price of the SKU.
        /// </summary>
        public decimal UnitPrice { get; set; }
    }
}
