public class FlightsLogic
{
    public static List<FlightModel> AvailableFlights = new();

    public static void AppendFlights()
    {
        AvailableFlights = FlightsAccess.LoadAll();
        FlightsAccess.WriteAll(AvailableFlights);
    }

    public List<FlightModel> GetAllFlights()
    {
        return AvailableFlights
            .Where(f => DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public FlightModel? GetFlightsById(int flightId)
    {
        return AvailableFlights
            .FirstOrDefault(f => f.FlightId == flightId);
        // .FirstOrDefault(f => f.FlightId == flightId && DateTime.Parse(f.DepartureTime) >= DateTime.Now);
    }

    public List<FlightModel> FilterFlightsByPriceUp(string origin, string destination, string seatClass)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && f.Destination.Equals(destination) &&
                        DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .OrderBy(f =>
                f.SeatClassOptions.FirstOrDefault(option => option.SeatClass == seatClass)?.Price ?? int.MaxValue)
            .ToList();
    }

    public List<FlightModel> FilterFlightsByPriceDown(string origin, string destination, string seatClass)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && f.Destination.Equals(destination) &&
                        DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .OrderByDescending(f =>
                f.SeatClassOptions.FirstOrDefault(option => option.SeatClass == seatClass)?.Price ?? int.MinValue)
            .ToList();
    }

    public List<FlightModel> FilterFlightsByPriceRange(string origin, string destination, string seatClass,
        int minPrice, int maxPrice)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && f.Destination.Equals(destination) &&
                        f.SeatClassOptions.Any(option =>
                            option.SeatClass == seatClass && option.Price >= minPrice && option.Price <= maxPrice) &&
                        DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public List<FlightModel> FilterFlightsByDestination(string origin, string destination)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && f.Destination.Equals(destination) &&
                        DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public List<FlightModel> GetReturnFlights(FlightModel selectedFlight)
    {
        return AvailableFlights
            .Where(f => f.Origin == selectedFlight.Destination &&
                        f.Destination == selectedFlight.Origin &&
                        DateTime.Parse(f.DepartureTime) > DateTime.Parse(selectedFlight.ArrivalTime))
            .ToList();
    }

    public List<string> GetDestinationsByOrigin(string origin)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .Select(f => f.Destination)
            .Distinct()
            .ToList();
    }

    public List<string> GetAllDestinations()
    {
        return AvailableFlights
            .Where(f => DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .Select(f => f.Destination)
            .Distinct()
            .ToList();
    }

    public List<string> GetAllOrigins()
    {
        return AvailableFlights
            .Where(f => DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .Select(f => f.Origin)
            .Distinct()
            .ToList();
    }

    public List<FlightModel> FilterByDateRange(string origin, string destination, DateTime startDate, DateTime endDate)
    {
        return AvailableFlights
            .Where(flight => flight.Origin.Equals(origin) && flight.Destination.Equals(destination) &&
                             DateTime.Parse(flight.DepartureTime) >= startDate &&
                             DateTime.Parse(flight.DepartureTime) <= endDate &&
                             DateTime.Parse(flight.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public List<FlightModel> GetFlightsByOriginAndDestination(string origin, string destination)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && f.Destination.Equals(destination) &&
                        DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public List<FlightModel> FilterFlights(string origin, string destination, DateTime startDate, DateTime endDate)
    {
        return AvailableFlights
            .Where(flight => flight.Origin.Equals(origin) && flight.Destination.Equals(destination) &&
                             DateTime.Parse(flight.DepartureTime) >= startDate &&
                             DateTime.Parse(flight.DepartureTime) <= endDate &&
                             DateTime.Parse(flight.DepartureTime) >= DateTime.Now)
            .ToList();
    }

    public List<FlightModel> FilterFlightsByDate(string origin, string destination, DateTime date)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) &&
                        f.Destination.Equals(destination) &&
                        DateTime.Parse(f.DepartureTime).Date == date.Date)
            .ToList();
    }

    public bool AddFlight(FlightModel newFlight)
    {
        if (!IsFlightValid(newFlight))
            return false;

        var newId = AvailableFlights.Count > 0 ? AvailableFlights.Max(f => f.FlightId) + 1 : 1;
        newFlight.FlightId = newId;

        AvailableFlights.Add(newFlight);
        FlightsAccess.WriteAll(AvailableFlights);
        return true;
    }

    public bool UpdateFlight(FlightModel updatedFlight)
    {
        if (!IsFlightValid(updatedFlight))
            return false;

        var existingFlight = AvailableFlights.FirstOrDefault(f => f.FlightId == updatedFlight.FlightId);
        if (existingFlight == null)
            return false;

        var index = AvailableFlights.IndexOf(existingFlight);
        AvailableFlights[index] = updatedFlight;

        FlightsAccess.WriteAll(AvailableFlights);
        return true;
    }

    public bool DeleteFlight(int flightId)
    {
        var flight = AvailableFlights.FirstOrDefault(f => f.FlightId == flightId);
        if (flight == null)
            return false;

        AvailableFlights.Remove(flight);
        FlightsAccess.WriteAll(AvailableFlights);
        return true;
    }

    private bool IsFlightValid(FlightModel flight)
    {
        if (string.IsNullOrWhiteSpace(flight.Origin) ||
            string.IsNullOrWhiteSpace(flight.Destination) ||
            string.IsNullOrWhiteSpace(flight.FlightNumber) ||
            string.IsNullOrWhiteSpace(flight.DepartureTime) ||
            string.IsNullOrWhiteSpace(flight.ArrivalTime))
            return false;

        if (DateTime.Parse(flight.DepartureTime) >= DateTime.Parse(flight.ArrivalTime))
            return false;

        if (flight.SeatClassOptions == null || !flight.SeatClassOptions.Any())
            return false;

        return true;
    }
}