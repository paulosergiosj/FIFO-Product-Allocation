using BackendFIFOAllocation.Data;
using BackendFIFOAllocation.Models;
using BackendFIFOAllocation.Services;
using Microsoft.AspNetCore.Mvc;

namespace BackendFIFOAllocation.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    [HttpPost]
    public IActionResult Allocate([FromBody] OrderRequestModel order)
        => Ok(AllocationService.AllocateOrder(order));

    [HttpGet]
    public IActionResult GetAll()
        => Ok(InMemoryData.Orders);
}
