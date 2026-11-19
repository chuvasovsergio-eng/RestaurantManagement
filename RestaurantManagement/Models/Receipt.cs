namespace RestaurantManagement.Models;

public class Receipt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime OpenedAt { get; set; } = DateTime.Now;
    public bool IsOpen { get; set; } = true;
    public List<Order> Orders { get; set; } = new();

    public decimal TotalAmount => Orders.Sum(o => o.TotalPrice);
}