using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

using WebIntSorterStartup = Challenge.WebIntSorter.Startup;

namespace WebIntSorter.IntegrationTests
{
    public class SortingTest : IClassFixture<WebApplicationFactory<WebIntSorterStartup>>
    {
        private readonly WebApplicationFactory<WebIntSorterStartup> factory;

        public SortingTest(WebApplicationFactory<WebIntSorterStartup> factory)
        {
            this.factory = factory;
        }

        [Theory]
        [InlineData("/api/sorting", "GET")]
        [InlineData("/api/sorting/00000000-0000-0000-0000-000000000000", "GET")]
        [InlineData("/api/sorting", "POST")]
        public async Task CheckHeaders(string url, string method)
        {
            var client = this.factory.CreateClient();
            var content = new StringContent("[1,4]", UTF8Encoding.UTF8, "application/json");

            var response = await (HttpMethods.IsPost(method)
                ? client.PostAsync(url, content)
                : client.GetAsync(url));

            Assert.NotNull(response);
            response.EnsureSuccessStatusCode();

            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                Assert.Equal("application/json; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }

        [Fact]
        public async Task WhenNonExistingJobRequestedThenEmptyContentIsReturned()
        {
            var client = this.factory.CreateClient();

            var response = await client.GetAsync("/api/sorting/00000000-0000-0000-0000-000000000000");

            Assert.NotNull(response);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.NotNull(content);
            Assert.Empty(content);
        }

        [Fact]
        public async Task WhenRequestingAllJobsAndNoJobsThenEmptyArrayIsReturned()
        {
            var client = this.factory.CreateClient();

            var response = await client.GetAsync("/api/sorting");

            Assert.NotNull(response);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            Assert.NotNull(content);
            Assert.NotEmpty(content);
            Assert.Equal("[]", content);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("invalid")]
        public async Task WhenPostWithInvalidContentThenBadRequestIsReturned(string data)
        {
            var client = this.factory.CreateClient();
            var content = new StringContent(data, UTF8Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/sorting", content);

            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
