using System;

using Xunit;

using Challenge.WebIntSorter.Controllers;

namespace WebIntSorter.Controllers.UnitTests
{
    public class ApiRootControllerUnitTest
    {
        [Fact]
        public void GetReturnsValidUrls()
        {
            var controller = new ApiRootController(
                Mocks.GetMockedLogger<SortingController>());

            var info = controller.Get();

            Assert.NotNull(info);
            Assert.NotNull(info.SortingUrl);
            Assert.NotEmpty(info.SortingUrl);
        }
    }
}
