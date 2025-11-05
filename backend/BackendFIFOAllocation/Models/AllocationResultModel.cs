namespace BackendFIFOAllocation.Models;

public record AllocationResultModel(
    string OrderId,
    decimal COGS,
    decimal Revenue,
    decimal Margin,
List<AllocationDetailModel> Details);
