using System.ComponentModel.DataAnnotations;

namespace BackendFIFOAllocation.Models;

public class InventoryReceiptRequestModel : IValidatableObject
{
    public string Sku { get; set; } = "";
    public int QuantityReceived { get; set; }
    public decimal UnitCost { get; set; }
    public DateTime ReceivedAtUtc { get; set; }
    public string WarehouseId { get; set; } = "";

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Sku))
            yield return new ValidationResult("SKU is required.", [nameof(Sku)]);

        if (Sku.Length > 50)
            yield return new ValidationResult("SKU cannot exceed 50 characters.", [nameof(Sku)]);

        if (string.IsNullOrWhiteSpace(WarehouseId))
            yield return new ValidationResult("WarehouseId is required.", [nameof(WarehouseId)]);

        if (WarehouseId.Length > 50)
            yield return new ValidationResult("WarehouseId cannot exceed 50 characters.", [nameof(WarehouseId)]);

        if (QuantityReceived <= 0)
            yield return new ValidationResult("QuantityReceived must be greater than zero.", [nameof(QuantityReceived)]);

        if (UnitCost <= 0)
            yield return new ValidationResult("UnitCost must be greater than zero.", [nameof(UnitCost)]);

        if (ReceivedAtUtc > DateTime.UtcNow)
            yield return new ValidationResult("ReceivedAtUtc cannot be a future date.", [nameof(ReceivedAtUtc)]);
    }
}
