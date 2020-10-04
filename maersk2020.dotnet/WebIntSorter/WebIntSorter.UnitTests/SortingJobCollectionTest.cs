using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Challenge.WebIntSorter.Models;

namespace Challenge.WebIntSorter.UnitTests
{
    [TestClass]
    public class SortingJobCollectionTest
    {
        [TestMethod]
        public void AddAndRetrieveJob()
        {
            var collection = new SortingJobCollection();
            var job = new SortingJob(new int[0]);

            collection.AddJob(job);
            var retrievedJob = collection.RetrieveJob(job.Id);
            Assert.IsNotNull(retrievedJob, "Retrieval was not successful");
        }

        [TestMethod]
        public void CollectionHoldsReferenceToJobs()
        {
            var collection = new SortingJobCollection();
            var job = new SortingJob(new int[0]);

            collection.AddJob(job);
            var retrievedJob = collection.RetrieveJob(job.Id);
            Assert.IsTrue((object)job == (object)retrievedJob, "Collection is not properly storing jobs");
        }

        [TestMethod]
        public void RetrieveAllJobs()
        {
            var collection = new SortingJobCollection();
            var job1 = new SortingJob(new int[0]);
            var job2 = new SortingJob(new int[0]);

            collection.AddJob(job1);
            collection.AddJob(job2);
            var jobs = collection.RetrieveJobs();
            Assert.IsTrue(jobs.Count() > 0, "Retrieval of all jobs was not successful");
        }
    }
}
