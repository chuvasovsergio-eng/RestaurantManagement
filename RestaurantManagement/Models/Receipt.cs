namespace RestaurantManagement.Models;

public class Receipt
{
    private decimal _discountPercent;

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TableId { get; set; }
    public int TableNumber { get; set; }
    public Guid? WaiterId { get; set; }
    public string WaiterName { get; set; } = "Не вказано";
    public DateTime OpenedAt { get; set; } = DateTime.Now;
    public DateTime? ClosedAt { get; set; }
    public bool IsOpen { get; set; } = true;
    public List<Order> Orders { get; set; } = new();

    public decimal DiscountPercent
    {
        get => _discountPercent;
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentException("Знижка має бути в межах від 0% до 100%.");
            _discountPercent = value;
        }
    }

    public decimal SubtotalAmount => Orders.Sum(o => o.TotalPrice);
    public decimal DiscountAmount => Math.Round(SubtotalAmount * (DiscountPercent / 100m), 2);
    public decimal TotalAmount => SubtotalAmount - DiscountAmount;

    public void Close()
    {
        if (!IsOpen)
            throw new InvalidOperationException("Чек уже закритий.");

        bool hasUnfinishedOrders = Orders.Any(o => o.Status != OrderStatus.Served && o.Status != OrderStatus.Cancelled);
        if (hasUnfinishedOrders)
            throw new InvalidOperationException("Неможливо закрити чек: є замовлення, які ще не подані або не скасовані.");

        IsOpen = false;
        ClosedAt = DateTime.Now;
    }
}