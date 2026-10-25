namespace Assessment.Api.Dto;

public sealed class OrderSummaryResponse
{
    public Guid Id { get; set; }

    public string ExternalReference { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public OrderStatus Status { get; set; }

    public decimal Total { get; set; }
}
