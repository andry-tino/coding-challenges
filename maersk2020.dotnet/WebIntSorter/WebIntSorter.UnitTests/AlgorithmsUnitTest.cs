using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge.WebIntSorter.UnitTests
{
    [TestClass]
    public class AlgorithmsUnitTest
    {
        [TestMethod]
        public async Task WhenNullArrayIsProvidedThenNullArrayIsReturned()
        {
            var output = await Algorithms.SortIntegers(null);
            Assert.IsNull(output,
                "When sorting a null sequence, a null sequence should be returned");
        }

        [TestMethod]
        public async Task WhenEmptyArrayIsProvidedThenEmptyArrayIsReturned()
        {
            var input = new int[0];
            var output = await Algorithms.SortIntegers(input);
            Assert.AreEqual(input.Count(), output.Count(),
                "When sorting an empty sequence, an empty sequence should be returned");
        }

        [TestMethod]
        public async Task WhenOneElementArrayIsProvidedThenOneElementArrayIsReturned()
        {
            var input = new int[] { 1 };
            var output = await Algorithms.SortIntegers(input);
            TestSequences(new int[] { 1 }, output);
        }

        [TestMethod]
        public async Task SequenceIsCorrectlySorted()
        {
            var input = new int[] { 1, 7, 3, 5 };
            var output = await Algorithms.SortIntegers(input);
            TestSequences(new int[] { 1, 3, 5, 7 }, output);
        }

        [TestMethod]
        public async Task SequenceWithDuplicatesIsCorrectlySorted()
        {
            var input = new int[] { 1, 7, 3, 5, 3, 1 };
            var output = await Algorithms.SortIntegers(input);
            TestSequences(new int[] { 1, 1, 3, 3, 5, 7 }, output);
        }

        private static void TestSequences(IEnumerable<int> expected, IEnumerable<int> actual)
        {
            Assert.AreEqual(expected.Count(), actual.Count(), "Sequences lengths do not match");
            Assert.IsTrue(Enumerable.SequenceEqual<int>(expected, actual), "Sequences do not match");
        }
    }
}
