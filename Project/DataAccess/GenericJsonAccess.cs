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
        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<T>>(json);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"File not found: {_filePath}");
            Console.WriteLine(ex.Message);
            return new List<T>();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to deserialize JSON data, for: {_filePath}");
            Console.WriteLine(ex.Message);
            return new List<T>();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error reading file: {_filePath}");
            Console.WriteLine(e.Message);
            return new List<T>();
        }
    }

    public void WriteAll(List<T> list)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(list, options);
            File.WriteAllText(_filePath, json);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Failed to deserialize JSON data for: {_filePath}");
            Console.WriteLine(ex.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error reading file: {_filePath}");
            Console.WriteLine(e.Message);
        }
    }
}