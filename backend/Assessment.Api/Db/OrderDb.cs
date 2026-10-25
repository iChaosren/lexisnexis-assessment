
using System.Collections.Concurrent;
using Assessment.Api;

public sealed class OrderDb : IOrderDb
{
    private ConcurrentDictionary<Guid, Order> orderList = new()
    {
        // Add testing data here
    };
    
    public int Create(CreateOrderRequest dto)
        => orderList.TryAdd(Guid.NewGuid(dto.ClientReference), new Order(dto)) ? 1 : 0;

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
