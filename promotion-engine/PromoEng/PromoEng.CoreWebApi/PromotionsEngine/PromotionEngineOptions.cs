using System;
using System.Collections.Generic;

namespace PromoEng.CoreWebApi
{
    /// <summary>
    /// Represents a configuration object for setting up the server.
    /// </summary>
    public class PromotionEngineOptions
    {
        /// <summary>
        /// The configuration key in the settings file.
        /// </summary>
        public const string ConfigurationKeyName = "PromotionEngine";

        /// <summary>
        /// Gets or sets the collection of promotion rules
        /// </summary>
        public IList<PromotionRuleOption> Rules { get; set; }

        /// <summary>
        /// Gets or sets the collection of available SKUs.
        /// </summary>
        public IEnumerable<SkuOption> Skus { get; set; }
    }

    /// <summary>
    /// Represents a configuration key for one promotion rule.
    /// </summary>
    public class PromotionRuleOption
    {
        /// <summary>
        /// Gets or sets the name (identifier) of the rule.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the first parameter of the rule.
        /// </summary>
        public string Param1 { get; set; }

        /// <summary>
        /// Gets or sets the second parameter of the rule.
        /// </summary>
        public string Param2 { get; set; }

        /// <summary>
        /// Gets or sets the third parameter of the rule.
        /// </summary>
        public string Param3 { get; set; }

        /// <summary>
        /// Gets or sets the fourth parameter of the rule.
        /// </summary>
        public string Param4 { get; set; }
    }

    /// <summary>
    /// Represents a configuration key for one SKU.
    /// </summary>
    public class SkuOption
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
