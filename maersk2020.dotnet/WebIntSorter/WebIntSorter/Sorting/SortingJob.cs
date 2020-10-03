using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// Describes a job for sorting a list of numbers.
    /// </summary>
    public class SortingJob
    {
        private IEnumerable<int> values;

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
        /// use <see cref="Values"/> instead.
        /// </remarks>
        [JsonIgnore]
        public string RawValues { get; set; }

        /// <summary>
        /// Utility property to allow interfacing with <see cref="RawValues"/> using the proper type.
        /// </summary>
        [NotMapped]
        public IEnumerable<int> Values
        {
            get { return this.values; }

            set
            {
                this.values = value;

                if (value == null)
                {
                    this.RawValues = null;
                    return;
                }

                if (value.Count() == 0)
                {
                    this.RawValues = string.Empty;
                    return;
                }

                this.RawValues = value.Select(n => n.ToString()).Aggregate((string a, string b) => $"{a},{b}");
            }
        }

        /// <summary>
        /// Makes sure that, in case <see cref="Values"/> was explicitely assigned,
        /// <see cref="IntegerValues"/> gets the correct value.
        /// </summary>
        /// <returns>The same object after the update to <see cref="IntegerValues"/>.</returns>
        public SortingJob SyncValues()
        {
            if (this.RawValues == null)
            {
                this.values = null;
            }
            else if (this.RawValues.Length == 0)
            {
                this.values = new int[0];
            }
            else
            {
                this.values = this.RawValues.Split(",").Select(s =>
                {
                    var success = int.TryParse(s, out int v);
                    return success ? v : 0;
                });
            }

            return this;
        }
    }
}
