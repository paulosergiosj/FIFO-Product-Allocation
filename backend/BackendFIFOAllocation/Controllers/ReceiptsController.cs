using BackendFIFOAllocation.Data;
using BackendFIFOAllocation.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendFIFOAllocation.Controllers;

[ApiController]
[Route("[controller]")]
public class ReceiptsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create([FromBody] InventoryReceiptRequestModel receiptRequestModel)
    {
        var receipt = new InventoryReceipt(
            receiptRequestModel.Sku, 
            receiptRequestModel.QuantityReceived, 
            receiptRequestModel.UnitCost, 
            receiptRequestModel.ReceivedAtUtc,
            receiptRequestModel.WarehouseId);
        InMemoryData.Receipts.Add(receipt);

        return CreatedAtAction(nameof(Create), receipt);
    }

    [HttpGet]
    public IActionResult GetAll()
        => Ok(InMemoryData.Receipts);
}
