using RestaurantManagement.Models;
using RestaurantManagement.Persistence;

namespace RestaurantManagement.BusinessLogic;

public class ReceiptService
{
    private readonly JsonStorage<List<Receipt>> _storage;
    private readonly List<Receipt> _receipts;

    public ReceiptService(JsonStorage<List<Receipt>> storage)
    {
        _storage = storage;
        _receipts = _storage.Load();
    }

    public IReadOnlyList<Receipt> GetAll() => _receipts;

    public IReadOnlyList<Receipt> GetOpenReceipts() => _receipts.Where(r => r.IsOpen).ToList();

    public Receipt? GetById(Guid id) => _receipts.FirstOrDefault(r => r.Id == id);

    public Receipt OpenReceipt()
    {
        var receipt = new Receipt();
        _receipts.Add(receipt);
        _storage.Save(_receipts);
        return receipt;
    }

    public bool AddOrderToReceipt(Guid receiptId, MenuItem menuItem, int quantity)
    {
        var receipt = GetById(receiptId);
        if (receipt == null || !receipt.IsOpen)
            return false;

        var order = new Order
        {
            MenuItem = new MenuItem
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Price = menuItem.Price
            },
            Quantity = quantity
        };

        receipt.Orders.Add(order);
        _storage.Save(_receipts);
        return true;
    }
}