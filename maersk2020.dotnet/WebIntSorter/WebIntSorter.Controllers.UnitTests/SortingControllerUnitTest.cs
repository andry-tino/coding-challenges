using System;
using System.Linq;

using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

using Challenge.WebIntSorter.Controllers;
using Challenge.WebIntSorter.Models;

namespace WebIntSorter.Controllers.UnitTests
{
    public class SortingControllerUnitTest
    {
        [Fact]
        public void WhenNonExistingJobIsRequestedThenNullIsReturned()
        {
            var jobsDb = new SortingJobCollection();
            Assert.Empty(jobsDb.RetrieveJobs());

            var controller = new SortingController(
                Mocks.GetMockedLogger<SortingController>(),
                jobsDb);

            var job = controller.Get(Guid.Empty.ToString());

            Assert.Null(job);
        }

        [Fact]
        public void WhenNoJobsThenGetAllReturnsEmptyCollection()
        {
            var jobsDb = new SortingJobCollection();
            Assert.Empty(jobsDb.RetrieveJobs());

            var controller = new SortingController(
                Mocks.GetMockedLogger<SortingController>(),
                jobsDb);

            var jobs = controller.GetAll();

            Assert.NotNull(jobs);
            Assert.Empty(jobs);
        }

        [Fact]
        public void WhenAddingNewJobThenJobIdIsReturned()
        {
            var jobsDb = new SortingJobCollection();
            Assert.Empty(jobsDb.RetrieveJobs());

            var controller = new SortingController(
                Mocks.GetMockedLogger<SortingController>(),
                jobsDb);

            SortingController.CreateJobResponse response = controller.Post(new int[] { 2, 1 });

            Assert.NotNull(response);
            Assert.NotNull(response.Id);
            Assert.NotEmpty(response.Id);
        }

        [Fact]
        public void WhenAddingNewJobThenCollectionImmediatelyHasNewJob()
        {
            var jobsDb = new SortingJobCollection();
            Assert.Empty(jobsDb.RetrieveJobs());

            var controller = new SortingController(
                Mocks.GetMockedLogger<SortingController>(),
                jobsDb);

            var jobs = controller.Post(new int[] { 2, 1 });

            Assert.NotEmpty(jobsDb.RetrieveJobs());

            var job = jobsDb.RetrieveJobs().First();
            Assert.NotNull(job);
        }
    }
}
