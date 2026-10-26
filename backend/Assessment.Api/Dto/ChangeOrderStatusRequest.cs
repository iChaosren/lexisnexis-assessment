using System.ComponentModel.DataAnnotations;

namespace Assessment.Api.Dto;

public sealed class ChangeOrderStatusRequest
{
    public Guid OrderId { get; set; }

    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus NewStatus { get; set; }
}
