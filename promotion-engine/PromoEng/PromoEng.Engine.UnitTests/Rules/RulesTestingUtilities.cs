using System;

using Xunit;

namespace PromoEng.Engine.UnitTests
{
    /// <summary>
    /// Class containing utility methods for checking rules.
    /// </summary>
    internal static class RulesTestingUtilities
    {
        /// <summary>
        /// Tests that an entry was processed by a rule.
        /// </summary>
        /// <param name="entry">The entry to test.</param>
        public static void CheckCartEntryWasProcessedByRule(this SkuCartEntry entry)
        {
            Assert.NotNull(entry.PromotionRuleId);
            Assert.NotNull(entry.Description);
        }

        /// <summary>
        /// Tests that an entry was not processed by a rule.
        /// </summary>
        /// <param name="entry">The entry to test.</param>
        public static void CheckCartEntryWasNotProcessedByRule(this SkuCartEntry entry)
        {
            Assert.Null(entry.PromotionRuleId);
        }
    }
}
