namespace Assessment.Api.Dto;

public sealed class OrderSummaryResponse
{
    public Guid Id { get; set; }

    public required string ExternalReference { get; set; }

    public DateTime CreatedAt { get; set; }

    public OrderStatus Status { get; set; }

    public decimal Total { get; set; }
}
