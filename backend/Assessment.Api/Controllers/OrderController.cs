using Assessment.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController(IOrderDb orderDb) : Controller
{
    [HttpPost]
    public ActionResult<int> CreateOrder([FromBody] CreateOrderRequest dto)
    {
        if (!orderDb.ListCustomers().Any(customer => customer.Id == dto.CustomerId))
            return BadRequest("Choose an existing customer.");

        return Ok(orderDb.Create(dto));
    }

    [HttpGet]
    public ActionResult<OrderResponse[]> ListOrders()
        => Ok(orderDb.ListOrders());

    [HttpGet("{id:guid}")]
    public ActionResult<OrderResponse> GetOrder(Guid id)
        => Ok(orderDb.GetOrder(id));

    [HttpPatch("{id:guid}/status")]
    public ActionResult<int> ChangeOrderStatus(Guid id, [FromBody] ChangeOrderStatusRequest dto)
        => Ok(orderDb.ChangeOrderStatus(dto));
}
