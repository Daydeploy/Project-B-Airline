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
}