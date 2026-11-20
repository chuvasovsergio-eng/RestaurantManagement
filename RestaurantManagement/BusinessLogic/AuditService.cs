using RestaurantManagement.Models;
using RestaurantManagement.Persistence;

namespace RestaurantManagement.BusinessLogic;

public class AuditService
{
    private readonly JsonStorage<List<AuditLogEntry>> _storage;
    private readonly List<AuditLogEntry> _logs;

    public AuditService(JsonStorage<List<AuditLogEntry>> storage)
    {
        _storage = storage;
        _logs = _storage.Load();
    }

    public void Log(User? user, string action)
    {
        var entry = new AuditLogEntry
        {
            Username = user?.Username ?? "system",
            Role = user?.Role.ToString() ?? "System",
            Action = action
        };

        _logs.Add(entry);
        _storage.Save(_logs);
    }

    public IReadOnlyList<AuditLogEntry> GetRecent(int count = 25) =>
        _logs.OrderByDescending(l => l.Timestamp).Take(count).ToList();
}