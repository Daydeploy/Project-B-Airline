public interface IDataAccess<T>
{
    string FileName { get; }
    List<T> LoadAll();
    void WriteAll(List<T> model);
}

public interface IAccountsAccess
{
    List<AccountModel> LoadAll();
    void WriteAll(List<AccountModel> accounts);
}

public interface IAirportAccess
{
    List<AirportModel> LoadAll();
    bool WriteAllAirports(List<AirportModel> airports);
    bool AddAirport(AirportModel newAirport);
}

public interface IBookingAccess
{
    List<BookingModel> LoadAll();
    void WriteAll(List<BookingModel> bookings);
}

public interface IFlightAccess
{
    List<FlightModel> LoadAll();
    void WriteAll(List<FlightModel> flights);
}