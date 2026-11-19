using RestaurantManagement.Models;
using RestaurantManagement.Persistence;

namespace RestaurantManagement.BusinessLogic;

public class WaiterService
{
    private readonly JsonStorage<List<Waiter>> _storage;
    private readonly List<Waiter> _waiters;

    public WaiterService(JsonStorage<List<Waiter>> storage)
    {
        _storage = storage;
        _waiters = _storage.Load();
    }

    public IReadOnlyList<Waiter> GetAll() => _waiters;

    public Waiter? GetById(Guid id) => _waiters.FirstOrDefault(w => w.Id == id);

    public Waiter AddWaiter(string fullName)
    {
        var waiter = new Waiter { FullName = fullName.Trim() };
        _waiters.Add(waiter);
        _storage.Save(_waiters);
        return waiter;
    }
}