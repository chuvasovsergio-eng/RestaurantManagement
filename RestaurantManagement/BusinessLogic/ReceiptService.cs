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
        if (receipt == null)
            throw new ArgumentException("Чек не знайдено.");

        if (!receipt.IsOpen)
            throw new InvalidOperationException("Неможливо додати замовлення у закритий чек.");

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

    public void ChangeOrderStatus(Guid receiptId, Guid orderId, OrderStatus newStatus)
    {
        var receipt = GetById(receiptId) ?? throw new ArgumentException("Чек не знайдено.");
        if (!receipt.IsOpen)
            throw new InvalidOperationException("Неможливо змінювати замовлення у закритому чеку.");

        var order = receipt.Orders.FirstOrDefault(o => o.Id == orderId)
            ?? throw new ArgumentException("Замовлення не знайдено.");

        order.ChangeStatus(newStatus);
        _storage.Save(_receipts);
    }

    public void CloseReceipt(Guid receiptId)
    {
        var receipt = GetById(receiptId) ?? throw new ArgumentException("Чек не знайдено.");
        receipt.Close();
        _storage.Save(_receipts);
    }

    public decimal CalculateDailyRevenue(DateTime date)
    {
        return _receipts
            .Where(r => !r.IsOpen && r.ClosedAt.HasValue && r.ClosedAt.Value.Date == date.Date)
            .Sum(r => r.TotalAmount);
    }
}