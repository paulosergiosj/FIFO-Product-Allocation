using System.ComponentModel.DataAnnotations;

namespace BackendFIFOAllocation.Models;

public class OrderLine : IValidatableObject
{
    public string Sku { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? PreferredWarehouseId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Sku))
            yield return new ValidationResult("SKU is required.", [nameof(Sku)]);

        if (Quantity <= 0)
            yield return new ValidationResult("Quantity must be greater than zero.", [nameof(Quantity)]);

        if (UnitPrice < 0)
            yield return new ValidationResult("UnitPrice cannot be negative.", [nameof(UnitPrice)]);

        if (PreferredWarehouseId != null && string.IsNullOrWhiteSpace(PreferredWarehouseId))
            yield return new ValidationResult("PreferredWarehouseId cannot be empty if provided.", [nameof(PreferredWarehouseId)]);
    }
}
