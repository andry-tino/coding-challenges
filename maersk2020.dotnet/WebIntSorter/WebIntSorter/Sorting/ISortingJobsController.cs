using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// Describes a component capable of handling sorting jobs.
    /// </summary>
    public interface ISortingJobsController
    {
        /// <summary>
        /// Creates a new job.
        /// </summary>
        /// <param name="input">The sequence to sort.</param>
        /// <returns>The id of the newly created job.</returns>
        Guid Create(IEnumerable<int> input);

        /// <summary>
        /// Gets all jobs.
        /// </summary>
        /// <returns>The collection of jobs. Empty collection if none.</returns>
        IEnumerable<SortingJob> Get();

        /// <summary>
        /// Gets a single job.
        /// </summary>
        /// <param name="id">The id of the job.</param>
        /// <returns>The job, <c>null</c> if not found.</returns>
        SortingJob Get(Guid id);
    }

    /// <summary>
    /// Describes a component capable of handling sorting jobs asynchronously.
    /// </summary>
    public interface ISortingJobsControllerAsync : ISortingJobsController
    {
        /// <summary>
        /// Creates a new job asynchronously.
        /// </summary>
        /// <param name="input">The sequence to sort.</param>
        /// <returns>The id of the newly created job.</returns>
        Task<Guid> CreateAsync(IEnumerable<int> input);

        /// <summary>
        /// Gets all jobs asynchronously.
        /// </summary>
        /// <returns>The collection of jobs. Empty collection if none.</returns>
        Task<IEnumerable<SortingJob>> GetAsync();

        /// <summary>
        /// Gets a single job asynchronously.
        /// </summary>
        /// <param name="id">The id of the job.</param>
        /// <returns>The job, <code>null</code> if not found.</returns>
        Task<SortingJob> GetAsync(Guid id);
    }
}
