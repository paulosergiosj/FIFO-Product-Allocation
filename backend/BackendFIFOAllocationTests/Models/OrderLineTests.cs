using BackendFIFOAllocation.Models;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace BackendFIFOAllocationTests.Models;

public class OrderLineTests
{
    [Fact]
    public void Validate_ShouldReturnError_WhenSkuIsEmpty()
    {
        var model = new OrderLine
        {
            Sku = "",
            Quantity = 10,
            UnitPrice = 5.00m
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "SKU is required.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenSkuIsWhitespace()
    {
        var model = new OrderLine
        {
            Sku = "   ",
            Quantity = 10,
            UnitPrice = 5.00m
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "SKU is required.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenQuantityIsZero()
    {
        var model = new OrderLine
        {
            Sku = "SKU001",
            Quantity = 0,
            UnitPrice = 5.00m
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "Quantity must be greater than zero.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenQuantityIsNegative()
    {
        var model = new OrderLine
        {
            Sku = "SKU001",
            Quantity = -1,
            UnitPrice = 5.00m
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "Quantity must be greater than zero.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenUnitPriceIsNegative()
    {
        var model = new OrderLine
        {
            Sku = "SKU001",
            Quantity = 10,
            UnitPrice = -1.00m
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "UnitPrice cannot be negative.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPreferredWarehouseIdIsEmptyString()
    {
        var model = new OrderLine
        {
            Sku = "SKU001",
            Quantity = 10,
            UnitPrice = 5.00m,
            PreferredWarehouseId = ""
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "PreferredWarehouseId cannot be empty if provided.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenPreferredWarehouseIdIsWhitespace()
    {
        var model = new OrderLine
        {
            Sku = "SKU001",
            Quantity = 10,
            UnitPrice = 5.00m,
            PreferredWarehouseId = "   "
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "PreferredWarehouseId cannot be empty if provided.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var model = new OrderLine
        {
            Sku = "SKU001",
            Quantity = 10,
            UnitPrice = 5.00m
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldPass_WhenUnitPriceIsZero()
    {
        var model = new OrderLine
        {
            Sku = "SKU001",
            Quantity = 10,
            UnitPrice = 0
        };

        var results = ValidateModel(model);

        results.Should().NotContain(r => r.ErrorMessage == "UnitPrice cannot be negative.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPreferredWarehouseIdIsNull()
    {
        var model = new OrderLine
        {
            Sku = "SKU001",
            Quantity = 10,
            UnitPrice = 5.00m,
            PreferredWarehouseId = null
        };

        var results = ValidateModel(model);

        results.Should().NotContain(r => r.ErrorMessage == "PreferredWarehouseId cannot be empty if provided.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenPreferredWarehouseIdIsValid()
    {
        var model = new OrderLine
        {
            Sku = "SKU001",
            Quantity = 10,
            UnitPrice = 5.00m,
            PreferredWarehouseId = "WH1"
        };

        var results = ValidateModel(model);

        results.Should().NotContain(r => r.ErrorMessage == "PreferredWarehouseId cannot be empty if provided.");
    }

    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }
}


