namespace Assessment.Api.Dto;

public sealed class OrderResponse
{
    public Guid Id { get; set; }

    public required string ExternalReference { get; set; }

    public Guid CustomerId { get; set; }

    public required CustomerResponse Customer { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public OrderStatus Status { get; set; }

    public Currency Currency { get; set; }

    public List<OrderLineResponse> Lines { get; set; } = new();

    public decimal SubTotal { get; set; }

    public decimal Tax { get; set; }

    public decimal Total { get; set; }
}
