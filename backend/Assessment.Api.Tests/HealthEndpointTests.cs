using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Assessment.Api.Tests;

// Infrastructure smoke coverage only. Assessment business tests are still to be written.
public class HealthEndpointTests
{
    [Fact]
    public async Task Health_endpoint_returns_healthy_from_the_real_api_pipeline()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        using var response = await client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }
}
