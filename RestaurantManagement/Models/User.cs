namespace RestaurantManagement.Models;

public class User
{
    private string _username = string.Empty;
    private string _fullName = string.Empty;

    public Guid Id { get; set; } = Guid.NewGuid();

    public string Username
    {
        get => _username;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Логін не може бути порожнім.");
            _username = value.Trim().ToLowerInvariant();
        }
    }

    public string PasswordHash { get; set; } = string.Empty;

    public string FullName
    {
        get => _fullName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("ПІБ користувача не може бути порожнім.");
            _fullName = value.Trim();
        }
    }

    public UserRole Role { get; set; } = UserRole.Waiter;
}