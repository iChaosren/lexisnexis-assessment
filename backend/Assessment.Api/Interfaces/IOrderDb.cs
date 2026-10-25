using Assessment.Api.Dto;

public interface IOrderDb
{
    public int Create(CreateOrderRequest dto);

    public OrderResponse[] ListOrders();

    public OrderResponse? GetOrder(Guid orderId);

    public int ChangeOrderStatus(ChangeOrderStatusRequest dto);
}
