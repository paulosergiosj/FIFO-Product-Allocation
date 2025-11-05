namespace BackendFIFOAllocation.Models;

public record AllocationDetailModel(
    string Sku,
    int AllocatedQty,
    string WarehouseId,
    decimal UnitCost
);
