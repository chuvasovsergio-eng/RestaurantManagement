using RestaurantManagement.Models;
using RestaurantManagement.Persistence;

namespace RestaurantManagement.BusinessLogic;

public class ReservationService
{
    private readonly JsonStorage<List<Reservation>> _storage;
    private readonly TableService _tableService;
    private readonly List<Reservation> _reservations;

    public ReservationService(JsonStorage<List<Reservation>> storage, TableService tableService)
    {
        _storage = storage;
        _tableService = tableService;
        _reservations = _storage.Load();
    }

    public IReadOnlyList<Reservation> GetAll() => _reservations.OrderBy(r => r.StartTime).ToList();

    public IReadOnlyList<Reservation> GetActiveByDate(DateTime date) =>
        _reservations
            .Where(r => !r.IsCancelled && r.StartTime.Date == date.Date)
            .OrderBy(r => r.StartTime)
            .ToList();

    public Reservation CreateReservation(
        Guid tableId,
        string customerName,
        string customerPhone,
        int guestsCount,
        DateTime startTime,
        int durationMinutes = 120)
    {
        var table = _tableService.GetById(tableId)
            ?? throw new ArgumentException("Столик не знайдено.");

        if (guestsCount > table.Capacity)
            throw new InvalidOperationException($"Столик №{table.Number} вміщує максимум {table.Capacity} гостей.");

        if (startTime < DateTime.Now.AddMinutes(-5))
            throw new InvalidOperationException("Неможливо створити бронювання у минулому часі.");

        DateTime endTime = startTime.AddMinutes(durationMinutes);

        bool hasOverlap = _reservations.Any(r =>
            !r.IsCancelled
            && r.TableId == tableId
            && startTime < r.EndTime
            && endTime > r.StartTime);

        if (hasOverlap)
            throw new InvalidOperationException($"Столик №{table.Number} уже заброньований на цей проміжок часу.");

        var reservation = new Reservation
        {
            TableId = table.Id,
            TableNumber = table.Number,
            CustomerName = customerName,
            CustomerPhone = customerPhone,
            GuestsCount = guestsCount,
            StartTime = startTime,
            DurationMinutes = durationMinutes
        };

        _reservations.Add(reservation);
        _storage.Save(_reservations);
        return reservation;
    }

    public void CancelReservation(Guid reservationId)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId)
            ?? throw new ArgumentException("Бронювання не знайдено.");

        if (reservation.IsCancelled)
            throw new InvalidOperationException("Бронювання уже скасоване.");

        reservation.IsCancelled = true;
        _storage.Save(_reservations);
    }
}