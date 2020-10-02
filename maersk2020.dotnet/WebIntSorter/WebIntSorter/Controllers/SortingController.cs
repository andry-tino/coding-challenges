using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Challenge.WebIntSorter.Models;

namespace Challenge.WebIntSorter.Controllers
{
    /// <summary>
    /// API controller responsible for routing here the calls to "/api/sorting".
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SortingController : ControllerBase
    {
        private readonly ILogger<SortingController> logger;
        private readonly SortingJobContext dbContext;
        private SortingJobsController jobsController;

        public SortingController(ILogger<SortingController> logger, SortingJobContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext;
            this.jobsController = new SortingJobsController();
        }

        [HttpGet]
        public SortingJob Get()
        {
            var rng = new Random();
            return new SortingJob
            {
                Duration = 10,
                Status = SortingJobStatus.Pending,
                Values = (new int[] { 3, 5, 7 }).ToString()
            };
        }

        [HttpPost]
        public async Task<ActionResult<SortingJob>> Post()
        {
            //var jobId = await this.jobsController.CreateAsync(new int[] { 3, 1, 6 });
            var jobId = await this.CreateAsync(new int[] { 3, 1, 6 });

            return CreatedAtAction("post", new { id = jobId });
        }

        private async Task<long> CreateAsync(IEnumerable<int> input)
        {
            return await Task<long>.Run(() =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var job = new SortingJob()
                {
                    Duration = -1,
                    Status = SortingJobStatus.Pending,
                    Values = null
                };

                this.dbContext.AddJob(job);
                this.dbContext
                    // Save to DB, do not wait for this
                    .SaveChangesAsync()
                    // Then continue to sorting (still non-blocking)
                    .ContinueWith(async antecedent =>
                    {
                        var sortedSequence = await input.SortIntegers();
                        stopwatch.Stop();

                        //job.Values = sortedSequence.ToString();
                        job.IntegerValues = sortedSequence;
                        job.Status = SortingJobStatus.Completed;
                        job.Duration = stopwatch.ElapsedMilliseconds;
                        this.dbContext.SaveChanges();
                    });

                // Return the id
                return job.Id;
            });
        }
    }
}
