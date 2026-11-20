namespace RestaurantManagement.Models;

public class Reservation
{
    private string _customerName = string.Empty;
    private string _customerPhone = string.Empty;
    private int _guestsCount;
    private int _durationMinutes = 120;

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TableId { get; set; }
    public int TableNumber { get; set; }

    public string CustomerName
    {
        get => _customerName;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Ім'я клієнта не може бути порожнім.");
            _customerName = value.Trim();
        }
    }

    public string CustomerPhone
    {
        get => _customerPhone;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Телефон клієнта не може бути порожнім.");
            _customerPhone = value.Trim();
        }
    }

    public int GuestsCount
    {
        get => _guestsCount;
        set
        {
            if (value < 1)
                throw new ArgumentException("Кількість гостей має бути не менше 1.");
            _guestsCount = value;
        }
    }

    public DateTime StartTime { get; set; }

    public int DurationMinutes
    {
        get => _durationMinutes;
        set
        {
            if (value < 30)
                throw new ArgumentException("Мінімальна тривалість бронювання — 30 хвилин.");
            _durationMinutes = value;
        }
    }

    public DateTime EndTime => StartTime.AddMinutes(DurationMinutes);
    public bool IsCancelled { get; set; } = false;
}