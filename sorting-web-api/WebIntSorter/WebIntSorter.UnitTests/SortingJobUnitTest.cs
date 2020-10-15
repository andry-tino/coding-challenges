using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge.WebIntSorter.UnitTests
{
    [TestClass]
    public class SortingJobUnitTest
    {
        [TestMethod]
        public void WhenCreatedThenIdIsAssigned()
        {
            var job = new SortingJob(new int[0]);
            Assert.IsNotNull(job.Id, "A new id should have been assigned");
        }

        [TestMethod]
        public void IdIsUnique()
        {
            var job1 = new SortingJob(new int[0]);
            var job2 = new SortingJob(new int[0]);
            Assert.AreNotEqual(job1.Id, job2.Id, "Ids should be unique");
        }

        [TestMethod]
        public void WhenCreatedThenTimestampIsAssigned()
        {
            var job = new SortingJob(new int[0]);
            Assert.IsNotNull(job.Timestamp, "A new timestamp should have been assigned");
        }

        [TestMethod]
        public void WhenCreatedThenDurationIsNegativeOne()
        {
            var job = new SortingJob(new int[0]);
            Assert.AreEqual(-1, job.Duration, "When created, duration should be -1");
        }

        [TestMethod]
        public void WhenCreatedThenStatusIsPending()
        {
            var job = new SortingJob(new int[0]);
            Assert.AreEqual(SortingJobStatus.Pending, job.Status, "When created, status should be pending");
        }

        [TestMethod]
        public void WhenCreatedThenValuesIsNull()
        {
            var job = new SortingJob(new int[0]);
            Assert.IsNull(job.Values, "When created, values should be null");
        }
    }
}
