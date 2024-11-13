public static class SeatAccess
{
    public static readonly List<FlightModel> _flights = FlightsAccess.LoadAll();

    public static List<SeatModel> LoadAll()
    {
        List<SeatModel> allFlights = new List<SeatModel>();
        foreach (var flight in _flights)
        {
            SeatModel seatModel = LoadForFlight(flight.FlightId);
            if (seatModel != null)
            {
                allFlights.Add(seatModel);
            }
        }
        return allFlights;
    }

    public static void WriteAll(List<SeatModel> seats)
    {
        foreach (var seat in seats)
        {
            SaveForFlight(seat);
        }
    }

    public static string GetFilePathForFlight(int flightId)
    {
        string flightNumber = _flights.FirstOrDefault(f => f.FlightId == flightId).FlightNumber ?? "unknown";
        return System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, $@"DataSources/Flights/{flightId}-{flightNumber}.json"));
    }

    public static SeatModel LoadForFlight(int flightId)
    {
        string _filePath = GetFilePathForFlight(flightId);
        return new GenericJsonAccess<SeatModel>(_filePath).LoadAll().FirstOrDefault();
    }

    public static void SaveForFlight(SeatModel seatModel)
    {
        string _filePath = GetFilePathForFlight(seatModel.FlightId);
        GenericJsonAccess<SeatModel> seatAccess = new GenericJsonAccess<SeatModel>(_filePath);
        seatAccess.WriteAll(new List<SeatModel> { seatModel });
    }
}