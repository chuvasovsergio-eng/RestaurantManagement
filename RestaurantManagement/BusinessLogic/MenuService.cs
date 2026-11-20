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

    public IReadOnlyList<MenuItem> GetByCategory(MenuCategory category) =>
        _menu.Items.Where(x => x.Category == category).ToList();

    public IReadOnlyList<MenuItem> SearchByName(string query) =>
        _menu.Items
            .Where(x => x.Name.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase))
            .ToList();

    public MenuItem? GetById(Guid id) => _menu.Items.FirstOrDefault(x => x.Id == id);

    public MenuItem AddItem(string name, decimal price, MenuCategory category = MenuCategory.MainCourses)
    {
        var item = new MenuItem
        {
            Name = name,
            Price = price,
            Category = category
        };
        _menu.Items.Add(item);
        _storage.Save(_menu);
        return item;
    }

    public bool UpdateItem(Guid id, string newName, decimal newPrice, MenuCategory newCategory)
    {
        var item = GetById(id);
        if (item == null)
            return false;

        item.Name = newName;
        item.Price = newPrice;
        item.Category = newCategory;
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