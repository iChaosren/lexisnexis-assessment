using Assessment.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Controllers;

[Route("api/orders")]
public class OrderController(IOrderDb orderDb) : Controller
{
    private readonly IOrderDb orderDb = orderDb;

    // Assessment excerpt: “Accepts new orders and handles repeat submissions of the same client reference without creating duplicates.”
    // Assessment excerpt: “Computes totals on the server.”
    [HttpPost]
    public IActionResult CreateOrder([FromBody] CreateOrderRequest request)
    {
        return Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Create order is not implemented.",
            detail: "TODO: Implement order creation in OrdersEndpoints.CreateOrder.");
    }

    // Assessment excerpt: “Supports retrieving a single order and listing orders with newest first.”
    [HttpGet]
    public IActionResult ListOrders()
    {
        return Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "List orders is not implemented.",
            detail: "TODO: Implement order listing in OrdersEndpoints.ListOrders.");
    }

    // Assessment excerpt: “Supports retrieving a single order and listing orders with newest first.”
    [HttpGet("{id:guid}")]
    public IActionResult GetOrder(Guid id)
    {
        return Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Get order is not implemented.",
            detail: "TODO: Implement single-order retrieval in OrdersEndpoints.GetOrder.");
    }

    // Assessment excerpt: “Supports changing an order’s status with sensible rules and helpful feedback when a change isn’t allowed.”
    [HttpPatch("{id:guid}/status")]
    public IActionResult ChangeOrderStatus(Guid id, [FromBody] ChangeOrderStatusRequest request)
    {
        return Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "Change order status is not implemented.",
            detail: "TODO: Implement status changes in OrdersEndpoints.ChangeOrderStatus.");
    }
}
