using System.Text.Json;

static class BookingAccess
{
    static string path = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/bookings.json"));


    public static List<BookingModel> LoadAll()
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<BookingModel>>(json);
    }


    public static void WriteAll(List<BookingModel> bookings)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(bookings, options);
        File.WriteAllText(path, json);
    }
}