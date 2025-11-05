using System.ComponentModel.DataAnnotations;

namespace BackendFIFOAllocation.Models;

public class OrderRequestModel : IValidatableObject
{
    public string OrderId { get; set; } = "";
    public List<OrderLine> Lines { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(OrderId))
            yield return new ValidationResult("OrderId is required.", [nameof(OrderId)]);

        if (OrderId.Length > 50)
            yield return new ValidationResult("OrderId cannot exceed 50 characters.", [nameof(OrderId)]);

        if (Lines == null || Lines.Count == 0)
            yield return new ValidationResult("At least one order line is required.", [nameof(Lines)]);
    }
}
