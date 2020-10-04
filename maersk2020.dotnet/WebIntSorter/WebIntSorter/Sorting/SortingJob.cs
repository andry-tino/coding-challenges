using System;
using System.Collections.Generic;
using System.Linq;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// Describes a job for sorting a list of numbers.
    /// </summary>
    public class SortingJob
    {
        private List<int> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingJob"/> class.
        /// </summary>
        /// <param name="originalValues">The original values to be sorted.</param>
        public SortingJob(IEnumerable<int> originalValues)
        {
            this.Id = Guid.NewGuid().ToString();
            this.Timestamp = DateTime.Now;
            this.Duration = -1;
            this.Status = SortingJobStatus.Pending;
            this.OriginalValues = originalValues ?? throw new ArgumentNullException(nameof(originalValues));
        }

        /// <summary>
        /// The unique ID assigned to this job.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// The timestamp of the moment the job was started.
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// Job diration in ms.
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// The job current status.
        /// </summary>
        public SortingJobStatus Status { get; set; }

        /// <summary>
        /// Sorted values.
        /// </summary>
        public IReadOnlyList<int> Values => this.values;

        /// <summary>
        /// Original values.
        /// </summary>
        public IEnumerable<int> OriginalValues { get; }

        /// <summary>
        /// Sorts the sequence. This will cause <see cref="Values"/> to be
        /// assigned the sorted sequence. <see cref="OriginalValues"/> will not change.
        /// </summary>
        public void Sort()
        {
            this.values = this.OriginalValues.SortIntegers().ToList();
        }
    }
}
