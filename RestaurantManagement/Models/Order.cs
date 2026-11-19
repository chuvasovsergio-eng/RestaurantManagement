namespace RestaurantManagement.Models;

public class Order
{
    private int _quantity;

    public Guid Id { get; set; } = Guid.NewGuid();
    public MenuItem MenuItem { get; set; } = new();

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value < 1)
                throw new ArgumentException("Кількість має бути не менше 1.");
            _quantity = value;
        }
    }

    public OrderStatus Status { get; set; } = OrderStatus.Ordered;
    public List<OrderStatusHistory> StatusHistory { get; set; } = new();

    public decimal TotalPrice => Status == OrderStatus.Cancelled ? 0 : MenuItem.Price * Quantity;

    public Order()
    {
        StatusHistory.Add(new OrderStatusHistory
        {
            Status = OrderStatus.Ordered,
            ChangedAt = DateTime.Now
        });
    }

    public void ChangeStatus(OrderStatus newStatus)
    {
        if (Status == OrderStatus.Served || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Неможливо змінити статус уже поданого або скасованого замовлення.");

        if (Status == newStatus)
            throw new InvalidOperationException("Замовлення вже має цей статус.");

        Status = newStatus;
        StatusHistory.Add(new OrderStatusHistory
        {
            Status = newStatus,
            ChangedAt = DateTime.Now
        });
    }
}