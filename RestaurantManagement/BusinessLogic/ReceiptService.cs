using RestaurantManagement.Models;
using RestaurantManagement.Persistence;

namespace RestaurantManagement.BusinessLogic;

public class ReceiptService
{
    private readonly JsonStorage<List<Receipt>> _storage;
    private readonly TableService _tableService;
    private readonly List<Receipt> _receipts;

    public ReceiptService(JsonStorage<List<Receipt>> storage, TableService tableService)
    {
        _storage = storage;
        _tableService = tableService;
        _receipts = _storage.Load();
    }

    public IReadOnlyList<Receipt> GetAll() => _receipts;

    public IReadOnlyList<Receipt> GetOpenReceipts() => _receipts.Where(r => r.IsOpen).ToList();

    public Receipt? GetById(Guid id) => _receipts.FirstOrDefault(r => r.Id == id);

    public Receipt OpenReceipt(Table table, Waiter waiter)
    {
        if (table.IsOccupied)
            throw new InvalidOperationException($"Столик №{table.Number} уже зайнятий.");

        var receipt = new Receipt
        {
            TableId = table.Id,
            TableNumber = table.Number,
            WaiterId = waiter.Id,
            WaiterName = waiter.FullName
        };

        _tableService.SetOccupied(table.Id, true);
        _receipts.Add(receipt);
        _storage.Save(_receipts);
        return receipt;
    }

    public void ChangeReceiptTable(Guid receiptId, Table newTable)
    {
        var receipt = GetById(receiptId) ?? throw new ArgumentException("Чек не знайдено.");
        if (!receipt.IsOpen)
            throw new InvalidOperationException("Неможливо змінити столик для закритого чеку.");

        if (newTable.IsOccupied)
            throw new InvalidOperationException($"Столик №{newTable.Number} уже зайнятий.");

        if (receipt.TableId.HasValue)
        {
            var oldTable = _tableService.GetById(receipt.TableId.Value);
            if (oldTable != null)
                _tableService.SetOccupied(oldTable.Id, false);
        }

        _tableService.SetOccupied(newTable.Id, true);
        receipt.TableId = newTable.Id;
        receipt.TableNumber = newTable.Number;
        _storage.Save(_receipts);
    }

    public void ApplyDiscount(Guid receiptId, decimal discountPercent)
    {
        var receipt = GetById(receiptId) ?? throw new ArgumentException("Чек не знайдено.");
        if (!receipt.IsOpen)
            throw new InvalidOperationException("Неможливо застосувати знижку до закритого чеку.");

        receipt.DiscountPercent = discountPercent;
        _storage.Save(_receipts);
    }

    public bool AddOrderToReceipt(Guid receiptId, MenuItem menuItem, int quantity)
    {
        var receipt = GetById(receiptId) ?? throw new ArgumentException("Чек не знайдено.");
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

        if (receipt.TableId.HasValue)
        {
            var table = _tableService.GetById(receipt.TableId.Value);
            if (table != null)
                _tableService.SetOccupied(table.Id, false);
        }

        _storage.Save(_receipts);
    }

    public decimal CalculateDailyRevenue(DateTime date)
    {
        return _receipts
            .Where(r => !r.IsOpen && r.ClosedAt.HasValue && r.ClosedAt.Value.Date == date.Date)
            .Sum(r => r.TotalAmount);
    }

    public (int ClosedCount, decimal Revenue) GetWaiterStats(Guid waiterId, DateTime date)
    {
        var waiterReceipts = _receipts
            .Where(r => !r.IsOpen
                        && r.ClosedAt.HasValue
                        && r.ClosedAt.Value.Date == date.Date
                        && r.WaiterId == waiterId)
            .ToList();

        return (waiterReceipts.Count, waiterReceipts.Sum(r => r.TotalAmount));
    }

    public List<(string DishName, int TotalQuantity, decimal TotalSales)> GetPopularDishes()
    {
        return _receipts
            .Where(r => !r.IsOpen)
            .SelectMany(r => r.Orders)
            .Where(o => o.Status == OrderStatus.Served)
            .GroupBy(o => o.MenuItem.Name)
            .Select(g => (
                DishName: g.Key,
                TotalQuantity: g.Sum(x => x.Quantity),
                TotalSales: g.Sum(x => x.TotalPrice)
            ))
            .OrderByDescending(x => x.TotalQuantity)
            .ThenByDescending(x => x.TotalSales)
            .ToList();
    }
}