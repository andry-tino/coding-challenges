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
        private IEnumerable<int> originalValues;

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
        /// In order to properly emit the values as a JSON array, property
        /// <see cref="Values"/> is serialized while this property will not
        /// be to avoid chattiness in the API.
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

                this.RawValues = ArrayToString(value);
            }
        }

        /// <summary>
        /// The job original values to sort.
        /// To have it mapped correctly in the entity, a primitive type is
        /// used: string representation, comma separated.
        /// </summary>
        /// <remarks>
        /// Do not use this when getting or setting the values in code,
        /// use <see cref="OriginalValues"/> instead.
        /// In order to properly emit the values as a JSON array, property
        /// <see cref="OriginalValues"/> is serialized while this property will not
        /// be to avoid chattiness in the API.
        /// </remarks>
        [JsonIgnore]
        public string RawOriginalValues { get; set; }

        /// <summary>
        /// Utility property to allow interfacing with <see cref="RawOriginalValues"/> using the proper type.
        /// </summary>
        [NotMapped]
        public IEnumerable<int> OriginalValues
        {
            get { return this.originalValues; }

            set
            {
                this.originalValues = value;

                if (value == null)
                {
                    this.RawOriginalValues = null;
                    return;
                }

                if (value.Count() == 0)
                {
                    this.RawOriginalValues = string.Empty;
                    return;
                }

                this.RawOriginalValues = ArrayToString(value);
            }
        }

        /// <summary>
        /// Makes sure that, in case either <see cref="RawValues"/> or
        /// <see cref="RawOriginalValues"/> was explicitely assigned,
        /// <see cref="Values"/> and <see cref="OriginalValues"/> get the correct values.
        /// </summary>
        /// <returns>
        /// The same object after the update to <see cref="Values"/> and
        /// <see cref="OriginalValues"/>.
        /// </returns>
        /// <remarks>
        /// Use this method in models when implementing derivates of
        /// <see cref="Microsoft.EntityFrameworkCore.DbContext"/>.
        /// </remarks>
        public SortingJob SyncValues()
        {
            SyncVariableFromRaw(this.RawValues, ref this.values);
            SyncVariableFromRaw(this.RawOriginalValues, ref this.originalValues);

            return this; // Allow chaining
        }

        private static void SyncVariableFromRaw(string rawValues, ref IEnumerable<int> values)
        {
            if (rawValues == null)
            {
                values = null;
            }
            else if (rawValues.Length == 0)
            {
                values = new int[0];
            }
            else
            {
                values = rawValues.Split(",").Select(s =>
                {
                    var success = int.TryParse(s, out int v);
                    return success ? v : 0;
                });
            }
        }

        private static string ArrayToString(IEnumerable<int> array)
        {
            return array.Select(n => n.ToString()).Aggregate((string a, string b) => $"{a},{b}");
        }
    }
}
