using System.Text.Json;

static class FlightsAccess
{
    static string path =
        System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/flights.json"));

    public static List<FlightModel> LoadAll()
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<FlightModel>>(json);
    }


    public static void WriteAll(List<FlightModel> flights)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(flights, options);
        File.WriteAllText(path, json);
    }
}