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
    public void CreateOrder_returns_one_for_a_new_reference_including_a_zero_price(int unitPrice)
    {
        var controller = new OrderController(new OrderDb());
        var request = NewOrder(unitPrice: unitPrice);

        var result = controller.CreateOrder(request);

        Assert.Equal(1, OkValue(result));
    }

    [Fact]
    public void CreateOrder_returns_zero_for_a_repeated_reference()
    {
        var controller = new OrderController(new OrderDb());
        var request = NewOrder();

        var first = controller.CreateOrder(request);
        var repeat = controller.CreateOrder(request);

        Assert.Equal(1, OkValue(first));
        Assert.Equal(0, OkValue(repeat));
    }

    [Fact]
    public void CreateOrder_does_not_duplicate_an_existing_seed_reference()
    {
        var controller = new OrderController(new OrderDb());
        var request = NewOrder(reference: "TEST-ORDER-001");

        var result = controller.CreateOrder(request);

        Assert.Equal(0, OkValue(result));
    }

    [Fact]
    public void ListOrders_returns_orders_newest_first()
    {
        var controller = new OrderController(new OrderDb());

        var result = controller.ListOrders();

        Assert.Equal(
            new[] { ConfirmedOrderId, PendingOrderId },
            OkValue(result).Select(order => order.Id));
    }

    [Fact]
    public void GetOrder_returns_customer_lines_and_server_totals()
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
    public void GetOrder_returns_a_null_value_for_an_unknown_id()
    {
        var controller = new OrderController(new OrderDb());

        var result = controller.GetOrder(UnknownOrderId);

        Assert.Null(Assert.IsType<OkObjectResult>(result.Result).Value);
    }

    [Fact]
    public void GetOrder_can_read_a_newly_created_order_with_server_totals()
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
    public void ListOrders_contains_one_order_after_a_repeated_submission()
    {
        var controller = new OrderController(new OrderDb());
        var request = NewOrder();
        Assert.Equal(1, OkValue(controller.CreateOrder(request)));
        Assert.Equal(0, OkValue(controller.CreateOrder(request)));

        var result = controller.ListOrders();

        var created = Assert.Single(OkValue(result), order => order.ExternalReference == request.ExternalReference);
        Assert.Equal(request.ExternalReference, created.ExternalReference);
    }

    [Fact]
    public void ChangeOrderStatus_updates_status_and_update_time()
    {
        var controller = new OrderController(new OrderDb());
        var before = OkValue(controller.GetOrder(PendingOrderId));
        var request = StatusChange(PendingOrderId, OrderStatus.Confirmed);

        var result = controller.ChangeOrderStatus(PendingOrderId, request);

        var after = OkValue(controller.GetOrder(PendingOrderId));
        Assert.Equal(1, OkValue(result));
        Assert.Equal(OrderStatus.Confirmed, after.Status);
        Assert.Equal(before.CreatedAt, after.CreatedAt);
        Assert.True(after.UpdatedAt > before.UpdatedAt);
    }

    [Fact]
    public void ChangeOrderStatus_returns_zero_without_changing_an_identical_status()
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
    public void ChangeOrderStatus_returns_zero_for_an_unknown_order()
    {
        var controller = new OrderController(new OrderDb());
        var request = StatusChange(UnknownOrderId, OrderStatus.Confirmed);

        var result = controller.ChangeOrderStatus(UnknownOrderId, request);

        Assert.Equal(0, OkValue(result));
    }

    [Fact]
    public void ChangeOrderStatus_rejects_returning_a_confirmed_order_to_pending()
    {
        var controller = new OrderController(new OrderDb());
        var request = StatusChange(ConfirmedOrderId, OrderStatus.Pending);

        Assert.Throws<ArgumentException>(() => controller.ChangeOrderStatus(ConfirmedOrderId, request));

        Assert.Equal(OrderStatus.Confirmed, OkValue(controller.GetOrder(ConfirmedOrderId)).Status);
    }

    [Theory]
    [InlineData(OrderStatus.Fulfilled)]
    [InlineData(OrderStatus.Cancelled)]
    public void ChangeOrderStatus_rejects_changes_from_a_terminal_order(OrderStatus terminalStatus)
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
