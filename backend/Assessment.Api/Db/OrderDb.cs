
using System.Collections.Concurrent;
using Assessment.Api.Dto;

public sealed class OrderDb : IOrderDb
{
    private ConcurrentDictionary<Guid, Order> orderList = new()
    {
        [Guid.Parse("11111111-1111-4111-8111-111111111111")] = new Order()
        {
            Id = Guid.Parse("11111111-1111-4111-8111-111111111111"),
            ExternalReference = "TEST-ORDER-001",
            CustomerId = Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"),
            Customer = new Customer()
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-4aaa-8aaa-aaaaaaaaaaaa"),
                Name = "Test Customer One",
                Email = "customer.one@example.invalid"
            },
            CreatedAt = new DateTime(2026, 10, 25, 12, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 10, 25, 12, 0, 0, DateTimeKind.Utc),
            Status = OrderStatus.Pending,
            Currency = Currency.ZAR,
            Lines = new List<LineItem>()
            {
                new LineItem(new CreateOrderLineRequest()
                {
                    SKU = "TEST-SKU-001",
                    Name = "Test item one",
                    Quantity = 2,
                    UnitPrice = 10m
                })
            }
        },
        [Guid.Parse("22222222-2222-4222-8222-222222222222")] = new Order()
        {
            Id = Guid.Parse("22222222-2222-4222-8222-222222222222"),
            ExternalReference = "TEST-ORDER-002",
            CustomerId = Guid.Parse("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"),
            Customer = new Customer()
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-4bbb-8bbb-bbbbbbbbbbbb"),
                Name = "Test Customer Two",
                Email = "customer.two@example.invalid"
            },
            CreatedAt = new DateTime(2026, 10, 25, 13, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2026, 10, 25, 13, 0, 0, DateTimeKind.Utc),
            Status = OrderStatus.Confirmed,
            Currency = Currency.USD,
            Lines = new List<LineItem>()
            {
                new LineItem(new CreateOrderLineRequest()
                {
                    SKU = "TEST-SKU-002",
                    Name = "Test item two",
                    Quantity = 1,
                    UnitPrice = 25m
                })
            }
        }
    };
    
    public int Create(CreateOrderRequest dto)
        => orderList.TryAdd(Guid.NewGuid(dto.ExternalReference), new Order(dto)) ? 1 : 0;

    public OrderResponse[] ListOrders()
        => orderList.Values.OrderByDescending(o => o.CreatedAt).Select(o => o.ToDto()).ToArray();
    
    public OrderResponse? GetOrder(Guid orderId)
        => orderList.TryGetValue(orderId, out Order? value) ? value.ToDto() : null;
    
    public int ChangeOrderStatus(ChangeOrderStatusRequest dto)
    {
        lock (orderList)
        {
            if (!orderList.TryGetValue(dto.OrderId, out Order? order))
                return 0;

            if (order.Status == dto.NewStatus)
                return 0;
            
            if (!orderList.TryUpdate(order.Id, order.UpdateStatus(dto), order))
                return 0;

            return 1;
        }
    }
}
