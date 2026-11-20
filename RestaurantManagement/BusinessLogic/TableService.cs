using RestaurantManagement.Models;
using RestaurantManagement.Persistence;

namespace RestaurantManagement.BusinessLogic;

public class TableService
{
    private readonly JsonStorage<List<Table>> _storage;
    private readonly List<Table> _tables;

    public TableService(JsonStorage<List<Table>> storage)
    {
        _storage = storage;
        _tables = _storage.Load();
    }

    public IReadOnlyList<Table> GetAll() => _tables.OrderBy(t => t.Number).ToList();

    public IReadOnlyList<Table> GetFreeTables() => _tables.Where(t => !t.IsOccupied).OrderBy(t => t.Number).ToList();

    public Table? GetById(Guid id) => _tables.FirstOrDefault(t => t.Id == id);

    public Table AddTable(int number, int capacity)
    {
        if (_tables.Any(t => t.Number == number))
            throw new InvalidOperationException($"Столик №{number} уже існує.");

        var table = new Table
        {
            Number = number,
            Capacity = capacity
        };

        _tables.Add(table);
        _storage.Save(_tables);
        return table;
    }

    public void SetOccupied(Guid tableId, bool isOccupied)
    {
        var table = GetById(tableId) ?? throw new ArgumentException("Столик не знайдено.");
        table.IsOccupied = isOccupied;
        _storage.Save(_tables);
    }
}