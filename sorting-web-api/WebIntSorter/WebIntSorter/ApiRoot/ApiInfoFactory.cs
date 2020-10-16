using System;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// Factory to create an instance of <see cref="ApiInfo"/> class.
    /// </summary>
    /// <remarks>
    /// As the API grows, new versions will be introduced.
    /// This factory will eventually take as input the version
    /// number and emit the proper <see cref="ApiInfo"/> object.
    /// </remarks>
    public class ApiInfoFactory
    {
        /// <summary>
        /// Creates an instance of the <see cref="ApiInfo"/> class.
        /// </summary>
        /// <returns>An <see cref="ApiInfo"/> object.</returns>
        public ApiInfo Create() => new ApiInfo()
        {
            SortingUrl = "/api/sorting"
        };
    }
}
