using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge.WebIntSorter.UnitTests
{
    [TestClass]
    public class SortingJobUnitTest
    {
        [TestMethod]
        public void ValuesMapToValues()
        {
            var job = new SortingJob();
            var sequence = new int[] { 1, 3 };
            job.Values = sequence.Clone() as int[];

            Assert.IsTrue(Enumerable.SequenceEqual<int>(sequence, job.Values), "Unexpected stored value");
            Assert.AreEqual("1,3", job.RawValues, "RawValues were not properly mapped from IntegerValues");
        }

        [TestMethod]
        public void WhenValuesIsNullThenRawValuesIsNull()
        {
            var job = new SortingJob();
            job.Values = null;
            Assert.IsNull(job.RawValues, "Values should be null when RawValues is null");
        }

        [TestMethod]
        public void WhenValuesIsEmptyCollectionThenRawValuesIsEmptyString()
        {
            var job = new SortingJob();
            job.Values = new int[0];
            Assert.IsNotNull(job.RawValues, "RawValues should not be null when IntegerValues is empty");
            Assert.AreEqual(0, job.RawValues.Length, "RawValues should be empty string when IntegerValues is empty");
        }

        [TestMethod]
        public void WhenRawValuesIsModifiedAndSyncCalledThenValuesHasCorrectValue()
        {
            var job = new SortingJob();
            job.RawValues = "4,3";
            job.SyncValues();
            Assert.IsNotNull(job.Values, "IntegerValues should not be null");
            Assert.IsTrue(Enumerable.SequenceEqual<int>(new int[] { 4, 3 }, job.Values), "Sync failed");
        }

        [TestMethod]
        public void WhenCreatedThenRawValuesIsNull()
        {
            var job = new SortingJob();
            Assert.IsNull(job.RawValues, "RawValues should be null");
        }

        [TestMethod]
        public void WhenCreatedThenValuesIsNull()
        {
            var job = new SortingJob();
            Assert.IsNull(job.Values, "IntegerValues should be null when RawValues is null");
        }

        [TestMethod]
        public void WhenCreatedThenTimestampIsAssigned()
        {
            var job = new SortingJob();
            Assert.IsNotNull(job.Timestamp, "A new timestamp should have been assigned");
        }
    }
}
