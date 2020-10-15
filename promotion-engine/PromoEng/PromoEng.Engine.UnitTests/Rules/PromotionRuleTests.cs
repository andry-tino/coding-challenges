using System;
using System.Linq;

using Xunit;

namespace PromoEng.Engine.UnitTests.Rules
{
    /// <summary>
    /// Common unit tests for all <see cref="IPromotionRule"/>.
    /// </summary>
    public class PromotionRuleTests
    {
        [Theory]
        [InlineData(null)]
        public void RulesApplicationDoesNotChangeTheTotalNumberOfItemsInCart(IPromotionRule rule)
        {
            
        }

        [Theory]
        [InlineData(null)]
        public void RulesApplicationChangesNothingWhenAllEntriesAreAlreadyMarkedAsHandled(IPromotionRule rule)
        {

        }
    }
}
