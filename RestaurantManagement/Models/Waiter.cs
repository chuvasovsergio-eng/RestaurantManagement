namespace RestaurantManagement.Models;

public class Waiter
{
    private string _fullName = string.Empty;

    public Guid Id { get; set; } = Guid.NewGuid();

    public string FullName
    {
        get => _fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ім'я офіціанта не може бути порожнім.");
            _fullName = value;
        }
    }
}