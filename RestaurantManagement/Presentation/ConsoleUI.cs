using RestaurantManagement.BusinessLogic;
using RestaurantManagement.Models;

namespace RestaurantManagement.Presentation;

public class ConsoleUI
{
    private readonly MenuService _menuService;
    private readonly TableService _tableService;
    private readonly WaiterService _waiterService;
    private readonly ReservationService _reservationService;
    private readonly ReceiptService _receiptService;

    public ConsoleUI(
        MenuService menuService,
        TableService tableService,
        WaiterService waiterService,
        ReservationService reservationService,
        ReceiptService receiptService)
    {
        _menuService = menuService;
        _tableService = tableService;
        _waiterService = waiterService;
        _reservationService = reservationService;
        _receiptService = receiptService;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n=== СИСТЕМА УПРАВЛІННЯ ЗАКЛАДОМ ХАРЧУВАННЯ (Етап 4) ===");
            Console.WriteLine("1. Управління меню (CRUD)");
            Console.WriteLine("2. Управління столиками");
            Console.WriteLine("3. Управління офіціантами");
            Console.WriteLine("4. Бронювання столиків");
            Console.WriteLine("5. Відкрити новий чек (за столиком та офіціантом)");
            Console.WriteLine("6. Додати замовлення у відкритий чек");
            Console.WriteLine("7. Змінити статус замовлення");
            Console.WriteLine("8. Пересадити гостей на інший столик");
            Console.WriteLine("9. Застосувати знижку до чеку");
            Console.WriteLine("10. Закрити чек");
            Console.WriteLine("11. Переглянути всі чеки та історію статусів");
            Console.WriteLine("12. Аналітика: виручка, офіціанти та популярні страви");
            Console.WriteLine("0. Вихід");
            Console.Write("Оберіть дію: ");

            string? choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        ManageMenu();
                        break;
                    case "2":
                        ManageTables();
                        break;
                    case "3":
                        ManageWaiters();
                        break;
                    case "4":
                        ManageReservations();
                        break;
                    case "5":
                        OpenReceiptUI();
                        break;
                    case "6":
                        AddOrderToReceiptUI();
                        break;
                    case "7":
                        ChangeOrderStatusUI();
                        break;
                    case "8":
                        ChangeReceiptTableUI();
                        break;
                    case "9":
                        ApplyDiscountUI();
                        break;
                    case "10":
                        CloseReceiptUI();
                        break;
                    case "11":
                        ShowReceipts();
                        break;
                    case "12":
                        ShowAnalyticsReportUI();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний пункт меню.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }

    private void ManageMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- УПРАВЛІННЯ МЕНЮ ---");
            Console.WriteLine("1. Показати меню");
            Console.WriteLine("2. Додати позицію");
            Console.WriteLine("3. Редагувати позицію");
            Console.WriteLine("4. Видалити позицію");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть дію: ");

            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    ShowMenuItems();
                    break;
                case "2":
                    Console.Write("Назва страви: ");
                    string name = Console.ReadLine() ?? "";
                    Console.Write("Ціна: ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                    {
                        Console.WriteLine("Некоректний формат ціни.");
                        break;
                    }
                    _menuService.AddItem(name, price);
                    Console.WriteLine("Позицію додано та збережено у файл.");
                    break;
                case "3":
                    var itemToEdit = SelectMenuItem();
                    if (itemToEdit == null) break;

                    Console.Write($"Нова назва ({itemToEdit.Name}): ");
                    string newName = Console.ReadLine() ?? "";
                    Console.Write($"Нова ціна ({itemToEdit.Price}): ");
                    if (!decimal.TryParse(Console.ReadLine(), out decimal newPrice))
                    {
                        Console.WriteLine("Некоректний формат ціни.");
                        break;
                    }
                    _menuService.UpdateItem(itemToEdit.Id, newName, newPrice);
                    Console.WriteLine("Позицію оновлено.");
                    break;
                case "4":
                    var itemToDelete = SelectMenuItem();
                    if (itemToDelete == null) break;

