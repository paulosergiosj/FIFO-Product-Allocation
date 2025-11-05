using BackendFIFOAllocation.Models;
using FluentAssertions;

namespace BackendFIFOAllocationTests.Models;

public class InventoryReceiptTests
{
    [Fact]
    public void QuantityAvailable_ShouldCalculate_Correctly()
    {
        var receipt = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");

        receipt.QuantityAvailable.Should().Be(10);
    }

    [Fact]
    public void QuantityAvailable_ShouldDecrease_WhenQuantityUsedIncreases()
    {
        var receipt = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");

        receipt.QuantityUsed = 3;
        receipt.QuantityAvailable.Should().Be(7);

        receipt.QuantityUsed = 5;
        receipt.QuantityAvailable.Should().Be(5);
    }

    [Fact]
    public void QuantityAvailable_ShouldBeZero_WhenFullyUsed()
    {
        var receipt = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");

        receipt.QuantityUsed = 10;
        receipt.QuantityAvailable.Should().Be(0);
    }

    [Fact]
    public void Constructor_ShouldSetQuantityUsed_ToZero()
    {
        var receipt = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");

        receipt.QuantityUsed.Should().Be(0);
    }

    [Fact]
    public void Constructor_ShouldSetAllProperties_Correctly()
    {
        var date = DateTime.UtcNow.AddDays(-1);
        var receipt = new InventoryReceipt("SKU001", 10, 5.00m, date, "WH1");

        receipt.Sku.Should().Be("SKU001");
        receipt.QuantityReceived.Should().Be(10);
        receipt.UnitCost.Should().Be(5.00m);
        receipt.ReceivedAtUtc.Should().Be(date);
        receipt.WarehouseId.Should().Be("WH1");
        receipt.QuantityUsed.Should().Be(0);
    }
}


