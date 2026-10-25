namespace Assessment.Api.Dto;

public sealed class ChangeOrderStatusRequest
{
    public Guid OrderId { get; set; }

    public OrderStatus NewStatus { get; set; }
}
