namespace Assessment.Api.Dto;

// Assessment excerpt: “Line totals and overall totals are calculated on the server.”
public sealed class OrderLineResponse
{
    public string SKU { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int Quantity;

    public decimal UnitPrice;

    public decimal LineTotal { get; set; }
}
