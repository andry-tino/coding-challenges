using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace Challenge.WebIntSorter.Models
{
    /// <summary>
    /// The model to use to store and retrieve <see cref="SortingJob"/>s.
    /// </summary>
    public class SortingJobContext : DbContext
    {
        public SortingJobContext(DbContextOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the database entry point for the model.
        /// </summary>
        public DbSet<SortingJob> Jobs { get; set; }

        /// <summary>
        /// Adds a new job in the database.
        /// </summary>
        /// <param name="job"></param>
        public void AddJob(SortingJob job)
        {
            this.Jobs.Add(job);
        }

        /// <summary>
        /// Retrieves a job from the database.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SortingJob RetrieveJob(long id)
        {
            return this.Jobs.FirstOrDefault(job => job.Id == id);
        }

        /// <summary>
        /// Gets all stored jobs.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SortingJob> RetrieveJob()
        {
            return this.Jobs.ToArray();
        }
    }
}
