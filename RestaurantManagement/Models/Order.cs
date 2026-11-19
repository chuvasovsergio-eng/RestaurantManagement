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
                throw new ArgumentException("Количество должно быть не меньше 1.");
            _quantity = value;
        }
    }

    public decimal TotalPrice => MenuItem.Price * Quantity;
}