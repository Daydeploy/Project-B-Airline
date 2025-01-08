using System.Collections.Generic;
using System.Linq;

public interface IAirportService
{
    List<AirportModel> GetAvailableAirports();
    string GetAvailableServices(AirportModel airport);
    string GetAirportDescription(AirportModel airport);
}

public class AirportServiceLogic : IAirportService
{
    private List<AirportModel> _airports;

    public AirportServiceLogic(List<AirportModel> airports)
    {
        _airports = airports;
    }

    public List<AirportModel> GetAvailableAirports()
    {
        return _airports.Where(a => a.Country == "NL").ToList();
    }

    public string GetAvailableServices(AirportModel airport)
    {
        return "Helicopter Transfer, VIP Lounge, Concierge Service";
    }

    public string GetAirportDescription(AirportModel airport)
    {
        return $"Experience luxury at its finest at {airport.Name}. " +
               $"Our world-class facilities and exclusive services ensure an unforgettable journey. " +
               $"From VIP lounges to personalized concierge, we redefine air travel.";
    }
}