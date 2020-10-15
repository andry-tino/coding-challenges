using System;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Challenge.WebIntSorter.Controllers
{
    /// <summary>
    /// API controller responsible for routing here the calls to "/api/sorting".
    /// </summary>
    [ApiController]
    [Route("api")]
    public class ApiRootController : ControllerBase
    {
        private readonly ILogger<SortingController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRootController"/> class.
        /// </summary>
        /// <param name="logger">The logger passed as dependency.</param>
        public ApiRootController(ILogger<SortingController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Gets all jobs.
        /// </summary>
        /// <returns>A collection of <see cref="SortingJob"/>.</returns>
        [HttpGet]
        public ApiInfo Get()
        {
            return new ApiInfoFactory().Create();
        }
    }
}
