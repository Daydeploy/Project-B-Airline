using System.Text.Json;

public class GenericJsonAccess<T>
{
    private readonly string _filePath;

    public GenericJsonAccess(string filepath)
    {
        _filePath = filepath;
    }

    public List<T> LoadAll()
    {
        var json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<T>>(json);
    }

    public void WriteAll(List<T> list)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(list, options);
        File.WriteAllText(_filePath, json);
    }
}