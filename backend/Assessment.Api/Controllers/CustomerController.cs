using Assessment.Api.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Assessment.Api.Controllers;

[Route("api/customers")]
public class CustomerController(IOrderDb orderDb) : Controller
{
    [HttpGet]
    public ActionResult<CustomerResponse[]> ListCustomers()
        => Ok(orderDb.ListCustomers());
}
