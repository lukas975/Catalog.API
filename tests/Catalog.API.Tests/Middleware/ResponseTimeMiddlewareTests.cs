using Catalog.Fixtures;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Catalog.API.Tests.Middleware
{
    public class ResponseTimeMiddlewareTests : IClassFixture<InMemoryApplicationFactory<Startup>>
    {
        public ResponseTimeMiddlewareTests(InMemoryApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        private readonly InMemoryApplicationFactory<Startup> _factory;
        
        [Theory]
        [InlineData("/api/items/?pageSize=1&pageIndex=0")]
        [InlineData("/api/artist/?pageSize=1&pageIndex=0")]
        [InlineData("/api/genre/?pageSize=1&pageIndex=0")]
        public async Task Middleware_should_set_the_correct_response_time_custom_header(string url)
        {
            var client = _factory.CreateClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            response.Headers.GetValues("X-Response-Timems").ShouldNotBeEmpty();
        }

    }
}
