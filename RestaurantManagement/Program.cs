using System.Text;
using RestaurantManagement.BusinessLogic;
using RestaurantManagement.Models;
using RestaurantManagement.Persistence;
using RestaurantManagement.Presentation;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var menuStorage = new JsonStorage<Menu>("menu.json");
var receiptStorage = new JsonStorage<List<Receipt>>("receipts.json");

var menuService = new MenuService(menuStorage);
var receiptService = new ReceiptService(receiptStorage);

var ui = new ConsoleUI(menuService, receiptService);
ui.Run();