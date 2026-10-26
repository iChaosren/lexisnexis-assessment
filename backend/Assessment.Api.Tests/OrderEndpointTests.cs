using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Assessment.Api.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Assessment.Api.Tests;

// HTTP integration checks for binding, validation and response formatting.
public class OrderEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        IncludeFields = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public static TheoryData<CreateOrderLineRequest, string> InvalidLines => new()
    {
        { Line(0, 10m), nameof(CreateOrderLineRequest.Quantity) },
        { Line(-1, 10m), nameof(CreateOrderLineRequest.Quantity) },
        { Line(1, -0.01m), nameof(CreateOrderLineRequest.UnitPrice) }
    };

    [Fact]
    public async Task ReturnsLineValues()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        using var response = await client.GetAsync($"/api/orders/{Guid.NewGuid("TEST-ORDER-001")}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var order = await response.Content.ReadFromJsonAsync<OrderResponse>(JsonOptions);
        Assert.NotNull(order);
        var line = Assert.Single(order.Lines);
        Assert.Equal(2, line.Quantity);
        Assert.Equal(10m, line.UnitPrice);
        Assert.Equal(20m, line.LineTotal);
    }

    [Fact]
    public async Task ReturnsNoContentForMissingOrder()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        using var response = await client.GetAsync("/api/orders/ffffffff-ffff-4fff-8fff-ffffffffffff");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Empty(await response.Content.ReadAsByteArrayAsync());
    }

    [Theory]
    [MemberData(nameof(InvalidLines))]
    public async Task RejectsInvalidLines(CreateOrderLineRequest line, string field)
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var request = new CreateOrderRequest
        {
            ExternalReference = "HTTP-VALIDATION",
            CustomerId = Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"),
            Currency = Currency.ZAR,
            Lines = new List<CreateOrderLineRequest> { line }
        };

        using var response = await client.PostAsJsonAsync("/api/orders", request, JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.Contains(problem.Errors, error => error.Key.EndsWith(field, StringComparison.Ordinal)
            && error.Value.Length > 0);
    }

    [Fact]
    public async Task RejectsFractionalQuantity()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        // The DTO's int Quantity cannot represent this malformed wire value.
        var request = new
        {
            ExternalReference = "HTTP-FRACTIONAL",
            CustomerId = Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"),
            Currency = Currency.ZAR,
            Lines = new[]
            {
                new { SKU = "TEST-SKU", Name = "Test item", Quantity = 1.5m, UnitPrice = 10m }
            }
        };

        using var response = await client.PostAsJsonAsync("/api/orders", request, JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Equal(400, problem.Status);
        Assert.NotEmpty(problem.Errors["$.lines[0].quantity"]);
    }

    [Fact]
    public async Task RejectsUnknownStatus()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var request = new ChangeOrderStatusRequest
        {
            OrderId = Guid.NewGuid("TEST-ORDER-001"),
            NewStatus = (OrderStatus)999
        };

        using var response = await client.PatchAsJsonAsync($"/api/orders/{request.OrderId}/status", request);

        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity });
    }

    [Fact]
    public async Task ReturnsToPending()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var request = new ChangeOrderStatusRequest
        {
            OrderId = Guid.NewGuid("TEST-ORDER-002"),
            NewStatus = OrderStatus.Pending
        };

        using var response = await client.PatchAsJsonAsync($"/api/orders/{request.OrderId}/status", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(1, await response.Content.ReadFromJsonAsync<int>());
        var order = await client.GetFromJsonAsync<OrderResponse>($"/api/orders/{request.OrderId}", JsonOptions);
        Assert.NotNull(order);
        Assert.Equal(OrderStatus.Pending, order.Status);
    }

    private static CreateOrderLineRequest Line(int quantity, decimal unitPrice)
        => new() { SKU = "TEST-SKU", Name = "Test item", Quantity = quantity, UnitPrice = unitPrice };
}
