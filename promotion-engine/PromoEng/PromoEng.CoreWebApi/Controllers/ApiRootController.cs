using System;

using Microsoft.AspNetCore.Mvc;

namespace PromoEng.CoreWebApi.Controllers
{
    /// <summary>
    /// Controller responsible for handling requests to the api root endpoint.
    /// </summary>
    [ApiController]
    [Route("api")]
    public class ApiRootController : ControllerBase
    {
        /// <summary>
        /// Redirects to the swagger to provide info on the API.
        /// </summary>
        /// <returns>The redirect result.</returns>
        [HttpGet()]
        public IActionResult Get()
        {
            return RedirectPermanent(Constants.Routing.SwaggerUrl);
        }
    }
}
