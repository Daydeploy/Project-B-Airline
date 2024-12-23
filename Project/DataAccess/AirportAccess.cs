public static class AirportAccess
{
    private static string _filePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/airports.json"));
    private static GenericJsonAccess<AirportModel> _airportAccess = new GenericJsonAccess<AirportModel>(_filePath);

    public static List<AirportModel> LoadAllAirports()
    {
        return _airportAccess.LoadAll();
    }

    public static void WriteAllAirports(List<AirportModel> airports)
    {
        _airportAccess.WriteAll(airports);
    }

    public static void AddAirport(AirportModel newAirport)
    {
        var airports = LoadAllAirports();
        if (!airports.Any(a => a.AirportID == newAirport.AirportID))
        {
            airports.Add(newAirport);
            _airportAccess.WriteAll(airports);
        }
        else
        {
            throw new Exception($"Airport with ID {newAirport.AirportID} already exists");
        }
    }
}