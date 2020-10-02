using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge.WebIntSorter.UnitTests
{
    [TestClass]
    public class SortingJobUnitTest
    {
        [TestMethod]
        public void WhenCreatedThenNewGuidIsAssigned()
        {
            var job = new SortingJob();
            Assert.IsNotNull(job.Id, "A new id should have been assigned");
        }

        [TestMethod]
        public void IdsAreUnique()
        {
            var job1 = new SortingJob();
            var job2 = new SortingJob();
            Assert.AreNotEqual(job1.Id, job2.Id, "Ids should be different");
        }

        [TestMethod]
        public void WhenCreatedThenTimestampIsAssigned()
        {
            var job = new SortingJob();
            Assert.IsNotNull(job.Timestamp, "A new timestamp should have been assigned");
        }
    }
}
