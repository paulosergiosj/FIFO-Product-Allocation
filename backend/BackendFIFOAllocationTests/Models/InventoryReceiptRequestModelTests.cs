using BackendFIFOAllocation.Models;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace BackendFIFOAllocationTests.Models;

public class InventoryReceiptRequestModelTests
{
    [Fact]
    public void Validate_ShouldReturnError_WhenSkuIsEmpty()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = "",
            QuantityReceived = 10,
            UnitCost = 5.00m,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = "WH1"
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "SKU is required." && r.MemberNames.Contains(nameof(InventoryReceiptRequestModel.Sku)));
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenSkuIsWhitespace()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = "   ",
            QuantityReceived = 10,
            UnitCost = 5.00m,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = "WH1"
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "SKU is required.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenSkuExceeds50Characters()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = new string('A', 51),
            QuantityReceived = 10,
            UnitCost = 5.00m,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = "WH1"
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "SKU cannot exceed 50 characters.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenWarehouseIdIsEmpty()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = "SKU001",
            QuantityReceived = 10,
            UnitCost = 5.00m,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = ""
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "WarehouseId is required.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenWarehouseIdExceeds50Characters()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = "SKU001",
            QuantityReceived = 10,
            UnitCost = 5.00m,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = new string('W', 51)
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "WarehouseId cannot exceed 50 characters.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenQuantityReceivedIsZero()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = "SKU001",
            QuantityReceived = 0,
            UnitCost = 5.00m,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = "WH1"
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "QuantityReceived must be greater than zero.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenQuantityReceivedIsNegative()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = "SKU001",
            QuantityReceived = -1,
            UnitCost = 5.00m,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = "WH1"
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "QuantityReceived must be greater than zero.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenUnitCostIsZero()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = "SKU001",
            QuantityReceived = 10,
            UnitCost = 0,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = "WH1"
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "UnitCost must be greater than zero.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenUnitCostIsNegative()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = "SKU001",
            QuantityReceived = 10,
            UnitCost = -1,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = "WH1"
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "UnitCost must be greater than zero.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenReceivedAtUtcIsFutureDate()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = "SKU001",
            QuantityReceived = 10,
            UnitCost = 5.00m,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(1),
            WarehouseId = "WH1"
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "ReceivedAtUtc cannot be a future date.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = "SKU001",
            QuantityReceived = 10,
            UnitCost = 5.00m,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = "WH1"
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldPass_WhenSkuIsExactly50Characters()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = new string('A', 50),
            QuantityReceived = 10,
            UnitCost = 5.00m,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = "WH1"
        };

        var results = ValidateModel(model);

        results.Should().NotContain(r => r.ErrorMessage == "SKU cannot exceed 50 characters.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenWarehouseIdIsExactly50Characters()
    {
        var model = new InventoryReceiptRequestModel
        {
            Sku = "SKU001",
            QuantityReceived = 10,
            UnitCost = 5.00m,
            ReceivedAtUtc = DateTime.UtcNow.AddDays(-1),
            WarehouseId = new string('W', 50)
        };

        var results = ValidateModel(model);

        results.Should().NotContain(r => r.ErrorMessage == "WarehouseId cannot exceed 50 characters.");
    }

    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }
}


