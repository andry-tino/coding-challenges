using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge.WebIntSorter.UnitTests
{
    [TestClass]
    public class SortingJobUnitTest
    {
        [TestMethod]
        public void IntegerValuesMapToValues()
        {
            var job = new SortingJob();
            var sequence = new int[] { 1, 3 };
            job.IntegerValues = sequence.Clone() as int[];

            Assert.IsTrue(Enumerable.SequenceEqual<int>(sequence, job.IntegerValues), "Unexpected stored value");
            Assert.AreEqual("1,3", job.Values, "Values were not properly mapped from IntegerValues");

            job.Values = "4,3";
            Assert.IsTrue(Enumerable.SequenceEqual<int>(new int[] { 4, 3 }, job.IntegerValues), "Unexpected stored value");
        }

        [TestMethod]
        public void WhenValuesIsNullThenIntegerValuesIsNull()
        {
            var job = new SortingJob();
            Assert.IsNull(job.IntegerValues, "IntegerValues should be null when Values is null");
        }

        [TestMethod]
        public void WhenCreatedThenTimestampIsAssigned()
        {
            var job = new SortingJob();
            Assert.IsNotNull(job.Timestamp, "A new timestamp should have been assigned");
        }
    }
}
