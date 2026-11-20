namespace RestaurantManagement.Models;

public class OrderStatusHistory
{
    public OrderStatus Status { get; set; }
    public DateTime ChangedAt { get; set; } = DateTime.Now;
}