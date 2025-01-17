public class AccountsAccess : BaseJsonAccess<AccountModel>, IAccountsAccess
{
    public AccountsAccess() : base(@"DataSources/accounts.json") { }
}

public class AirportAccess : BaseJsonAccess<AirportModel>, IAirportAccess
{
    public AirportAccess() : base(@"DataSources/airports.json") { }

    public bool WriteAllAirports(List<AirportModel> airports)
    {
        if (airports == null)
            return false;

        WriteAll(airports);
        return true;
    }

    public bool AddAirport(AirportModel newAirport)
    {
        if (newAirport == null)
            return false;

        var airports = LoadAll();
        if (airports.Any(a => a.AirportID == newAirport.AirportID))
            return false;

        airports.Add(newAirport);
        return WriteAllAirports(airports);
    }
}

public class BookingAccess : BaseJsonAccess<BookingModel>, IBookingAccess
{
    public BookingAccess() : base(@"DataSources/bookings.json") { }
}

public class FlightsAccess : BaseJsonAccess<FlightModel>, IFlightAccess
{
    public FlightsAccess() : base(@"DataSources/flights.json") { }
}