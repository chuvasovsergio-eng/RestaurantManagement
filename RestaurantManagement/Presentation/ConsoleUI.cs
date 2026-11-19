using RestaurantManagement.BusinessLogic;
using RestaurantManagement.Models;

namespace RestaurantManagement.Presentation;

public class ConsoleUI
{
    private readonly MenuService _menuService;
    private readonly ReceiptService _receiptService;

    public ConsoleUI(MenuService menuService, ReceiptService receiptService)
    {
        _menuService = menuService;
        _receiptService = receiptService;
    }

    public void Run()
    {
        while (true)
        {
            Console.WriteLine("\n=== СИСТЕМА УПРАВЛІННЯ ЗАКЛАДОМ ХАРЧУВАННЯ (Етап 1) ===");
            Console.WriteLine("1. Управління меню (CRUD)");
            Console.WriteLine("2. Відкрити новий чек");
            Console.WriteLine("3. Додати замовлення у відкритий чек");
            Console.WriteLine("4. Переглянути всі чеки");
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
                        OpenReceiptUI();
                        break;
                    case "3":
                        AddOrderToReceiptUI();
                        break;
                    case "4":
                        ShowReceipts();
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

    private void OpenReceiptUI()
    {
        var receipt = _receiptService.OpenReceipt();
        Console.WriteLine($"Відкрито новий чек! ID: {receipt.Id} | Час: {receipt.OpenedAt:dd.MM.yyyy HH:mm:ss}");
    }

    private void AddOrderToReceiptUI()
    {
        var openReceipts = _receiptService.GetOpenReceipts();
        if (openReceipts.Count == 0)
        {
            Console.WriteLine("Немає відкритих чеків. Спочатку відкрийте чек.");
            return;
        }

        Console.WriteLine("\nВідкриті чеки:");
        for (int i = 0; i < openReceipts.Count; i++)
        {
            Console.WriteLine($"{i + 1}. ID: {openReceipts[i].Id} (відкрито {openReceipts[i].OpenedAt:HH:mm:ss}, сума: {openReceipts[i].TotalAmount:F2} грн)");
        }

        Console.Write("Оберіть номер чеку: ");
        if (!int.TryParse(Console.ReadLine(), out int receiptIndex) || receiptIndex < 1 || receiptIndex > openReceipts.Count)
        {
            Console.WriteLine("Невірний номер чеку.");
            return;
        }

        var selectedReceipt = openReceipts[receiptIndex - 1];
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
            Console.WriteLine($"Додано: {selectedItem.Name} x{quantity} у чек {selectedReceipt.Id}.");
        }
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
            string status = r.IsOpen ? "Відкритий" : "Закритий";
            Console.WriteLine($"\nЧек ID: {r.Id} | Статус: {status} | Відкрито: {r.OpenedAt:dd.MM.yyyy HH:mm:ss}");
            if (r.Orders.Count == 0)
            {
                Console.WriteLine("  (Замовлень поки немає)");
            }
            else
            {
                foreach (var o in r.Orders)
                {
                    Console.WriteLine($"  - {o.MenuItem.Name} x{o.Quantity} = {o.TotalPrice:F2} грн");
                }
            }
            Console.WriteLine($"  Разом: {r.TotalAmount:F2} грн");
        }
    }
}