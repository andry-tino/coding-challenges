using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

using WebIntSorterStartup = Challenge.WebIntSorter.Startup;

namespace WebIntSorter.IntegrationTests
{
    public class ApiInfoTest : IClassFixture<WebApplicationFactory<WebIntSorterStartup>>
    {
        private readonly WebApplicationFactory<WebIntSorterStartup> factory;

        public ApiInfoTest(WebApplicationFactory<WebIntSorterStartup> factory)
        {
            this.factory = factory;
        }

        [Fact]
        public async Task ApiIsCompliantToHATEOAS()
        {
            var client = this.factory.CreateClient();

            var response = await client.GetAsync("/api");

            Assert.NotNull(response);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.NotNull(content);
            Assert.NotEmpty(content);
        }
    }
}
