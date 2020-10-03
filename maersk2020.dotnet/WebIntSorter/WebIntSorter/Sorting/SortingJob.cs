using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// Describes a job for sorting a list of numbers.
    /// </summary>
    public class SortingJob
    {
        private IEnumerable<int> integerValues;

        /// <summary>
        /// The unique ID assigned to this job.
        /// </summary>
        /// <remarks>
        /// This will act as primary key of the entity.
        /// </remarks>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// The timestamp of the moment the job was started.
        /// </summary>
        public DateTime Timestamp => DateTime.Now;

        /// <summary>
        /// Job diration in ms.
        /// </summary>
        public long Duration { get; set; }

        /// <summary>
        /// The job current status.
        /// </summary>
        public SortingJobStatus Status { get; set; }

        /// <summary>
        /// The job result.
        /// To have it mapped correctly in the entity, a primitive type is
        /// used: string representation, comma separated.
        /// </summary>
        /// <remarks>
        /// Do not use this when getting or setting the values in code,
        /// use <see cref="IntegerValues"/> instead.
        /// </remarks>
        public string Values { get; set; } // RENAME TO TO '_RawValues'

        /// <summary>
        /// Utility property to allow interfacing with <see cref="Values"/> using the proper type.
        /// </summary>
        [NotMapped]
        public IEnumerable<int> IntegerValues // RENAME TO 'Values'
        {
            get { return this.integerValues; }

            set
            {
                this.integerValues = value;

                if (value == null)
                {
                    this.Values = null;
                    return;
                }

                if (value.Count() == 0)
                {
                    this.Values = string.Empty;
                    return;
                }

                this.Values = value.Select(n => n.ToString()).Aggregate((string a, string b) => $"{a},{b}");
            }
        }
    }
}
