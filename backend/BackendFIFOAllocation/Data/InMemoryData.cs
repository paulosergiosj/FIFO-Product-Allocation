using BackendFIFOAllocation.Models;

namespace BackendFIFOAllocation.Data;
public class InMemoryData
{
    public static List<InventoryReceipt> Receipts { get; set; } = [];
    public static List<Order> Orders { get; set; } = [];

    public static void ClearAll()
    {
        Receipts.Clear();
        Orders.Clear();
    }
}
