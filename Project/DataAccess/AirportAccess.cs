using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

public static class AirportAccess
{
    private static string path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/airports.json"));

    public static List<AirportModel> LoadAllAirports()
    {
        string json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<AirportModel>>(json);
    }

    public static void WriteAllAirports(List<AirportModel> airports)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(airports, options);
        File.WriteAllText(path, json);
    }
}