                    _menuService.DeleteItem(itemToDelete.Id);
                    Console.WriteLine("Позицію видалено.");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Невірний пункт.");
                    break;
            }
        }
    }

    private void ManageTables()
    {
        while (true)
        {
            Console.WriteLine("\n--- УПРАВЛІННЯ СТОЛИКАМИ ---");
            Console.WriteLine("1. Показати всі столики");
            Console.WriteLine("2. Додати столик");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть дію: ");

            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    var tables = _tableService.GetAll();
                    if (tables.Count == 0)
                    {
                        Console.WriteLine("Столиків поки немає.");
                        break;
                    }
                    Console.WriteLine("\nСписок столиків:");
                    foreach (var t in tables)
                    {
                        string status = t.IsOccupied ? "Зайнятий" : "Вільний";
                        Console.WriteLine($"- Столик №{t.Number} (місць: {t.Capacity}) — [{status}]");
                    }
                    break;
                case "2":
                    Console.Write("Номер столика: ");
                    if (!int.TryParse(Console.ReadLine(), out int number))
                    {
                        Console.WriteLine("Некоректний номер.");
                        break;
                    }
                    Console.Write("Кількість місць: ");
                    if (!int.TryParse(Console.ReadLine(), out int capacity))
                    {
                        Console.WriteLine("Некоректна кількість місць.");
                        break;
                    }
                    _tableService.AddTable(number, capacity);
                    Console.WriteLine($"Столик №{number} додано.");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Невірний пункт.");
                    break;
            }
        }
    }

    private void ManageWaiters()
    {
        while (true)
        {
            Console.WriteLine("\n--- УПРАВЛІННЯ ОФІЦІАНТАМИ ---");
            Console.WriteLine("1. Показати список офіціантів");
            Console.WriteLine("2. Додати офіціанта");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть дію: ");

            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    var waiters = _waiterService.GetAll();
                    if (waiters.Count == 0)
                    {
                        Console.WriteLine("Офіціантів поки немає.");
                        break;
                    }
                    Console.WriteLine("\nСписок офіціантів:");
                    for (int i = 0; i < waiters.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {waiters[i].FullName}");
                    }
                    break;
                case "2":
                    Console.Write("ПІБ офіціанта: ");
                    string fullName = Console.ReadLine() ?? "";
                    _waiterService.AddWaiter(fullName);
                    Console.WriteLine("Офіціанта додано.");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Невірний пункт.");
                    break;
            }
        }
    }

    private void ManageReservations()
    {
        while (true)
        {
            Console.WriteLine("\n--- БРОНЮВАННЯ СТОЛИКІВ ---");
            Console.WriteLine("1. Показати активні бронювання на дату");
            Console.WriteLine("2. Створити нове бронювання");
            Console.WriteLine("3. Скасувати бронювання");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть дію: ");

            string? choice = Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("Введіть дату (дд.мм.рррр) або Enter для сьогоднішньої: ");
                        string? dateInput = Console.ReadLine();
                        DateTime date = string.IsNullOrWhiteSpace(dateInput)
                            ? DateTime.Today
                            : DateTime.Parse(dateInput);

                        var bookings = _reservationService.GetActiveByDate(date);
                        if (bookings.Count == 0)
                        {
                            Console.WriteLine($"На {date:dd.MM.yyyy} активних бронювань немає.");
                            break;
                        }

                        Console.WriteLine($"\nБронювання на {date:dd.MM.yyyy}:");
                        foreach (var b in bookings)
                        {
                            Console.WriteLine($"- Столик №{b.TableNumber} | {b.StartTime:HH:mm}–{b.EndTime:HH:mm} | Гість: {b.CustomerName} ({b.CustomerPhone}), {b.GuestsCount} чол.");
                        }
                        break;

                    case "2":
                        var allTables = _tableService.GetAll();
                        if (allTables.Count == 0)
                        {
                            Console.WriteLine("Спочатку додайте столики у розділі 2.");
                            break;
                        }

                        Console.WriteLine("\nДоступні столики:");
                        for (int i = 0; i < allTables.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. Столик №{allTables[i].Number} (місць: {allTables[i].Capacity})");
                        }
                        Console.Write("Оберіть столик: ");
                        if (!int.TryParse(Console.ReadLine(), out int tIdx) || tIdx < 1 || tIdx > allTables.Count)
                        {
                            Console.WriteLine("Невірний вибір столика.");
                            break;
                        }

                        Console.Write("Ім'я гостя: ");
                        string guestName = Console.ReadLine() ?? "";
                        Console.Write("Телефон гостя: ");
                        string guestPhone = Console.ReadLine() ?? "";
                        Console.Write("Кількість гостей: ");
                        if (!int.TryParse(Console.ReadLine(), out int guestsCount))
                        {
                            Console.WriteLine("Некоректна кількість гостей.");
                            break;
                        }
                        Console.Write("Дата та час початку (наприклад, 21.11.2026 19:00): ");
                        if (!DateTime.TryParse(Console.ReadLine(), out DateTime startTime))
                        {
                            Console.WriteLine("Некоректний формат дати та часу.");
                            break;
                        }
                        Console.Write("Тривалість у хвилинах (Enter = 120): ");
                        string? durInput = Console.ReadLine();
                        int duration = string.IsNullOrWhiteSpace(durInput) ? 120 : int.Parse(durInput);

                        var created = _reservationService.CreateReservation(
                            allTables[tIdx - 1].Id,
                            guestName,
                            guestPhone,
                            guestsCount,
                            startTime,
                            duration);

                        Console.WriteLine($"Бронювання створено! Столик №{created.TableNumber} на {created.StartTime:dd.MM.yyyy HH:mm}–{created.EndTime:HH:mm}.");
                        break;

                    case "3":
                        var activeAll = _reservationService.GetAll().Where(r => !r.IsCancelled).ToList();
                        if (activeAll.Count == 0)
                        {
                            Console.WriteLine("Активних бронювань немає.");
                            break;
                        }

                        for (int i = 0; i < activeAll.Count; i++)
                        {
                            var b = activeAll[i];
                            Console.WriteLine($"{i + 1}. Столик №{b.TableNumber} | {b.StartTime:dd.MM.yyyy HH:mm} | {b.CustomerName}");
                        }
                        Console.Write("Оберіть номер для скасування: ");
                        if (!int.TryParse(Console.ReadLine(), out int bIdx) || bIdx < 1 || bIdx > activeAll.Count)
                        {
                            Console.WriteLine("Невірний номер.");
                            break;
                        }

                        _reservationService.CancelReservation(activeAll[bIdx - 1].Id);
                        Console.WriteLine("Бронювання скасовано.");
                        break;

                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний пункт.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }

    private void ShowMenuItems()
    {
        var items = _menuService.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("Меню поки порожнє.");
            return;
        }

        Console.WriteLine("\nСписок страв:");
        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {items[i].Name} — {items[i].Price:F2} грн");
        }
    }

    private MenuItem? SelectMenuItem()
    {
        var items = _menuService.GetAll();
        if (items.Count == 0)
        {
            Console.WriteLine("Меню порожнє.");
            return null;
        }

        ShowMenuItems();
        Console.Write("Введіть номер позиції: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > items.Count)
        {
            Console.WriteLine("Невірний номер.");
            return null;
        }

        return items[index - 1];
    }

    private Table? SelectFreeTable()
    {
        var freeTables = _tableService.GetFreeTables();
        if (freeTables.Count == 0)
        {
            Console.WriteLine("Немає вільних столиків. Додайте столик або закрийте один із чеків.");
            return null;
        }

        Console.WriteLine("\nВільні столики:");
        for (int i = 0; i < freeTables.Count; i++)
        {
            Console.WriteLine($"{i + 1}. Столик №{freeTables[i].Number} (місць: {freeTables[i].Capacity})");
        }

        Console.Write("Оберіть номер зі списку: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > freeTables.Count)
        {
            Console.WriteLine("Невірний вибір.");
            return null;
        }

        return freeTables[index - 1];
    }

    private Waiter? SelectWaiter()
    {
        var waiters = _waiterService.GetAll();
        if (waiters.Count == 0)
        {
            Console.WriteLine("Список офіціантів порожній. Спочатку додайте офіціанта.");
            return null;
        }

        Console.WriteLine("\nОфіціанти:");
        for (int i = 0; i < waiters.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {waiters[i].FullName}");
        }

        Console.Write("Оберіть офіціанта: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > waiters.Count)
        {
            Console.WriteLine("Невірний вибір.");
            return null;
        }

        return waiters[index - 1];
    }

    private Receipt? SelectOpenReceipt()
    {
        var openReceipts = _receiptService.GetOpenReceipts();
        if (openReceipts.Count == 0)
        {
            Console.WriteLine("Немає відкритих чеків.");
            return null;
        }

        Console.WriteLine("\nВідкриті чеки:");
        for (int i = 0; i < openReceipts.Count; i++)
        {
            var r = openReceipts[i];
            Console.WriteLine($"{i + 1}. Столик №{r.TableNumber} | Офіціант: {r.WaiterName} | Знижка: {r.DiscountPercent}% | Сума: {r.TotalAmount:F2} грн");
        }

        Console.Write("Оберіть номер чеку: ");
        if (!int.TryParse(Console.ReadLine(), out int receiptIndex) || receiptIndex < 1 || receiptIndex > openReceipts.Count)
        {
            Console.WriteLine("Невірний номер чеку.");
            return null;
        }

        return openReceipts[receiptIndex - 1];
    }

    private void OpenReceiptUI()
    {
        var table = SelectFreeTable();
        if (table == null) return;

        var waiter = SelectWaiter();
        if (waiter == null) return;

        var receipt = _receiptService.OpenReceipt(table, waiter);
        Console.WriteLine($"Відкрито чек! Столик №{receipt.TableNumber} | Офіціант: {receipt.WaiterName} | ID: {receipt.Id}");
    }

    private void ChangeReceiptTableUI()
    {
        var receipt = SelectOpenReceipt();
        if (receipt == null) return;

        Console.WriteLine($"Поточний столик: №{receipt.TableNumber}. Оберіть новий вільний столик:");
        var newTable = SelectFreeTable();
        if (newTable == null) return;

        _receiptService.ChangeReceiptTable(receipt.Id, newTable);
        Console.WriteLine($"Чек перенесено на столик №{newTable.Number}.");
    }

    private void ApplyDiscountUI()
    {
        var receipt = SelectOpenReceipt();
        if (receipt == null) return;

        Console.Write("Введіть розмір знижки у відсотках (0–100): ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal discount))
        {
            Console.WriteLine("Некоректне значення знижки.");
            return;
        }

        _receiptService.ApplyDiscount(receipt.Id, discount);
        Console.WriteLine($"Знижку {discount}% застосовано. Нова сума до сплати: {receipt.TotalAmount:F2} грн.");
    }

    private void AddOrderToReceiptUI()
    {
        var selectedReceipt = SelectOpenReceipt();
        if (selectedReceipt == null) return;

        var selectedItem = SelectMenuItem();
        if (selectedItem == null) return;

        Console.Write("Вкажіть кількість: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity < 1)
        {
            Console.WriteLine("Кількість має бути цілим числом >= 1.");
            return;
        }

        if (_receiptService.AddOrderToReceipt(selectedReceipt.Id, selectedItem, quantity))
        {
            Console.WriteLine($"Додано: {selectedItem.Name} x{quantity} у чек (Столик №{selectedReceipt.TableNumber}).");
        }
    }

    private void ChangeOrderStatusUI()
    {
        var selectedReceipt = SelectOpenReceipt();
        if (selectedReceipt == null) return;

        if (selectedReceipt.Orders.Count == 0)
        {
            Console.WriteLine("У цьому чеку ще немає замовлень.");
            return;
        }

        Console.WriteLine("\nЗамовлення у чеку:");
        for (int i = 0; i < selectedReceipt.Orders.Count; i++)
        {
            var o = selectedReceipt.Orders[i];
            Console.WriteLine($"{i + 1}. {o.MenuItem.Name} x{o.Quantity} — поточний статус: {FormatStatus(o.Status)}");
        }

        Console.Write("Оберіть номер замовлення: ");
        if (!int.TryParse(Console.ReadLine(), out int orderIndex) || orderIndex < 1 || orderIndex > selectedReceipt.Orders.Count)
        {
            Console.WriteLine("Невірний номер замовлення.");
            return;
        }

        var selectedOrder = selectedReceipt.Orders[orderIndex - 1];

        Console.WriteLine("Оберіть новий статус:");
        Console.WriteLine("1. Готується");
        Console.WriteLine("2. Подано");
        Console.WriteLine("3. Скасовано");
        Console.Write("Ваш вибір: ");

        OrderStatus? newStatus = Console.ReadLine() switch
        {
            "1" => OrderStatus.Preparing,
            "2" => OrderStatus.Served,
            "3" => OrderStatus.Cancelled,
            _ => null
        };

        if (newStatus == null)
        {
            Console.WriteLine("Невірний вибір статусу.");
            return;
        }

        _receiptService.ChangeOrderStatus(selectedReceipt.Id, selectedOrder.Id, newStatus.Value);
        Console.WriteLine($"Статус замовлення «{selectedOrder.MenuItem.Name}» змінено на «{FormatStatus(newStatus.Value)}».");
    }

    private void CloseReceiptUI()
    {
        var selectedReceipt = SelectOpenReceipt();
        if (selectedReceipt == null) return;

        _receiptService.CloseReceipt(selectedReceipt.Id);
        Console.WriteLine($"Чек (Столик №{selectedReceipt.TableNumber}) закрито! Підсумкова сума: {selectedReceipt.TotalAmount:F2} грн. Столик знову вільний.");
    }

    private void ShowReceipts()
    {
        var receipts = _receiptService.GetAll();
        if (receipts.Count == 0)
        {
            Console.WriteLine("Чеків поки немає.");
            return;
        }

        Console.WriteLine("\n--- УСІ ЧЕКИ ---");
        foreach (var r in receipts)
        {
            string status = r.IsOpen
                ? "Відкритий"
                : $"Закритий ({r.ClosedAt:dd.MM.yyyy HH:mm:ss})";

            Console.WriteLine($"\nЧек ID: {r.Id} | Столик №{r.TableNumber} | Офіціант: {r.WaiterName}");
            Console.WriteLine($"Статус: {status} | Відкрито: {r.OpenedAt:dd.MM.yyyy HH:mm:ss}");
            if (r.Orders.Count == 0)
            {
                Console.WriteLine("  (Замовлень поки немає)");
            }
            else
            {
                foreach (var o in r.Orders)
                {
                    Console.WriteLine($"  - {o.MenuItem.Name} x{o.Quantity} = {o.TotalPrice:F2} грн [{FormatStatus(o.Status)}]");
                    foreach (var h in o.StatusHistory)
                    {
                        Console.WriteLine($"      * {h.ChangedAt:HH:mm:ss} -> {FormatStatus(h.Status)}");
                    }
                }
            }
            if (r.DiscountPercent > 0)
            {
                Console.WriteLine($"  Сума без знижки: {r.SubtotalAmount:F2} грн | Знижка {r.DiscountPercent}%: -{r.DiscountAmount:F2} грн");
            }
            Console.WriteLine($"  До сплати: {r.TotalAmount:F2} грн");
        }
    }

    private void ShowAnalyticsReportUI()
    {
        Console.Write("Введіть дату для звіту (дд.мм.рррр) або Enter для сьогоднішньої: ");
        string? input = Console.ReadLine();

        DateTime targetDate;
        if (string.IsNullOrWhiteSpace(input))
        {
            targetDate = DateTime.Today;
        }
        else if (!DateTime.TryParse(input, out targetDate))
        {
            Console.WriteLine("Некоректний формат дати.");
            return;
        }

        decimal totalRevenue = _receiptService.CalculateDailyRevenue(targetDate);
        Console.WriteLine($"\n=== АНАЛІТИЧНИЙ ЗВІТ ЗА {targetDate:dd.MM.yyyy} ===");
        Console.WriteLine($"Загальна виручка закладу: {totalRevenue:F2} грн");

        var waiters = _waiterService.GetAll();
        if (waiters.Count > 0)
        {
            Console.WriteLine("\nСтатистика по офіціантах:");
            foreach (var w in waiters)
            {
                var (closedCount, revenue) = _receiptService.GetWaiterStats(w.Id, targetDate);
                Console.WriteLine($"- {w.FullName}: закрито чеків — {closedCount}, виручка — {revenue:F2} грн");
            }
        }

        var popular = _receiptService.GetPopularDishes();
        Console.WriteLine("\nРейтинг популярних страв (по всіх закритих чеках):");
        if (popular.Count == 0)
        {
            Console.WriteLine("  (Поки немає поданих страв у закритих чеках)");
        }
        else
        {
            for (int i = 0; i < popular.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {popular[i].DishName} — замовлено {popular[i].TotalQuantity} порц. (виручка: {popular[i].TotalSales:F2} грн)");
            }
        }
    }

    private static string FormatStatus(OrderStatus status) => status switch
    {
        OrderStatus.Ordered => "Замовлено",
        OrderStatus.Preparing => "Готується",
        OrderStatus.Served => "Подано",
        OrderStatus.Cancelled => "Скасовано",
        _ => status.ToString()
    };
}