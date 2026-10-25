using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Assessment.Api.Tests;

// These tests check HTTP wiring and explicit placeholder responses, not order business behavior.
// Assessment excerpt: “Include unit tests for the critical business rules you implement.”
// TODO: Add meaningful business-rule unit tests as you implement those rules.
public class OrderEndpointWiringTests
{
    [Theory]
    [InlineData(
        "POST", "/api/orders", "{\"clientReference\":\"scaffold-smoke\",\"lines\":[]}",
        "Create order is not implemented.",
        "TODO: Implement order creation in OrdersEndpoints.CreateOrder.")]
    [InlineData(
        "GET", "/api/orders", null,
        "List orders is not implemented.",
        "TODO: Implement order listing in OrdersEndpoints.ListOrders.")]
    [InlineData(
        "GET", "/api/orders/17e13259-c22d-4b19-8786-41b7cd05a0f1", null,
        "Get order is not implemented.",
        "TODO: Implement single-order retrieval in OrdersEndpoints.GetOrder.")]
    [InlineData(
        "PATCH", "/api/orders/17e13259-c22d-4b19-8786-41b7cd05a0f1/status", "{\"status\":\"example\"}",
        "Change order status is not implemented.",
        "TODO: Implement status changes in OrdersEndpoints.ChangeOrderStatus.")]
    public async Task Order_endpoint_returns_its_explicit_not_implemented_problem(
        string method, string path, string? body, string expectedTitle, string expectedDetail)
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        using var request = new HttpRequestMessage(new HttpMethod(method), path);

        if (body is not null)
        {
            request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        }

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        using var problem = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(501, problem.RootElement.GetProperty("status").GetInt32());
        Assert.Equal(expectedTitle, problem.RootElement.GetProperty("title").GetString());
        Assert.Equal(expectedDetail, problem.RootElement.GetProperty("detail").GetString());
    }
}
