using Assessment.Api.Controllers;
using Assessment.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Tests;

// Direct action tests use a fresh real in-memory store; MVC behavior is tested separately.
public class OrderControllerTests
{
    private static readonly Guid PendingOrderId = Guid.Parse("11111111-1111-4111-8111-111111111111");
    private static readonly Guid ConfirmedOrderId = Guid.Parse("22222222-2222-4222-8222-222222222222");
    private static readonly Guid UnknownOrderId = Guid.Parse("ffffffff-ffff-4fff-8fff-ffffffffffff");

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void CreatesNewOrder(int unitPrice)
    {
        var controller = new OrderController(new OrderDb());
        var request = NewOrder(unitPrice: unitPrice);

        var result = controller.CreateOrder(request);

        Assert.Equal(1, OkValue(result));
    }

    [Fact]
    public void SkipsRepeatedReference()
    {
        var controller = new OrderController(new OrderDb());
        var request = NewOrder();

        var first = controller.CreateOrder(request);
        var repeat = controller.CreateOrder(request);

        Assert.Equal(1, OkValue(first));
        Assert.Equal(0, OkValue(repeat));
    }

    [Fact]
    public void SkipsExistingReference()
    {
        var controller = new OrderController(new OrderDb());
        var request = NewOrder(reference: "TEST-ORDER-001");

        var result = controller.CreateOrder(request);

        Assert.Equal(0, OkValue(result));
    }

    [Fact]
    public void ListsNewestFirst()
    {
        var controller = new OrderController(new OrderDb());

        var result = controller.ListOrders();

        Assert.Equal(
            new[] { ConfirmedOrderId, PendingOrderId },
            OkValue(result).Select(order => order.Id));
    }

    [Fact]
    public void ReturnsOrderDetails()
    {
        var controller = new OrderController(new OrderDb());

        var result = controller.GetOrder(PendingOrderId);

        var order = OkValue(result);
        Assert.Equal(PendingOrderId, order.Id);
        Assert.Equal("TEST-ORDER-001", order.ExternalReference);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal(Currency.ZAR, order.Currency);
        Assert.Equal(order.CustomerId, order.Customer.Id);
        Assert.Equal("Test Customer One", order.Customer.Name);
        Assert.Equal("customer.one@example.invalid", order.Customer.Email);
        var line = Assert.Single(order.Lines);
        Assert.Equal("TEST-SKU-001", line.SKU);
        Assert.Equal("Test item one", line.Name);
        Assert.Equal(2, line.Quantity);
        Assert.Equal(10m, line.UnitPrice);
        Assert.Equal(20m, line.LineTotal);
        Assert.Equal(20m, order.SubTotal);
        Assert.Equal(3m, order.Tax);
        Assert.Equal(23m, order.Total);
    }

    [Fact]
    public void ReturnsNullForMissingOrder()
    {
        var controller = new OrderController(new OrderDb());

        var result = controller.GetOrder(UnknownOrderId);

        Assert.Null(Assert.IsType<OkObjectResult>(result.Result).Value);
    }

    [Fact]
    public void ReturnsCreatedOrder()
    {
        var controller = new OrderController(new OrderDb());
        var request = NewOrder();
        Assert.Equal(1, OkValue(controller.CreateOrder(request)));

        var result = controller.GetOrder(Guid.NewGuid(request.ExternalReference));

        var order = OkValue(result);
        Assert.Equal(request.ExternalReference, order.ExternalReference);
        Assert.Equal(20m, Assert.Single(order.Lines).LineTotal);
        Assert.Equal(20m, order.SubTotal);
        Assert.Equal(3m, order.Tax);
        Assert.Equal(23m, order.Total);
    }

