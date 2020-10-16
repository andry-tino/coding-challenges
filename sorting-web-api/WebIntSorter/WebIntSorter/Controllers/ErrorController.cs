using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

namespace Challenge.WebIntSorter.Controllers
{
    /// <summary>
    /// API controller responsible for handling errors.
    /// </summary>
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public IActionResult Get()
        {
            return Problem("A problem occurred and the operation cannot be completed");
        }
    }
}
