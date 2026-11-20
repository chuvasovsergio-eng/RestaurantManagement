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
                throw new ArgumentException("Назва страви не може бути порожньою.");
            _name = value.Trim();
        }
    }

    public decimal Price
    {
        get => _price;
        set
        {
            if (value <= 0)
                throw new ArgumentException("Ціна має бути більшою за 0.");
            _price = value;
        }
    }

    public MenuCategory Category { get; set; } = MenuCategory.MainCourses;
}