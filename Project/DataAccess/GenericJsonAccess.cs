using System.Text.Json;

public class GenericJsonAccess<T>
{
    private string _filePath;

    public GenericJsonAccess(string filepath)
    {
        _filePath = filepath;
    }

    public List<T> LoadAll()
    {
        string json = File.ReadAllText(_filePath);
        return JsonSerializer.Deserialize<List<T>>(json);
    }

    public void WriteAll(List<T> list)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(list, options);
        File.WriteAllText(_filePath, json);
    }
}