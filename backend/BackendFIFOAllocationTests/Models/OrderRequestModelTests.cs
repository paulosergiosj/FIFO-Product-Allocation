using BackendFIFOAllocation.Models;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace BackendFIFOAllocationTests.Models;

public class OrderRequestModelTests
{
    [Fact]
    public void Validate_ShouldReturnError_WhenOrderIdIsEmpty()
    {
        var model = new OrderRequestModel
        {
            OrderId = "",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 10, UnitPrice = 5.00m }]
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "OrderId is required.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenOrderIdIsWhitespace()
    {
        var model = new OrderRequestModel
        {
            OrderId = "   ",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 10, UnitPrice = 5.00m }]
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "OrderId is required.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenOrderIdExceeds50Characters()
    {
        var model = new OrderRequestModel
        {
            OrderId = new string('O', 51),
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 10, UnitPrice = 5.00m }]
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "OrderId cannot exceed 50 characters.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenLinesIsNull()
    {
        var model = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = null!
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "At least one order line is required.");
    }

    [Fact]
    public void Validate_ShouldReturnError_WhenLinesIsEmpty()
    {
        var model = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = []
        };

        var results = ValidateModel(model);

        results.Should().Contain(r => r.ErrorMessage == "At least one order line is required.");
    }

    [Fact]
    public void Validate_ShouldPass_WhenAllFieldsAreValid()
    {
        var model = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 10, UnitPrice = 5.00m }]
        };

        var results = ValidateModel(model);

        results.Should().BeEmpty();
    }

    [Fact]
    public void Validate_ShouldPass_WhenOrderIdIsExactly50Characters()
    {
        var model = new OrderRequestModel
        {
            OrderId = new string('O', 50),
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 10, UnitPrice = 5.00m }]
        };

        var results = ValidateModel(model);

        results.Should().NotContain(r => r.ErrorMessage == "OrderId cannot exceed 50 characters.");
    }

    private static IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }
}


