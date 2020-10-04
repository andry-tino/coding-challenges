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
        private readonly SortingJobCollection jobsCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingController"/> class.
        /// </summary>
        /// <param name="logger">The logger passed as dependency.</param>
        /// <param name="jobsCollection">The collection used to store jobs across different calls.</param>
        public SortingController(ILogger<SortingController> logger, SortingJobCollection jobsCollection)
        {
            this.logger = logger;
            this.jobsCollection = jobsCollection;
        }

        /// <summary>
        /// Gets all jobs.
        /// </summary>
        /// <returns>A collection of <see cref="SortingJob"/>.</returns>
        [HttpGet]
        public IEnumerable<SortingJob> GetAll()
        {
            return this.jobsCollection.RetrieveJobs();
        }

        /// <summary>
        /// Gets one specific job.
        /// </summary>
        /// <param name="id">The id of the job to look for.</param>
        /// <returns>The requested <see cref="SortingJob"/>.</returns>
        [HttpGet("{id}")]
        public SortingJob Get(string id)
        {
            return this.jobsCollection.RetrieveJob(id);
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
        public ActionResult PostAsync(IEnumerable<int> sequence)
        {
            // Validate input
            if (sequence == null)
            {
                this.logger.LogError($"Numeric sequence provided is null, cannot enqueue job");
                throw new ArgumentException("A numeric sequence is required", nameof(sequence));
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
            var job = new SortingJob(input);

            this.jobsCollection.AddJob(job);

            return job;
        }

        private void EnqueueJob(SortingJob job)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                // Execute the sorting logic
                job.Sort();
                job.Status = SortingJobStatus.Completed;
            }
            catch (InvalidOperationException e)
            {
                this.logger.LogError($"Error while executing sorting job '{job.Id}': {e.Message}");
                throw e;
            }
            finally
            {
                // Tracked time: until sorting is complete
                stopwatch.Stop();

                // Update the job
                job.Duration = stopwatch.ElapsedMilliseconds;
            }
        }
    }
}
