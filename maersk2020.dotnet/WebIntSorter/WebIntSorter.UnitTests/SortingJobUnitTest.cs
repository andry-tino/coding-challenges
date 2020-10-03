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
        }

        [TestMethod]
        public void WhenCreatedThenIntegerValuesIsNull()
        {
            var job = new SortingJob();
            Assert.IsNull(job.IntegerValues, "IntegerValues should be null when Values is null");
        }

        [TestMethod]
        public void WhenIntegerValuesIsNullThenValuesIsNull()
        {
            var job = new SortingJob();
            job.IntegerValues = null;
            Assert.IsNull(job.Values, "Values should be null when Values is null");
        }

        [TestMethod]
        public void WhenIntegerValuesIsEmptyCollectionThenValuesIsEmptyString()
        {
            var job = new SortingJob();
            job.IntegerValues = new int[0];
            Assert.IsNotNull(job.Values, "Values should not be null when IntegerValues is empty");
            Assert.AreEqual(0, job.Values.Length, "Values should be empty string when IntegerValues is empty");
        }

        [TestMethod]
        public void WhenCreatedThenTimestampIsAssigned()
        {
            var job = new SortingJob();
            Assert.IsNotNull(job.Timestamp, "A new timestamp should have been assigned");
        }
    }
}
