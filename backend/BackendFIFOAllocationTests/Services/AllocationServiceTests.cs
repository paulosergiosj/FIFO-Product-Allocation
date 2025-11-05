using BackendFIFOAllocation.Data;
using BackendFIFOAllocation.Models;
using BackendFIFOAllocation.Services;
using FluentAssertions;

namespace BackendFIFOAllocationTests.Services;

public class AllocationServiceTests : IDisposable
{
    public AllocationServiceTests()
    {
        InMemoryData.ClearAll();
    }

    public void Dispose()
    {
        InMemoryData.ClearAll();
    }

    [Fact]
    public void AllocateOrder_ShouldAllocateFIFO_ByOldestReceiptDate()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-3), "WH1");
        var receipt2 = new InventoryReceipt("SKU001", 10, 6.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        var receipt3 = new InventoryReceipt("SKU001", 10, 7.00m, DateTime.UtcNow, "WH1");
        
        InMemoryData.Receipts.AddRange([receipt1, receipt2, receipt3]);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 15, UnitPrice = 10.00m }]
        };

        var result = AllocationService.AllocateOrder(order);

        result.OrderId.Should().Be("ORDER001");
        result.Details.Should().HaveCount(2);
        result.Details[0].WarehouseId.Should().Be("WH1");
        result.Details[0].AllocatedQty.Should().Be(10);
        result.Details[0].UnitCost.Should().Be(5.00m);
        result.Details[1].AllocatedQty.Should().Be(5);
        result.Details[1].UnitCost.Should().Be(6.00m);

        receipt1.QuantityUsed.Should().Be(10);
        receipt2.QuantityUsed.Should().Be(5);
        receipt3.QuantityUsed.Should().Be(0);
    }

    [Fact]
    public void AllocateOrder_ShouldUpdateQuantityUsedInPlace()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-2), "WH1");
        InMemoryData.Receipts.Add(receipt1);

        var order1 = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 5, UnitPrice = 10.00m }]
        };

        AllocationService.AllocateOrder(order1);

        var order2 = new OrderRequestModel
        {
            OrderId = "ORDER002",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 5, UnitPrice = 10.00m }]
        };

        var result = AllocationService.AllocateOrder(order2);

        receipt1.QuantityUsed.Should().Be(10);
        receipt1.QuantityAvailable.Should().Be(0);
        result.Details.Should().HaveCount(1);
        result.Details[0].AllocatedQty.Should().Be(5);
    }

    [Fact]
    public void AllocateOrder_ShouldAllocateFromPreferredWarehouseFirst()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-3), "WH1");
        var receipt2 = new InventoryReceipt("SKU001", 10, 6.00m, DateTime.UtcNow.AddDays(-1), "WH2");
        
        InMemoryData.Receipts.AddRange([receipt1, receipt2]);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine 
            { 
                Sku = "SKU001", 
                Quantity = 15, 
                UnitPrice = 10.00m,
                PreferredWarehouseId = "WH2"
            }]
        };

        var result = AllocationService.AllocateOrder(order);

        result.Details.Should().HaveCount(2);
        result.Details[0].WarehouseId.Should().Be("WH2");
        result.Details[0].AllocatedQty.Should().Be(10);
        result.Details[0].UnitCost.Should().Be(6.00m);
        result.Details[1].WarehouseId.Should().Be("WH1");
        result.Details[1].AllocatedQty.Should().Be(5);
        result.Details[1].UnitCost.Should().Be(5.00m);

        receipt2.QuantityUsed.Should().Be(10);
        receipt1.QuantityUsed.Should().Be(5);
    }

    [Fact]
    public void AllocateOrder_ShouldContinueFromOtherWarehouses_WhenPreferredWarehouseInsufficient()
    {
        InMemoryData.Orders = [];
        InMemoryData.Receipts = [];
        var receipt1 = new InventoryReceipt("SKU001", 5, 5.00m, DateTime.UtcNow.AddDays(-3), "WH1");
        var receipt2 = new InventoryReceipt("SKU001", 10, 6.00m, DateTime.UtcNow.AddDays(-1), "WH2");
        var receipt3 = new InventoryReceipt("SKU001", 5, 7.00m, DateTime.UtcNow.AddDays(-2), "WH3");
        
        InMemoryData.Receipts.AddRange([receipt1, receipt2, receipt3]);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine 
            { 
                Sku = "SKU001", 
                Quantity = 15, 
                UnitPrice = 10.00m,
                PreferredWarehouseId = "WH2"
            }]
        };

        var result = AllocationService.AllocateOrder(order);

        result.Details.Should().HaveCount(2);
        result.Details[0].WarehouseId.Should().Be("WH2");
        result.Details[0].AllocatedQty.Should().Be(10);
        result.Details[1].WarehouseId.Should().Be("WH1");
        result.Details[1].AllocatedQty.Should().Be(5);
        result.Details[1].UnitCost.Should().Be(5.00m);
    }

    [Fact]
    public void AllocateOrder_ShouldCreateBackorder_WhenInsufficientStock()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        InMemoryData.Receipts.Add(receipt1);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 15, UnitPrice = 10.00m }]
        };

        var result = AllocationService.AllocateOrder(order);

        result.Details.Should().HaveCount(2);
        result.Details[1].WarehouseId.Should().Be("backorder");
        result.Details[1].AllocatedQty.Should().Be(0);
        result.Details[1].UnitCost.Should().Be(0);

        receipt1.QuantityUsed.Should().Be(10);
    }

    [Fact]
    public void AllocateOrder_ShouldCalculateCOGS_Correctly()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-2), "WH1");
        var receipt2 = new InventoryReceipt("SKU001", 10, 6.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        
        InMemoryData.Receipts.AddRange([receipt1, receipt2]);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 15, UnitPrice = 10.00m }]
        };

        var result = AllocationService.AllocateOrder(order);

        var expectedCOGS = (10 * 5.00m) + (5 * 6.00m);
        result.COGS.Should().Be(expectedCOGS);
    }

    [Fact]
    public void AllocateOrder_ShouldCalculateRevenue_Correctly()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        InMemoryData.Receipts.Add(receipt1);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 8, UnitPrice = 10.00m }]
        };

        var result = AllocationService.AllocateOrder(order);

        var expectedRevenue = 8 * 10.00m;
        result.Revenue.Should().Be(expectedRevenue);
    }

    [Fact]
    public void AllocateOrder_ShouldCalculateRevenue_OnlyForFulfilledQuantity()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        InMemoryData.Receipts.Add(receipt1);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 15, UnitPrice = 10.00m }]
        };

        var result = AllocationService.AllocateOrder(order);

        var expectedRevenue = 10 * 10.00m;
        result.Revenue.Should().Be(expectedRevenue);
    }

    [Fact]
    public void AllocateOrder_ShouldCalculateMargin_Correctly()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        InMemoryData.Receipts.Add(receipt1);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 10, UnitPrice = 10.00m }]
        };

        var result = AllocationService.AllocateOrder(order);

        var expectedCOGS = 10 * 5.00m;
        var expectedRevenue = 10 * 10.00m;
        var expectedMargin = expectedRevenue - expectedCOGS;

        result.COGS.Should().Be(expectedCOGS);
        result.Revenue.Should().Be(expectedRevenue);
        result.Margin.Should().Be(expectedMargin);
    }

    [Fact]
    public void AllocateOrder_ShouldHandleMultipleOrderLines()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        var receipt2 = new InventoryReceipt("SKU002", 10, 6.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        
        InMemoryData.Receipts.AddRange([receipt1, receipt2]);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [
                new OrderLine { Sku = "SKU001", Quantity = 5, UnitPrice = 10.00m },
                new OrderLine { Sku = "SKU002", Quantity = 5, UnitPrice = 12.00m }
            ]
        };

        var result = AllocationService.AllocateOrder(order);

        result.Details.Should().HaveCount(2);
        result.COGS.Should().Be((5 * 5.00m) + (5 * 6.00m));
        result.Revenue.Should().Be((5 * 10.00m) + (5 * 12.00m));
        result.Margin.Should().Be(result.Revenue - result.COGS);
    }

    [Fact]
    public void AllocateOrder_ShouldHandleMultipleOrderLines_WithBackorders()
    {
        var receipt1 = new InventoryReceipt("SKU001", 5, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        var receipt2 = new InventoryReceipt("SKU002", 5, 6.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        
        InMemoryData.Receipts.AddRange([receipt1, receipt2]);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [
                new OrderLine { Sku = "SKU001", Quantity = 10, UnitPrice = 10.00m },
                new OrderLine { Sku = "SKU002", Quantity = 10, UnitPrice = 12.00m }
            ]
        };

        var result = AllocationService.AllocateOrder(order);

        result.Details.Should().HaveCount(4);
        result.Details.Count(d => d.WarehouseId == "backorder").Should().Be(2);
        result.Revenue.Should().Be((5 * 10.00m) + (5 * 12.00m));
    }

    [Fact]
    public void AllocateOrder_ShouldPreserveFIFOOrder_AfterAllocation()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-3), "WH1");
        var receipt2 = new InventoryReceipt("SKU001", 10, 6.00m, DateTime.UtcNow.AddDays(-2), "WH1");
        var receipt3 = new InventoryReceipt("SKU001", 10, 7.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        
        InMemoryData.Receipts.AddRange([receipt1, receipt2, receipt3]);

        var order1 = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 5, UnitPrice = 10.00m }]
        };

        AllocationService.AllocateOrder(order1);

        var order2 = new OrderRequestModel
        {
            OrderId = "ORDER002",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 10, UnitPrice = 10.00m }]
        };

        var result = AllocationService.AllocateOrder(order2);

        result.Details[0].UnitCost.Should().Be(5.00m);
        result.Details[1].UnitCost.Should().Be(6.00m);
        
        receipt1.QuantityUsed.Should().Be(10);
        receipt2.QuantityUsed.Should().Be(5);
        receipt3.QuantityUsed.Should().Be(0);
    }

    [Fact]
    public void AllocateOrder_ShouldAddOrderToInMemoryData()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        InMemoryData.Receipts.Add(receipt1);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 5, UnitPrice = 10.00m }]
        };

        var result = AllocationService.AllocateOrder(order);

        InMemoryData.Orders.Should().HaveCount(1);
        InMemoryData.Orders[0].OrderId.Should().Be("ORDER001");
        InMemoryData.Orders[0].Allocation.Should().Be(result);
    }

    [Fact]
    public void AllocateOrder_ShouldHandleEmptyReceipts()
    {
        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 5, UnitPrice = 10.00m }]
        };

        var result = AllocationService.AllocateOrder(order);

        result.Details.Should().HaveCount(1);
        result.Details[0].WarehouseId.Should().Be("backorder");
        result.COGS.Should().Be(0);
        result.Revenue.Should().Be(0);
        result.Margin.Should().Be(0);
    }

    [Fact]
    public void AllocateOrder_ShouldHandleZeroQuantity()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        InMemoryData.Receipts.Add(receipt1);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine { Sku = "SKU001", Quantity = 0, UnitPrice = 10.00m }]
        };

        var result = AllocationService.AllocateOrder(order);

        result.Details.Should().BeEmpty();
        result.COGS.Should().Be(0);
        result.Revenue.Should().Be(0);
        result.Margin.Should().Be(0);
        receipt1.QuantityUsed.Should().Be(0);
    }

    [Fact]
    public void AllocateOrder_ShouldHandlePreferredWarehouse_WithNoStock()
    {
        var receipt1 = new InventoryReceipt("SKU001", 10, 5.00m, DateTime.UtcNow.AddDays(-1), "WH1");
        InMemoryData.Receipts.Add(receipt1);

        var order = new OrderRequestModel
        {
            OrderId = "ORDER001",
            Lines = [new OrderLine 
            { 
                Sku = "SKU001", 
                Quantity = 5, 
                UnitPrice = 10.00m,
                PreferredWarehouseId = "WH2"
            }]
        };

        var result = AllocationService.AllocateOrder(order);

        result.Details.Should().HaveCount(1);
        result.Details[0].WarehouseId.Should().Be("WH1");
        result.Details[0].AllocatedQty.Should().Be(5);
    }
}


