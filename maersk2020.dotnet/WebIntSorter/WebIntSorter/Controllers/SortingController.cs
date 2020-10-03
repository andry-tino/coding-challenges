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

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingController"/> class.
        /// </summary>
        /// <param name="logger">The logger passed as dependency.</param>
        /// <param name="dbContext">The database context passed as dependency.</param>
        public SortingController(ILogger<SortingController> logger, SortingJobContext dbContext)
        {
            this.logger = logger;
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet]
        public IEnumerable<SortingJob> Get()
        {
            return this.dbContext.RetrieveJobs();
        }

        [HttpPost]
        public async Task<ActionResult> Post(SortingJob input)
        {
            // Validate input
            var sequence = input.IntegerValues;
            if (sequence == null)
            {
                throw new ArgumentException("A numeric sequence is required", nameof(input));
            }

            // Start background job
            var jobId = await this.CreateAsync(sequence);

            // Return response
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
