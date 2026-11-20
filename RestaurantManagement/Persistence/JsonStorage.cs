using System.Text.Encodings.Web;
using System.Text.Json;

namespace RestaurantManagement.Persistence;

public class JsonStorage<T> where T : new()
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public JsonStorage(string fileName)
    {
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
    }

    public void Save(T data)
    {
        string json = JsonSerializer.Serialize(data, _options);
        File.WriteAllText(_filePath, json);
    }

    public T Load()
    {
        if (!File.Exists(_filePath))
            return new T();

        string json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new T();

        return JsonSerializer.Deserialize<T>(json, _options) ?? new T();
    }
}