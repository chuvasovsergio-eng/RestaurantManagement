namespace RestaurantManagement.Models;

public class MenuItem
{
    private string _name = string.Empty;
    private decimal _price;

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Название блюда не может быть пустым.");
            _name = value;
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Цена должна быть больше 0.");
            _price = value;
        }
    }
}