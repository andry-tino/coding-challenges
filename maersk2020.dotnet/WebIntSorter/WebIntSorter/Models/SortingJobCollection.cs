using System;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Challenge.WebIntSorter.Models
{
    /// <summary>
    /// Handles the memorization of jobs.
    /// </summary>
    public class SortingJobCollection
    {
        private readonly IDictionary<string, SortingJob> jobs;

        public SortingJobCollection()
        {
            this.jobs = new ConcurrentDictionary<string, SortingJob>();
        }

        /// <summary>
        /// Adds a new job in the database.
        /// </summary>
        /// <param name="job"></param>
        public void AddJob(SortingJob job)
        {
            this.jobs.Add(job.Id, job);
        }

        /// <summary>
        /// Retrieves a job from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SortingJob RetrieveJob(string id)
        {
            this.jobs.TryGetValue(id, out SortingJob job);
            return job;
        }

        /// <summary>
        /// Gets all stored jobs.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SortingJob> RetrieveJobs()
        {
            return this.jobs.Values;
        }
    }
}
