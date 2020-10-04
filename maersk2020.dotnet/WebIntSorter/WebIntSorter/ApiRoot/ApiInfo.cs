using System;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// Describes an API info object which is used to give requestors information about te API itself.
    /// </summary>
    /// <remarks>
    /// This allows the API to be more compliant to RESTful principles.
    /// Principle 6: Uniform Interfaces. Subprinciple 4: HATEOAS.
    /// </remarks>
    public class ApiInfo
    {
        /// <summary>
        /// The endpoint to reach to executing sorting operations.
        /// </summary>
        public string SortingUrl { get; set; }
    }
}
