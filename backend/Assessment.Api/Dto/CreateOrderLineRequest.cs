namespace Assessment.Api.Dto;

// Assessment excerpt: “Quantities are positive whole numbers; prices are non-negative.”
public sealed class CreateOrderLineRequest
{
    public string SKU { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Quantity;

    public decimal UnitPrice;
}
