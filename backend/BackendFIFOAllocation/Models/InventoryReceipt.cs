namespace BackendFIFOAllocation.Models;

public class InventoryReceipt(string sku, int quantityReceived, decimal unitCost, DateTime receivedAtUtc, string warehouseId)
{
    public string Sku { get; set; } = sku;
    public int QuantityReceived { get; set; } = quantityReceived;
    public int QuantityUsed { get; set; } = 0;
    public decimal UnitCost { get; set; } = unitCost;
    public DateTime ReceivedAtUtc { get; set; } = receivedAtUtc;
    public string WarehouseId { get; set; } = warehouseId;

    public int QuantityAvailable => QuantityReceived - QuantityUsed;
}
