namespace Assessment.Api.Dto;

// Assessment excerpt: “You design the API, data model, validation, and error behavior.”
public sealed class CreateOrderRequest
{
    public required string ExternalReference { get; set; }

    public Currency Currency { get; set; }

    public List<CreateOrderLineRequest> Lines { get; set; } = new();
}
