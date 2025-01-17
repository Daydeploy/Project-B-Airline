public static class AirportAccess
{
    private static readonly string _filePath =
        Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/airports.json"));

    private static readonly GenericJsonAccess<AirportModel> _airportAccess = new(_filePath);

    public static List<AirportModel> LoadAllAirports()
    {
        return _airportAccess.LoadAll();
    }

    public static bool WriteAllAirports(List<AirportModel> airports)
    {
        if (airports == null)
            return false;

        _airportAccess.WriteAll(airports);
        return true;
    }

    public static bool AddAirport(AirportModel newAirport)
    {
        if (newAirport == null)
            return false;

        var airports = LoadAllAirports();
        if (airports.Any(a => a.AirportID == newAirport.AirportID))
            return false;

        airports.Add(newAirport);
        return WriteAllAirports(airports);
    }
}