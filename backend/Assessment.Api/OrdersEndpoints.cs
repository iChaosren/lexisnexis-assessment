namespace Assessment.Api;

public static class OrdersEndpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var orders = endpoints.MapGroup("/api/orders");

        orders.MapPost("", CreateOrder);
        orders.MapGet("", ListOrders);
        orders.MapGet("/{id:guid}", GetOrder);
        orders.MapPatch("/{id:guid}/status", ChangeOrderStatus);

        return endpoints;
    }

    // Assessment excerpt: “Accepts new orders and handles repeat submissions of the same client reference without creating duplicates.”
    // Assessment excerpt: “Computes totals on the server.”
    private static IResult CreateOrder(CreateOrderRequest request)
    {
        // TODO: Design and implement order creation, repeat-submission handling, and server totals.
        return Results.Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Create order is not implemented.",
            detail: "TODO: Implement order creation in OrdersEndpoints.CreateOrder.");
    }

    // Assessment excerpt: “Supports retrieving a single order and listing orders with newest first.”
    private static IResult ListOrders()
    {
        // TODO: Implement retrieval and choose how to return OrderSummaryResponse values newest first.
        return Results.Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "List orders is not implemented.",
            detail: "TODO: Implement order listing in OrdersEndpoints.ListOrders.");
    }

    // Assessment excerpt: “Supports retrieving a single order and listing orders with newest first.”
    private static IResult GetOrder(Guid id)
    {
        // TODO: Implement lookup and decide the response for an unknown order ID.
        return Results.Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Get order is not implemented.",
            detail: "TODO: Implement single-order retrieval in OrdersEndpoints.GetOrder.");
    }

    // Assessment excerpt: “Supports changing an order’s status with sensible rules and helpful feedback when a change isn’t allowed.”
    private static IResult ChangeOrderStatus(Guid id, ChangeOrderStatusRequest request)
    {
        // TODO: Design and implement the allowed status changes and feedback.
        return Results.Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Change order status is not implemented.",
            detail: "TODO: Implement status changes in OrdersEndpoints.ChangeOrderStatus.");
    }
}
