using RestaurantManagement.BusinessLogic;
using RestaurantManagement.Models;

namespace RestaurantManagement.Presentation;

public class ConsoleUI
{
    private readonly AuthService _authService;
    private readonly AuditService _auditService;
    private readonly MenuService _menuService;
    private readonly TableService _tableService;
    private readonly WaiterService _waiterService;
    private readonly ReservationService _reservationService;
    private readonly ReceiptService _receiptService;

    public ConsoleUI(
        AuthService authService,
        AuditService auditService,
        MenuService menuService,
        TableService tableService,
        WaiterService waiterService,
        ReservationService reservationService,
        ReceiptService receiptService)
    {
        _authService = authService;
        _auditService = auditService;
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
            if (_authService.CurrentUser == null)
            {
                if (!ShowLoginScreen())
                    return;
            }

            ShowMainMenu();
        }
    }

    private bool ShowLoginScreen()
    {
        Console.WriteLine("\n=== АВТОРИЗАЦІЯ В СИСТЕМІ ЗАКЛАДУ ХАРЧУВАННЯ ===");
        Console.WriteLine("Тестові облікові записи: admin / admin123 або waiter / 1234");
        Console.WriteLine("1. Увійти в систему");
        Console.WriteLine("0. Вийти з програми");
        Console.Write("Оберіть дію: ");

        string? choice = Console.ReadLine();
        if (choice == "0")
            return false;

        if (choice != "1")
        {
            Console.WriteLine("Невірний вибір.");
            return true;
        }

        Console.Write("Логін: ");
        string username = Console.ReadLine() ?? "";
        Console.Write("Пароль: ");
        string password = Console.ReadLine() ?? "";

        try
        {
            var user = _authService.Login(username, password);
            _auditService.Log(user, "Вхід у систему");
            Console.WriteLine($"\nВітаємо, {user.FullName}! Ваша роль: {FormatRole(user.Role)}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка входу: {ex.Message}");
        }

        return true;
    }

    private void ShowMainMenu()
    {
        var user = _authService.CurrentUser!;
        bool isAdmin = user.Role == UserRole.Admin;

        Console.WriteLine($"\n=== СИСТЕМА УПРАВЛІННЯ ЗАКЛАДОМ (Етап 6) | Користувач: {user.FullName} [{FormatRole(user.Role)}] ===");
        Console.WriteLine("1. Переглянути меню (з фільтром за категоріями та пошуком)");
        Console.WriteLine("2. Бронювання столиків");
        Console.WriteLine("3. Відкрити новий чек");
        Console.WriteLine("4. Додати замовлення у відкритий чек (з коментарем на кухню)");
        Console.WriteLine("5. Змінити статус замовлення");
        Console.WriteLine("6. Пересадити гостей на інший столик");
        Console.WriteLine("7. Застосувати знижку до чеку");
        Console.WriteLine("8. Закрити чек");
        Console.WriteLine("9. Переглянути всі чеки та історію статусів");

        if (isAdmin)
        {
            Console.WriteLine("--- Панель Адміністратора ---");
            Console.WriteLine("10. Управління меню (додавання/редагування/видалення)");
            Console.WriteLine("11. Управління столиками");
            Console.WriteLine("12. Управління офіціантами");
            Console.WriteLine("13. Управління обліковими записами користувачів");
            Console.WriteLine("14. Аналітика: виручка, офіціанти, популярні страви та експорт звіту");
            Console.WriteLine("15. Журнал дій користувачів (Audit Log)");
        }

        Console.WriteLine("99. Вийти з облікового запису (Змінити користувача)");
        Console.WriteLine("0. Завершити роботу програми");
        Console.Write("Оберіть дію: ");

        string? choice = Console.ReadLine();

        try
        {
            switch (choice)
            {
                case "1":
                    BrowseMenuUI();
                    break;
                case "2":
                    ManageReservations();
                    break;
                case "3":
                    OpenReceiptUI();
                    break;
                case "4":
                    AddOrderToReceiptUI();
                    break;
                case "5":
                    ChangeOrderStatusUI();
                    break;
                case "6":
                    ChangeReceiptTableUI();
                    break;
                case "7":
                    ApplyDiscountUI();
                    break;
                case "8":
                    CloseReceiptUI();
                    break;
                case "9":
                    ShowReceipts();
                    break;
                case "10":
                    _authService.EnsureAdmin();
                    ManageMenu();
                    break;
                case "11":
                    _authService.EnsureAdmin();
                    ManageTables();
                    break;
                case "12":
                    _authService.EnsureAdmin();
                    ManageWaiters();
                    break;
                case "13":
                    _authService.EnsureAdmin();
                    ManageUsers();
                    break;
                case "14":
                    _authService.EnsureAdmin();
                    ShowAnalyticsReportUI();
                    break;
                case "15":
                    _authService.EnsureAdmin();
                    ShowAuditLogUI();
                    break;
                case "99":
                    _auditService.Log(user, "Вихід із облікового запису");
                    _authService.Logout();
                    Console.WriteLine("Ви вийшли з облікового запису.");
                    break;
                case "0":
                    _auditService.Log(user, "Завершення роботи програми");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Невірний пункт меню або недостатньо прав доступу.");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка: {ex.Message}");
        }
    }

    private void BrowseMenuUI()
    {
        Console.WriteLine("\n--- ПЕРЕГЛЯД МЕНЮ ---");
        Console.WriteLine("1. Показати все меню");
        Console.WriteLine("2. Фільтр за категорією");
        Console.WriteLine("3. Пошук страви за назвою");
        Console.Write("Оберіть варіант: ");

        string? choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                ShowMenuItems();
                break;
            case "2":
                var cat = SelectCategory();
                if (cat == null) return;
                var filtered = _menuService.GetByCategory(cat.Value);
                if (filtered.Count == 0)
                {
                    Console.WriteLine($"У категорії «{FormatCategory(cat.Value)}» поки немає страв.");
                    return;
                }
                Console.WriteLine($"\nКатегорія «{FormatCategory(cat.Value)}»:");
                for (int i = 0; i < filtered.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {filtered[i].Name} — {filtered[i].Price:F2} грн");
                }
                break;
            case "3":
                Console.Write("Введіть частину назви страви: ");
                string query = Console.ReadLine() ?? "";
                var found = _menuService.SearchByName(query);
                if (found.Count == 0)
                {
                    Console.WriteLine("Нічого не знайдено.");
                    return;
                }
                Console.WriteLine("\nРезультати пошуку:");
                for (int i = 0; i < found.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. [{FormatCategory(found[i].Category)}] {found[i].Name} — {found[i].Price:F2} грн");
                }
                break;
            default:
                Console.WriteLine("Невірний вибір.");
                break;
        }
    }

    private static MenuCategory? SelectCategory()
    {
        Console.WriteLine("Оберіть категорію:");
        Console.WriteLine("1. Закуски");
        Console.WriteLine("2. Перші страви");
        Console.WriteLine("3. Основні страви");
        Console.WriteLine("4. Десерти");
        Console.WriteLine("5. Напої");
        Console.Write("Ваш вибір: ");

        return Console.ReadLine() switch
        {
            "1" => MenuCategory.Starters,
            "2" => MenuCategory.Soups,
            "3" => MenuCategory.MainCourses,
            "4" => MenuCategory.Desserts,
            "5" => MenuCategory.Drinks,
            _ => null
        };
    }

    private void ManageUsers()
    {
        while (true)
        {
            Console.WriteLine("\n--- УПРАВЛІННЯ КОРИСТУВАЧАМИ СИСТЕМИ ---");
            Console.WriteLine("1. Список користувачів");
            Console.WriteLine("2. Зареєструвати нового користувача");
            Console.WriteLine("0. Назад");
            Console.Write("Оберіть дію: ");

            string? choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    foreach (var u in _authService.GetAllUsers())
                    {
                        Console.WriteLine($"- {u.Username} | {u.FullName} | Роль: {FormatRole(u.Role)}");
                    }
                    break;
                case "2":
                    Console.Write("Логін: ");
                    string login = Console.ReadLine() ?? "";
                    Console.Write("Пароль (мін. 4 символи): ");
                    string pass = Console.ReadLine() ?? "";
                    Console.Write("ПІБ: ");
                    string fullName = Console.ReadLine() ?? "";
                    Console.WriteLine("Роль: 1 — Офіціант, 2 — Адміністратор");
                    Console.Write("Оберіть роль: ");
                    UserRole role = Console.ReadLine() == "2" ? UserRole.Admin : UserRole.Waiter;

                    var created = _authService.RegisterUser(login, pass, fullName, role);
                    _auditService.Log(_authService.CurrentUser, $"Створено користувача '{created.Username}' ({FormatRole(role)})");
                    Console.WriteLine($"Користувача '{created.Username}' успішно зареєстровано.");
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Невірний пункт.");
                    break;
            }
        }
    }

    private void ShowAuditLogUI()
    {
        var logs = _auditService.GetRecent(25);
        Console.WriteLine("\n=== ОСТАННІ 25 ДІЙ У СИСТЕМІ ===");
        if (logs.Count == 0)
        {
            Console.WriteLine("Записів поки немає.");
            return;
        }

        foreach (var entry in logs)
        {
            Console.WriteLine($"[{entry.Timestamp:dd.MM.yyyy HH:mm:ss}] {entry.Username} ({entry.Role}): {entry.Action}");
        }
    }

    private void ManageMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- УПРАВЛІННЯ МЕНЮ (АДМІНІСТРАТОР) ---");
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
                    var cat = SelectCategory() ?? MenuCategory.MainCourses;
                    _menuService.AddItem(name, price, cat);
                    _auditService.Log(_authService.CurrentUser, $"Додано страву в меню: {name} ({price:F2} грн, {FormatCategory(cat)})");
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
                    var newCat = SelectCategory() ?? itemToEdit.Category;
                    _menuService.UpdateItem(itemToEdit.Id, newName, newPrice, newCat);
                    _auditService.Log(_authService.CurrentUser, $"Оновлено страву в меню: {newName} ({newPrice:F2} грн)");
                    Console.WriteLine("Позицію оновлено.");
                    break;
                case "4":
                    var itemToDelete = SelectMenuItem();
                    if (itemToDelete == null) break;

                    string deletedName = itemToDelete.Name;
                    _menuService.DeleteItem(itemToDelete.Id);
                    _auditService.Log(_authService.CurrentUser, $"Видалено страву з меню: {deletedName}");
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
                    _auditService.Log(_authService.CurrentUser, $"Додано столик №{number} на {capacity} місць");
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
                    _auditService.Log(_authService.CurrentUser, $"Додано офіціанта: {fullName}");
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
                            Console.WriteLine("Спочатку додайте столики у панелі адміністратора.");
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

                        _auditService.Log(_authService.CurrentUser, $"Створено бронь столика №{created.TableNumber} на {created.StartTime:dd.MM.yyyy HH:mm} для {created.CustomerName}");
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

                        var targetBooking = activeAll[bIdx - 1];
                        _reservationService.CancelReservation(targetBooking.Id);
                        _auditService.Log(_authService.CurrentUser, $"Скасовано бронь столика №{targetBooking.TableNumber} ({targetBooking.CustomerName})");
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
            Console.WriteLine($"{i + 1}. [{FormatCategory(items[i].Category)}] {items[i].Name} — {items[i].Price:F2} грн");
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
            Console.WriteLine("Немає вільних столиків.");
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
            Console.WriteLine("Список офіціантів порожній.");
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
        _auditService.Log(_authService.CurrentUser, $"Відкрито чек для столика №{receipt.TableNumber} (офіціант: {receipt.WaiterName})");
        Console.WriteLine($"Відкрито чек! Столик №{receipt.TableNumber} | Офіціант: {receipt.WaiterName} | ID: {receipt.Id}");
    }

    private void ChangeReceiptTableUI()
    {
        var receipt = SelectOpenReceipt();
        if (receipt == null) return;

        int oldTableNum = receipt.TableNumber;
        Console.WriteLine($"Поточний столик: №{oldTableNum}. Оберіть новий вільний столик:");
        var newTable = SelectFreeTable();
        if (newTable == null) return;

        _receiptService.ChangeReceiptTable(receipt.Id, newTable);
        _auditService.Log(_authService.CurrentUser, $"Пересаджено гостей зі столика №{oldTableNum} на столик №{newTable.Number}");
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
        _auditService.Log(_authService.CurrentUser, $"Застосовано знижку {discount}% до чеку столика №{receipt.TableNumber}");
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

        Console.Write("Коментар для кухні (необов'язково, натисніть Enter щоб пропустити): ");
        string note = Console.ReadLine() ?? "";

        if (_receiptService.AddOrderToReceipt(selectedReceipt.Id, selectedItem, quantity, note))
        {
            _auditService.Log(_authService.CurrentUser, $"Додано замовлення: {selectedItem.Name} x{quantity} (Столик №{selectedReceipt.TableNumber})");
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
            string noteInfo = string.IsNullOrWhiteSpace(o.KitchenNote) ? "" : $" (Примітка: {o.KitchenNote})";
            Console.WriteLine($"{i + 1}. {o.MenuItem.Name} x{o.Quantity}{noteInfo} — поточний статус: {FormatStatus(o.Status)}");
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
        _auditService.Log(_authService.CurrentUser, $"Змінено статус '{selectedOrder.MenuItem.Name}' на '{FormatStatus(newStatus.Value)}' (Столик №{selectedReceipt.TableNumber})");
        Console.WriteLine($"Статус замовлення «{selectedOrder.MenuItem.Name}» змінено на «{FormatStatus(newStatus.Value)}».");
    }

    private void CloseReceiptUI()
    {
        var selectedReceipt = SelectOpenReceipt();
        if (selectedReceipt == null) return;

        _receiptService.CloseReceipt(selectedReceipt.Id);
        _auditService.Log(_authService.CurrentUser, $"Закрито чек столика №{selectedReceipt.TableNumber} на суму {selectedReceipt.TotalAmount:F2} грн");
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
                    string note = string.IsNullOrWhiteSpace(o.KitchenNote) ? "" : $" | Коментар: «{o.KitchenNote}»";
                    Console.WriteLine($"  - {o.MenuItem.Name} x{o.Quantity} = {o.TotalPrice:F2} грн [{FormatStatus(o.Status)}]{note}");
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

        Console.Write("\nЗберегти цей звіт у текстовий файл? (y/n): ");
        if (Console.ReadLine()?.Trim().Equals("y", StringComparison.OrdinalIgnoreCase) == true)
        {
            string savedPath = _receiptService.ExportDailyReportToFile(targetDate);
            _auditService.Log(_authService.CurrentUser, $"Експортовано фінансовий звіт за {targetDate:dd.MM.yyyy}");
            Console.WriteLine($"Звіт збережено у файл: {savedPath}");
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

    private static string FormatRole(UserRole role) => role switch
    {
        UserRole.Admin => "Адміністратор",
        UserRole.Waiter => "Офіціант",
        _ => role.ToString()
    };

    private static string FormatCategory(MenuCategory category) => category switch
    {
        MenuCategory.Starters => "Закуски",
        MenuCategory.Soups => "Перші страви",
        MenuCategory.MainCourses => "Основні страви",
        MenuCategory.Desserts => "Десерти",
        MenuCategory.Drinks => "Напої",
        _ => category.ToString()
    };
}