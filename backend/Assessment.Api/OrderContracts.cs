using System.ComponentModel.DataAnnotations;

namespace Assessment.Api;

// Provisional scaffold contracts chosen with AI assistance. Revise these as you design the API.
// Assessment excerpt: “You design the API, data model, validation, and error behavior.”
public record CreateOrderRequest(
    string ClientReference,
    IReadOnlyList<CreateOrderLineRequest> Lines);

// Assessment excerpt: “Quantities are positive whole numbers; prices are non-negative.”
// TODO: Implement the validation yourself; these types do not enforce the business rules.
public record CreateOrderLineRequest(string Description, int Quantity, decimal UnitPrice);

public record ChangeOrderStatusRequest(Guid OrderId, OrderStatus NewStatus);

public record OrderSummaryResponse(
    Guid Id,
    string ClientReference,
    DateTimeOffset CreatedAtUtc,
    string Status,
    decimal Total);

public record OrderResponse(
    Guid Id,
    string ClientReference,
    DateTimeOffset CreatedAtUtc,
    string Status,
    decimal Total,
    IReadOnlyList<OrderLineResponse> Lines);

// Assessment excerpt: “Line totals and overall totals are calculated on the server.”
// TODO: Decide how to calculate and populate these response values on the backend.
public record OrderLineResponse(string Description, int Quantity, decimal UnitPrice, decimal LineTotal);
