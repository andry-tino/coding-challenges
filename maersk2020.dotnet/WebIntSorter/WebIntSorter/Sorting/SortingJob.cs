using System;
using System.Collections.Generic;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// Describes a job for sorting a list of numbers.
    /// </summary>
    public class SortingJob
    {
        /// <summary>
        /// The unique ID assigned to this job.
        /// </summary>
        public Guid Id => Guid.NewGuid();

        /// <summary>
        /// The timestamp of the moment the job was started.
        /// </summary>
        public DateTime Timestamp => DateTime.Now;

        /// <summary>
        /// Job diration in ms.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// The job current status.
        /// </summary>
        public SortingJobStatus Status { get; set; }

        /// <summary>
        /// The job result.
        /// </summary>
        public IEnumerable<int> Values { get; set; }
    }
}
