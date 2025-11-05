using BackendFIFOAllocation.Data;
using BackendFIFOAllocation.Models;

namespace BackendFIFOAllocation.Services;

public class AllocationService
{
    public static AllocationResultModel AllocateOrder(OrderRequestModel order)
    {
        decimal totalCOGS = 0;
        decimal totalRevenue = 0;
        List<AllocationDetailModel> details = [];

        foreach (var line in order.Lines)
        {
            var receipts = InMemoryData.Receipts
                .Where(r => r.Sku == line.Sku && r.QuantityAvailable > 0)
                .OrderBy(r => r.ReceivedAtUtc)
                .ToList();

            if (line.PreferredWarehouseId is not null)
            {
                receipts = [.. receipts
                    .OrderByDescending(r => r.WarehouseId == line.PreferredWarehouseId)
                    .ThenBy(r => r.ReceivedAtUtc)];
            }

            int remaining = line.Quantity;
            decimal cogs = 0;

            foreach (var receipt in receipts)
            {
                if (remaining <= 0) break;
                int allocQty = Math.Min(receipt.QuantityAvailable, remaining);
                receipt.QuantityUsed += allocQty;

                cogs += allocQty * receipt.UnitCost;
                remaining -= allocQty;

                details.Add(new AllocationDetailModel(line.Sku, allocQty, receipt.WarehouseId, receipt.UnitCost));
            }

            totalCOGS += cogs;
            totalRevenue += (line.Quantity - remaining) * line.UnitPrice;

            if (remaining > 0)
            {
                details.Add(new AllocationDetailModel(line.Sku, 0, "backorder", 0));
            }
        }

        decimal margin = totalRevenue - totalCOGS;
        var allocationResult = new AllocationResultModel(order.OrderId, totalCOGS, totalRevenue, margin, details);

        InMemoryData.Orders.Add(new(order.OrderId, order.Lines, allocationResult));

        return allocationResult;
    }
}
