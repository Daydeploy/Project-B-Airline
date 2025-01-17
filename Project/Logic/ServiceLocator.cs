public static class ServiceLocator
{
    private static readonly IAccountsAccess _accountsAccess;
    private static readonly IAirportAccess _airportAccess;
    private static readonly IBookingAccess _bookingAccess;
    private static readonly IFlightAccess _flightsAccess;

    static ServiceLocator()
    {
        _accountsAccess = new AccountsAccess();
        _airportAccess = new AirportAccess();
        _bookingAccess = new BookingAccess();
        _flightsAccess = new FlightsAccess();
    }

    public static IAccountsAccess GetAccountsAccess() => _accountsAccess;
    public static IAirportAccess GetAirportAccess() => _airportAccess;
    public static IBookingAccess GetBookingAccess() => _bookingAccess;
    public static IFlightAccess GetFlightsAccess() => _flightsAccess;
}