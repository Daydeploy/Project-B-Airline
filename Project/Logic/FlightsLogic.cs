public class FlightsLogic
{
    public static readonly Random random = new Random();
    public static List<FlightModel> AvailableFlights = new List<FlightModel>();

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

    public FlightModel GetFlightsById(int flightId)
    {
        return AvailableFlights
            .FirstOrDefault(f => f.FlightId == flightId && DateTime.Parse(f.DepartureTime) >= DateTime.Now);
    }

    public List<FlightModel> FilterFlightsByPriceUp(string origin, string destination, string seatClass)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && f.Destination.Equals(destination) &&
                        DateTime.Parse(f.DepartureTime) >= DateTime.Now)
            .OrderBy(f => f.SeatClassOptions.FirstOrDefault(option => option.SeatClass == seatClass)?.Price ?? int.MaxValue)
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

    public List<FlightModel> GetFlightsByOrigin(string origin)
    {
        return AvailableFlights
            .Where(f => f.Origin.Equals(origin) && DateTime.Parse(f.DepartureTime) >= DateTime.Now)
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
}