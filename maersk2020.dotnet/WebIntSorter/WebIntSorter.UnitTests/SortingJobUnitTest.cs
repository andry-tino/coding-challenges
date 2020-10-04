using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge.WebIntSorter.UnitTests
{
    [TestClass]
    public class SortingJobUnitTest
    {
        [TestMethod]
        public void ValuesMapToRawValues()
        {
            var job = new SortingJob();
            var sequence = new int[] { 1, 3 };
            job.Values = sequence.Clone() as int[];

            Assert.IsTrue(Enumerable.SequenceEqual<int>(sequence, job.Values), "Unexpected stored value");
            Assert.AreEqual("1,3", job.RawValues, "RawValues were not properly mapped from Values");
        }

        [TestMethod]
        public void OriginalValuesMapToRawOriginalValues()
        {
            var job = new SortingJob();
            var sequence = new int[] { 1, 3 };
            job.OriginalValues = sequence.Clone() as int[];

            Assert.IsTrue(Enumerable.SequenceEqual<int>(sequence, job.OriginalValues), "Unexpected stored value");
            Assert.AreEqual("1,3", job.RawOriginalValues, "RawOriginalValues were not properly mapped from OriginalValues");
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
            Assert.IsNotNull(job.RawValues, "RawValues should not be null when Values is empty");
            Assert.AreEqual(0, job.RawValues.Length, "RawValues should be empty string when Values is empty");
        }

        [TestMethod]
        public void WhenOriginalValuesIsNullThenRawOriginalValuesIsNull()
        {
            var job = new SortingJob();
            job.OriginalValues = null;
            Assert.IsNull(job.RawOriginalValues, "OriginalValues should be null when RawOriginalValues is null");
        }

        [TestMethod]
        public void WhenOriginalValuesIsEmptyCollectionThenRawOriginalValuesIsEmptyString()
        {
            var job = new SortingJob();
            job.OriginalValues = new int[0];
            Assert.IsNotNull(job.RawOriginalValues, "RawOriginalValues should not be null when OriginalValues is empty");
            Assert.AreEqual(0, job.RawOriginalValues.Length, "RawOriginalValues should be empty string when OriginalValues is empty");
        }

        [TestMethod]
        public void WhenRawValuesIsModifiedAndSyncCalledThenValuesHasCorrectValue()
        {
            var job = new SortingJob();
            job.RawValues = "4,3";
            job.SyncValues();
            Assert.IsNotNull(job.Values, "Values should not be null");
            Assert.IsTrue(Enumerable.SequenceEqual<int>(new int[] { 4, 3 }, job.Values), "Sync failed");
        }

        [TestMethod]
        public void WhenRawOriginalValuesIsModifiedAndSyncCalledThenOriginalValuesHasCorrectValue()
        {
            var job = new SortingJob();
            job.RawOriginalValues = "4,3";
            job.SyncValues();
            Assert.IsNotNull(job.OriginalValues, "OriginalValues should not be null");
            Assert.IsTrue(Enumerable.SequenceEqual<int>(new int[] { 4, 3 }, job.OriginalValues), "Sync failed");
        }

        [TestMethod]
        public void SyncValuesWhenRawValuesIsNull()
        {
            var job = new SortingJob();
            job.SyncValues();
            Assert.IsNull(job.Values, "Values should be null");
        }

        [TestMethod]
        public void SyncValuesWhenRawValuesIsEmptyString()
        {
            var job = new SortingJob();
            job.RawValues = string.Empty;
            job.SyncValues();
            Assert.IsNotNull(job.Values, "Values should not be null");
            Assert.AreEqual(0, job.Values.Count());
        }

        [TestMethod]
        public void SyncValuesWhenRawOriginalValuesIsNull()
        {
            var job = new SortingJob();
            job.SyncValues();
            Assert.IsNull(job.OriginalValues, "OriginalValues should be null");
        }

        [TestMethod]
        public void SyncValuesWhenRawOriginalValuesIsEmptyString()
        {
            var job = new SortingJob();
            job.RawOriginalValues = string.Empty;
            job.SyncValues();
            Assert.IsNotNull(job.OriginalValues, "OriginalValues should not be null");
            Assert.AreEqual(0, job.OriginalValues.Count());
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
            Assert.IsNull(job.Values, "Values should be null when RawValues is null");
        }

        [TestMethod]
        public void WhenCreatedThenRawOriginalValuesIsNull()
        {
            var job = new SortingJob();
            Assert.IsNull(job.RawOriginalValues, "RawOriginalValues should be null");
        }

        [TestMethod]
        public void WhenCreatedThenOriginalValuesIsNull()
        {
            var job = new SortingJob();
            Assert.IsNull(job.OriginalValues, "OriginalValues should be null when RawValues is null");
        }

        [TestMethod]
        public void WhenCreatedThenTimestampIsAssigned()
        {
            var job = new SortingJob();
            Assert.IsNotNull(job.Timestamp, "A new timestamp should have been assigned");
        }
    }
}
