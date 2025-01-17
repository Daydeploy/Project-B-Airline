internal static class FlightsAccess
{
    private static readonly string _filePath =
        Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"DataSources/flights.json"));

    private static readonly GenericJsonAccess<FlightModel> _flightAccess = new(_filePath);

    public static List<FlightModel> LoadAll()
    {
        return _flightAccess.LoadAll();
    }

    public static void WriteAll(List<FlightModel> flights)
    {
        _flightAccess.WriteAll(flights);
    }
}