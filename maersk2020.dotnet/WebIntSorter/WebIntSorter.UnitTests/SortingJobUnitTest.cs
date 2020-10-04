using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Challenge.WebIntSorter.UnitTests
{
    [TestClass]
    public class SortingJobUnitTest
    {
        [TestMethod]
        public void WhenCreatedThenTimestampIsAssigned()
        {
            var job = new SortingJob(new int[0]);
            Assert.IsNotNull(job.Timestamp, "A new timestamp should have been assigned");
        }
    }
}
