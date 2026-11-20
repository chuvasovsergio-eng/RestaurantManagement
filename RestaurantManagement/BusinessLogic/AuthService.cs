using System.Security.Cryptography;
using System.Text;
using RestaurantManagement.Models;
using RestaurantManagement.Persistence;

namespace RestaurantManagement.BusinessLogic;

public class AuthService
{
    private readonly JsonStorage<List<User>> _storage;
    private readonly List<User> _users;

    public User? CurrentUser { get; private set; }

    public AuthService(JsonStorage<List<User>> storage)
    {
        _storage = storage;
        _users = _storage.Load();

        if (_users.Count == 0)
        {
            SeedDefaultUsers();
        }
    }

    private void SeedDefaultUsers()
    {
        _users.Add(new User
        {
            Username = "admin",
            PasswordHash = HashPassword("admin123"),
            FullName = "Адміністратор Закладу",
            Role = UserRole.Admin
        });

        _users.Add(new User
        {
            Username = "waiter",
            PasswordHash = HashPassword("1234"),
            FullName = "Офіціант Черговий",
            Role = UserRole.Waiter
        });

        _storage.Save(_users);
    }

    public IReadOnlyList<User> GetAllUsers() => _users;

    public User Login(string username, string password)
    {
        string normalized = username.Trim().ToLowerInvariant();
        string hash = HashPassword(password);

        var user = _users.FirstOrDefault(u => u.Username == normalized && u.PasswordHash == hash)
            ?? throw new UnauthorizedAccessException("Невірний логін або пароль.");

        CurrentUser = user;
        return user;
    }

    public void Logout()
    {
        CurrentUser = null;
    }

    public User RegisterUser(string username, string password, string fullName, UserRole role)
    {
        EnsureAdmin();

        if (string.IsNullOrWhiteSpace(password) || password.Length < 4)
            throw new ArgumentException("Пароль має містити щонайменше 4 символи.");

        string normalized = username.Trim().ToLowerInvariant();
        if (_users.Any(u => u.Username == normalized))
            throw new InvalidOperationException($"Користувач із логіном '{normalized}' уже існує.");

        var user = new User
        {
            Username = normalized,
            PasswordHash = HashPassword(password),
            FullName = fullName,
            Role = role
        };

        _users.Add(user);
        _storage.Save(_users);
        return user;
    }

    public void EnsureAuthenticated()
    {
        if (CurrentUser == null)
            throw new UnauthorizedAccessException("Необхідно виконати вхід у систему.");
    }

    public void EnsureAdmin()
    {
        EnsureAuthenticated();
        if (CurrentUser!.Role != UserRole.Admin)
            throw new UnauthorizedAccessException("Доступ заборонено: потрібні права Адміністратора.");
    }

    private static string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}