using System.Text;
using RestaurantManagement.BusinessLogic;
using RestaurantManagement.Models;
using RestaurantManagement.Persistence;
using RestaurantManagement.Presentation;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var userStorage = new JsonStorage<List<User>>("users.json");
var auditStorage = new JsonStorage<List<AuditLogEntry>>("audit_log.json");
var menuStorage = new JsonStorage<Menu>("menu.json");
var tableStorage = new JsonStorage<List<Table>>("tables.json");
var waiterStorage = new JsonStorage<List<Waiter>>("waiters.json");
var reservationStorage = new JsonStorage<List<Reservation>>("reservations.json");
var receiptStorage = new JsonStorage<List<Receipt>>("receipts.json");

var authService = new AuthService(userStorage);
var auditService = new AuditService(auditStorage);
var menuService = new MenuService(menuStorage);
var tableService = new TableService(tableStorage);
var waiterService = new WaiterService(waiterStorage);
var reservationService = new ReservationService(reservationStorage, tableService);
var receiptService = new ReceiptService(receiptStorage, tableService);

var ui = new ConsoleUI(
    authService,
    auditService,
    menuService,
    tableService,
    waiterService,
    reservationService,
    receiptService);

ui.Run();