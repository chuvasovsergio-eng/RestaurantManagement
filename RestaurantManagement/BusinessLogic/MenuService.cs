using RestaurantManagement.Models;
using RestaurantManagement.Persistence;

namespace RestaurantManagement.BusinessLogic;

public class MenuService
{
    private readonly JsonStorage<Menu> _storage;
    private Menu _menu;

    public MenuService(JsonStorage<Menu> storage)
    {
        _storage = storage;
        _menu = _storage.Load();
    }

    public IReadOnlyList<MenuItem> GetAll() => _menu.Items;

    public MenuItem? GetById(Guid id) => _menu.Items.FirstOrDefault(x => x.Id == id);

    public MenuItem AddItem(string name, decimal price)
    {
        var item = new MenuItem
        {
            Name = name,
            Price = price
        };
        _menu.Items.Add(item);
        _storage.Save(_menu);
        return item;
    }

    public bool UpdateItem(Guid id, string newName, decimal newPrice)
    {
        var item = GetById(id);
        if (item == null)
            return false;

        item.Name = newName;
        item.Price = newPrice;
        _storage.Save(_menu);
        return true;
    }

    public bool DeleteItem(Guid id)
    {
        var item = GetById(id);
        if (item == null)
            return false;

        _menu.Items.Remove(item);
        _storage.Save(_menu);
        return true;
    }
}