using System;
using System.Linq;

using Xunit;

using PromoEng.Engine.Rules;

namespace PromoEng.Engine.UnitTests.Rules
{
    /// <summary>
    /// Unit tests for <see cref="PairOfDifferentSkusForRule"/>.
    /// </summary>
    public class PairOfDifferentSkusForRuleTests
    {
        [Fact]
        public void WhenSameAmountOfBothSkuTypesThenAllAreBatched()
        {

        }

        [Fact]
        public void WhenNotSameAmountOfBothSkuTypesThenSomeResidualsAreLeft()
        {

        }

        [Fact]
        public void WhenOneAmountOfSkuBatchableTypesIsZeroThenNoBatchesAreCreated()
        {

        }

        [Fact]
        public void CorrectPricesWhenBatching()
        {

        }
    }
}
