using BackendFIFOAllocation.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackendFIFOAllocation.Controllers;

[ApiController]
[Route("[controller]")]
public class ClearController : ControllerBase
{
    [HttpPost]
    public IActionResult Clear()
    {
        InMemoryData.ClearAll();
        return NoContent();
    }
}
