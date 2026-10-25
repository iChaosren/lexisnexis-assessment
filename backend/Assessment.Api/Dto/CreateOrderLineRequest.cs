using System.ComponentModel.DataAnnotations;

namespace Assessment.Api.Dto;

// Assessment excerpt: “Quantities are positive whole numbers; prices are non-negative.”
public sealed class CreateOrderLineRequest
{
    public required string SKU { get; set; }

    public required string Name { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Range(typeof(decimal), "0", DecimalExtensions.MaxValue)]
    public decimal UnitPrice { get; set; }
}
