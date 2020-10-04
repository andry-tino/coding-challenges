using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Cors;

using Challenge.WebIntSorter.Models;

namespace Challenge.WebIntSorter.Controllers
{
    /// <summary>
    /// API controller responsible for routing here the calls to "/api/sorting".
    /// </summary>
    [ApiController]
    [EnableCors(Constants.Service.CorsReactClientAllowSpecificOriginsPolicyName)]
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

        /// <summary>
        /// Gets all jobs.
        /// </summary>
        /// <returns>A collection of <see cref="SortingJob"/>.</returns>
        [HttpGet]
        public IEnumerable<SortingJob> GetAll()
        {
            return this.dbContext.RetrieveJobs();
        }

        /// <summary>
        /// Gets one specific job.
        /// </summary>
        /// <param name="id">The id of the job to look for.</param>
        /// <returns>The requested <see cref="SortingJob"/>.</returns>
        [HttpGet("{id}")]
        public SortingJob Get(long id)
        {
            return this.dbContext.RetrieveJob(id);
        }

        /// <summary>
        /// Creates a new job.
        /// </summary>
        /// <param name="input">The input <see cref="SortingJob"/> which must ensure to have values defined.</param>
        /// <returns>The id of the created job.</returns>
        /// <remarks>
        /// In order to optimize for responsiveness, jobs to sort sequences are run asynchronously.
        /// This way, the requestor will not have to wait for the job to be completed, but it will
        /// be allowed to query the job status via the <see cref="Get"/> method.
        /// </remarks>
        [HttpPost]
        public async Task<ActionResult> PostAsync(SortingJob input)
        {
            // Validate input
            var sequence = input.Values;
            if (sequence == null)
            {
                this.logger.LogError($"Numeric sequence provided is null, cannot enqueue job");
                throw new ArgumentException("A numeric sequence is required", nameof(input));
            }

            // Start background job
            var job = this.Create(sequence);
            Task.Run(() => this.EnqueueJob(job));

            this.logger.LogInformation($"Enqueued sorting job '{job.Id}' with {sequence.Count()} elements to sort");

            // Return response
            return CreatedAtAction("post", new { id = job.Id });
        }

        private SortingJob Create(IEnumerable<int> input)
        {
            var job = new SortingJob()
            {
                Duration = -1,
                Status = SortingJobStatus.Pending,
                OriginalValues = input
            };

            this.dbContext.AddJob(job);
            // Save the job now, later it will be updated
            // SaveChanges will assign an id
            this.dbContext.SaveChanges();

            return job;
        }

        private void EnqueueJob(SortingJob job)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                // Execute the sorting logic
                IEnumerable<int> sortedSequence = null;
                try
                {
                    sortedSequence = job.OriginalValues.SortIntegers();
                    job.Status = SortingJobStatus.Completed;
                }
                catch (Exception e)
                {
                    this.logger.LogError($"Error while sorting when executing job '{job.Id}': {e.Message}. Operation continues");
                    job.Status = SortingJobStatus.Error;
                }
                // Tracked time: until sorting is complete
                stopwatch.Stop();

                // Update the job
                job.Values = sortedSequence;
                job.Duration = stopwatch.ElapsedMilliseconds;
                this.dbContext.SaveChanges();
            }
            catch (Exception e)
            {
                this.logger.LogError($"Error while executing job '{job.Id}': {e.Message}");
                throw e;
            }
            finally
            {
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }
            }
        }
    }
}
