namespace RestaurantManagement.Models;

public class Table
{
    private int _number;
    private int _capacity;

    public Guid Id { get; set; } = Guid.NewGuid();

    public int Number
    {
        get => _number;
        set
        {
            if (value < 1)
                throw new ArgumentException("Номер столика має бути не менше 1.");
            _number = value;
        }
    }

    public int Capacity
    {
        get => _capacity;
        set
        {
            if (value < 1)
                throw new ArgumentException("Місткість столика має бути не менше 1.");
            _capacity = value;
        }
    }

    public bool IsOccupied { get; set; } = false;
}