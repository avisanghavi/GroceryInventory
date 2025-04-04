namespace GroceryInventory.Core.Models;

public class Order
{
    public int Id { get; set; }
    public int GroceryItemId { get; set; }
    public GroceryItem? GroceryItem { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public int Quantity { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? DeliveryDate { get; set; }
    public string Status { get; set; } = string.Empty; // Pending, Delivered, Cancelled
}