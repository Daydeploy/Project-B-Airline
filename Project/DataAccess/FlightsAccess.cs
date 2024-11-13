static class FlightsAccess
{
    private static string _filePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/flights.json"));
    private static GenericJsonAccess<FlightModel> _flightAccess = new GenericJsonAccess<FlightModel>(_filePath);

    public static List<FlightModel> LoadAll()
    {
        return _flightAccess.LoadAll();
    }

    public static void WriteAll(List<FlightModel> flights)
    {
        _flightAccess.WriteAll(flights);
    }
}