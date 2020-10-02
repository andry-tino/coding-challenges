using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Challenge.WebIntSorter.Controllers
{
    /// <summary>
    /// API controller responsible for routing here the calls to "/api/sorting".
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SortingController : ControllerBase
    {
        private readonly ILogger<SortingController> logger;
        private ConcurrentDictionary<Guid, IEnumerable<int>> jobs;

        public SortingController(ILogger<SortingController> logger)
        {
            this.logger = logger;
            this.jobs = new ConcurrentDictionary<Guid, IEnumerable<int>>();
        }

        [HttpGet]
        public SortingJob Get()
        {
            var rng = new Random();
            return new SortingJob
            {
                Duration = 10,
                Status = SortingJobStatus.Pending,
                Values = new int[] { 3, 5, 7 }
            };
        }

        [HttpPost]
        public async Task<ActionResult<SortingJob>> Post()
        {
            var rng = new Random();
            var values = await Task<SortingJob>.Run(() =>
            {
                return new SortingJob
                {
                    Duration = 10,
                    Status = SortingJobStatus.Pending,
                    Values = new int[] { 3, 5, 7 }
                };
            });

            return CreatedAtAction("post", values);
        }
    }
}
