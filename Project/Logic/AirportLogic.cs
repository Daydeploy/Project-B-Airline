public class AirportLogic
{
    private readonly IAirportAccess _airportAccess;
    private List<AirportModel> _airports;

    public AirportLogic()
    {
        _airportAccess = ServiceLocator.GetAirportAccess();
        _airports = _airportAccess.LoadAll();
    }

    public List<AirportModel> GetAllAirports()
    {
        return _airports;
    }

    public AirportModel GetAirportById(int id)
    {
        return _airports.FirstOrDefault(a => a.AirportID == id);
    }

    public void AddAirport(AirportModel newAirport)
    {
        _airportAccess.AddAirport(newAirport);
        _airports = _airportAccess.LoadAll();
    }

    public bool UpdateAirport(AirportModel updatedAirport)
    {
        var existingAirport = _airports.FirstOrDefault(a => a.AirportID == updatedAirport.AirportID);
        if (existingAirport == null) return false;

        var index = _airports.IndexOf(existingAirport);
        _airports[index] = updatedAirport;

        _airportAccess.WriteAllAirports(_airports);
        _airports = _airportAccess.LoadAll();
        return true;
    }
}