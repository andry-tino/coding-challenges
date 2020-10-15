using System;

namespace Challenge.WebIntSorter
{
    /// <summary>
    /// Constants available in the assembly.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// Constants specific to the service configuration.
        /// </summary>
        public static class Service
        {
            /// <summary>
            /// The name of the policy to enable CORS for the React client.
            /// </summary>
            public const string CorsReactClientAllowSpecificOriginsPolicyName = "ReactClientAllowSpecificOrigins";
        }
    }
}
