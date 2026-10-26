using System.Net;
using System.Net.Http.Json;
using Assessment.Api.Dto;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Assessment.Api.Tests;

public class CustomerEndpointTests
{
    [Fact]
    public async Task ListsSeededCustomers()
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var request = new CreateOrderRequest
        {
            ExternalReference = "CUSTOMER-LIST-TEST",
            CustomerId = Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"),
            Currency = Currency.ZAR,
            Lines = new List<CreateOrderLineRequest>
            {
                new() { SKU = "TEST-SKU", Name = "Test item", Quantity = 1, UnitPrice = 10m }
            }
        };
        using var createResponse = await client.PostAsJsonAsync("/api/orders", request);
        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);

        using var response = await client.GetAsync("/api/customers");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var customers = await response.Content.ReadFromJsonAsync<CustomerResponse[]>();
        Assert.NotNull(customers);
        Assert.Collection(customers.OrderBy(customer => customer.Id),
            customer =>
            {
                Assert.Equal(Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"), customer.Id);
                Assert.Equal("Test Customer One", customer.Name);
                Assert.Equal("customer.one@example.invalid", customer.Email);
            },
            customer =>
            {
                Assert.Equal(Guid.Parse("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"), customer.Id);
                Assert.Equal("Test Customer Two", customer.Name);
                Assert.Equal("customer.two@example.invalid", customer.Email);
            });
    }

    [Theory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("ffffffff-ffff-4fff-8fff-ffffffffffff")]
    public async Task RejectsUnknownCustomer(string customerId)
    {
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var request = new CreateOrderRequest
        {
            ExternalReference = "UNKNOWN-CUSTOMER-TEST",
            CustomerId = Guid.Parse(customerId),
            Currency = Currency.ZAR,
            Lines = new List<CreateOrderLineRequest>
            {
                new() { SKU = "TEST-SKU", Name = "Test item", Quantity = 1, UnitPrice = 10m }
            }
        };

        using var response = await client.PostAsJsonAsync("/api/orders", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("Choose an existing customer.", await response.Content.ReadAsStringAsync());
    }
}
