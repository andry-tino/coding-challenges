using System;

using Microsoft.Extensions.Logging;
using Moq;

namespace WebIntSorter.Controllers.UnitTests
{
    /// <summary>
    /// Provides the cmost common mocks required in tests.
    /// </summary>
    internal static class Mocks
    {
        /// <summary>
        /// Gets a mocked <see cref="ILogger"/>.
        /// </summary>
        /// <typeparam name="T">The controller the logger is used against.</typeparam>
        /// <returns>A <see cref="ILogger"/>.</returns>
        public static ILogger<T> GetMockedLogger<T>()
        {
            var mock = new Mock<ILogger<T>>();
            mock.Setup(logger => logger.Log<T>(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(),
                    It.IsAny<T>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<T, Exception, string>>()))
                .Verifiable();

            return mock.Object;
        }
    }
}