    [Fact]
    public void ListsRepeatedOrderOnce()
    {
        var controller = new OrderController(new OrderDb());
        var request = NewOrder();
        Assert.Equal(1, OkValue(controller.CreateOrder(request)));
        Assert.Equal(0, OkValue(controller.CreateOrder(request)));

        var result = controller.ListOrders();

        Assert.Single(OkValue(result), order => order.ExternalReference == request.ExternalReference);
    }

    [Fact]
    public void UpdatesStatus()
    {
        var controller = new OrderController(new OrderDb());
        var before = OkValue(controller.GetOrder(PendingOrderId));
        var request = StatusChange(PendingOrderId, OrderStatus.Confirmed);
        var earliestUpdate = DateTime.UtcNow;

        var result = controller.ChangeOrderStatus(PendingOrderId, request);
        var latestUpdate = DateTime.UtcNow;

        var after = OkValue(controller.GetOrder(PendingOrderId));
        Assert.Equal(1, OkValue(result));
        Assert.Equal(OrderStatus.Confirmed, after.Status);
        Assert.Equal(before.CreatedAt, after.CreatedAt);
        Assert.InRange(after.UpdatedAt, earliestUpdate, latestUpdate);
    }

    [Fact]
    public void SkipsUnchangedStatus()
    {
        var controller = new OrderController(new OrderDb());
        var before = OkValue(controller.GetOrder(PendingOrderId));
        var request = StatusChange(PendingOrderId, OrderStatus.Pending);

        var result = controller.ChangeOrderStatus(PendingOrderId, request);

        var after = OkValue(controller.GetOrder(PendingOrderId));
        Assert.Equal(0, OkValue(result));
        Assert.Equal(before.Status, after.Status);
        Assert.Equal(before.UpdatedAt, after.UpdatedAt);
    }

    [Fact]
    public void ReturnsZeroForMissingOrder()
    {
        var controller = new OrderController(new OrderDb());
        var request = StatusChange(UnknownOrderId, OrderStatus.Confirmed);

        var result = controller.ChangeOrderStatus(UnknownOrderId, request);

        Assert.Equal(0, OkValue(result));
    }

    [Fact]
    public void RejectsReturnToPending()
    {
        var controller = new OrderController(new OrderDb());
        var request = StatusChange(ConfirmedOrderId, OrderStatus.Pending);

        Assert.Throws<ArgumentException>(() => controller.ChangeOrderStatus(ConfirmedOrderId, request));

        Assert.Equal(OrderStatus.Confirmed, OkValue(controller.GetOrder(ConfirmedOrderId)).Status);
    }

    [Theory]
    [InlineData(OrderStatus.Fulfilled)]
    [InlineData(OrderStatus.Cancelled)]
    public void RejectsTerminalChanges(OrderStatus terminalStatus)
    {
        var controller = new OrderController(new OrderDb());
        Assert.Equal(1, OkValue(controller.ChangeOrderStatus(
            ConfirmedOrderId, StatusChange(ConfirmedOrderId, terminalStatus))));
        var request = StatusChange(ConfirmedOrderId, OrderStatus.Confirmed);

        Assert.Throws<ArgumentException>(() => controller.ChangeOrderStatus(ConfirmedOrderId, request));

        Assert.Equal(terminalStatus, OkValue(controller.GetOrder(ConfirmedOrderId)).Status);
    }

    private static T OkValue<T>(ActionResult<T> result)
        => Assert.IsType<T>(Assert.IsType<OkObjectResult>(result.Result).Value);

    private static ChangeOrderStatusRequest StatusChange(Guid orderId, OrderStatus status)
        => new() { OrderId = orderId, NewStatus = status };

    private static CreateOrderRequest NewOrder(string reference = "UNIT-ORDER-001", decimal unitPrice = 10m)
        => new()
        {
            ExternalReference = reference,
            Currency = Currency.ZAR,
            Lines = new List<CreateOrderLineRequest>
            {
                new() { SKU = "UNIT-SKU", Name = "Test item", Quantity = 2, UnitPrice = unitPrice }
            }
        };
}
