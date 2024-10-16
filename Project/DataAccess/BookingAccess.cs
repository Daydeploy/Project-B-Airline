using System.Text.Json;
using System.IO;

static class BookingAccess
{
    static string path = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/bookings.json"));


    public static List<BookingModel> LoadAll()
    {
        if (!File.Exists(path))
        {
            WriteAll(new List<BookingModel>());
        }

        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<BookingModel>>(json) ?? new List<BookingModel>();
    }


    public static void WriteAll(List<BookingModel> bookings)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(bookings, options);
        
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        
        File.WriteAllText(path, json);
    }
}
