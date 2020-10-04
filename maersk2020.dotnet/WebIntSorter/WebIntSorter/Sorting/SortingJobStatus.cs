using System;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// Describes the status of a sorting job.
    /// </summary>
    public enum SortingJobStatus
    {
        /// <summary>
        /// The job is currently ongoing.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// The job is done and its result can be retrieved.
        /// </summary>
        Completed = 1,

        /// <summary>
        /// The job is not running anymore as it terminated due to an error.
        /// The result cannot be retrieved.
        /// </summary>
        Error = 2
    }
}
