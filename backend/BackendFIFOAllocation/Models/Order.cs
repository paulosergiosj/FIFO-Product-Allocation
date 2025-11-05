namespace BackendFIFOAllocation.Models;

public class Order(string orderId, List<OrderLine> lines, AllocationResultModel allocation)
{
    public string OrderId { get; set; } = orderId;
    public List<OrderLine> Lines { get; set; } = lines;
    public AllocationResultModel Allocation { get; set; } = allocation;
}
