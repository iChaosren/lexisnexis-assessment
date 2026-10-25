using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Assessment.Api.Dto;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Assessment.Api.Tests;

// HTTP integration checks for binding, validation and response formatting.
public class OrderEndpointTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        IncludeFields = true
    };

    public static TheoryData<CreateOrderLineRequest> InvalidLines => new()
    {
        Line(0, 10m),
        Line(-1, 10m),
        Line(1, -0.01m)
    };

    [Fact]
    public async Task ReturnsLineValues()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        using var response = await client.GetAsync("/api/orders/11111111-1111-4111-8111-111111111111");

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
    public async Task RejectsInvalidLines(CreateOrderLineRequest line)
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var request = new CreateOrderRequest
        {
            ExternalReference = "HTTP-VALIDATION",
            Currency = Currency.ZAR,
            Lines = new List<CreateOrderLineRequest> { line }
        };

        using var response = await client.PostAsJsonAsync("/api/orders", request, JsonOptions);

        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity });
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
            Currency = Currency.ZAR,
            Lines = new[]
            {
                new { SKU = "TEST-SKU", Name = "Test item", Quantity = 1.5m, UnitPrice = 10m }
            }
        };

        using var response = await client.PostAsJsonAsync("/api/orders", request, JsonOptions);

        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity });
    }

    [Fact]
    public async Task RejectsUnknownStatus()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var request = new ChangeOrderStatusRequest
        {
            OrderId = Guid.Parse("11111111-1111-4111-8111-111111111111"),
            NewStatus = (OrderStatus)999
        };

        using var response = await client.PatchAsJsonAsync($"/api/orders/{request.OrderId}/status", request);

        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.BadRequest, HttpStatusCode.UnprocessableEntity });
    }

    [Fact]
    public async Task RejectsInvalidStatusChange()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var request = new ChangeOrderStatusRequest
        {
            OrderId = Guid.Parse("22222222-2222-4222-8222-222222222222"),
            NewStatus = OrderStatus.Pending
        };

        using var response = await client.PatchAsJsonAsync($"/api/orders/{request.OrderId}/status", request);

        Assert.Contains(response.StatusCode, new[]
        {
            HttpStatusCode.BadRequest, HttpStatusCode.Conflict, HttpStatusCode.UnprocessableEntity
        });
        Assert.False(string.IsNullOrWhiteSpace(await response.Content.ReadAsStringAsync()),
            "A rejected status change should explain why it was rejected.");
    }

    private static CreateOrderLineRequest Line(int quantity, decimal unitPrice)
        => new() { SKU = "TEST-SKU", Name = "Test item", Quantity = quantity, UnitPrice = unitPrice };
}
