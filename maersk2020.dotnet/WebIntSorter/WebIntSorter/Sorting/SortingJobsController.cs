using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Collections;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// A jobs controller using a dictionary to store jobs.
    /// </summary>
    /// <remarks>
    /// This class is not capable of handling a max size for the underlying dictionary.
    /// </remarks>
    public class SortingJobsController : ISortingJobsControllerAsync
    {
        private ConcurrentDictionary<Guid, SortingJob> jobs;

        public SortingJobsController()
        {
            this.jobs = new ConcurrentDictionary<Guid, SortingJob>();
        }

        /// <inheritdoc/>
        public Guid Create(IEnumerable<int> input)
        {
            return this.CreateAsync(input).GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public async Task<Guid> CreateAsync(IEnumerable<int> input)
        {
            return await Task<Guid>.Run(() => 
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                var job = new SortingJob()
                {
                    Duration = -1,
                    Status = SortingJobStatus.Pending,
                    Values = null
                };

                //this.jobs[job.Id] = job;

                // Start work execution for this job (without awaiting it)
                Task.Run(async () =>
                {
                    var sortedSequence = await input.SortIntegers();
                    stopwatch.Stop();

                    //this.jobs[job.Id].Values = sortedSequence.ToString();
                    //this.jobs[job.Id].Status = SortingJobStatus.Completed;
                    //this.jobs[job.Id].Duration = stopwatch.ElapsedMilliseconds;
                });

                // Return the id
                //return job.Id;
                return Guid.NewGuid();
            });
        }

        /// <inheritdoc/>
        public IEnumerable<SortingJob> Get()
        {
            return this.GetAsync().GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<SortingJob>> GetAsync()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public SortingJob Get(Guid id)
        {
            return this.GetAsync(id).GetAwaiter().GetResult();
        }

        /// <inheritdoc/>
        public async Task<SortingJob> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
