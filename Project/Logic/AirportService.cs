using System.Collections.Generic;
using System.Linq;

public class AirportService
{
    private List<AirportModel> _airports;

    public AirportService(List<AirportModel> airports)
    {
        _airports = airports;
    }

    public List<AirportModel> GetLuxuriousAirports()
    {
        return _airports.Where(a => a.Type == "Public" && IsAirportLuxurious(a)).ToList();
    }

    public List<AirportModel> GetLuxuriousPrivateAirstrips()
    {
        return _airports.Where(a => a.Type == "Private" && IsAirportLuxurious(a)).ToList();
    }

    public bool IsAirportLuxurious(AirportModel airport)
    {

        return airport.Name.Contains("Luxury") || airport.Name.Contains("VIP") || airport.Name.Contains("Executive");
    }

    public string GetAirportTransportationOptions(AirportModel airport)
    {
        return "Taxi, Shuttle, Limousine Service";
    }

    public string GetNearbyHotels(AirportModel airport)
    {
        return "Luxury Hotel A, Executive Suite B, VIP Resort C";
    }

    public string GetAdditionalServices(AirportModel airport)
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