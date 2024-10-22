public class BookingLogic
{
    private static readonly Random random = new Random();

    private static readonly List<FlightModel> _flights = FlightsAccess.LoadAll();
    private static readonly List<AccountModel> _accounts = AccountsAccess.LoadAll();
    private static readonly List<BookingModel> _bookings = BookingAccess.LoadAll();

    public static BookingModel CreateBooking(int id, string destination, List<PassengerModel> passengerDetails)
    {

        // Generate a new booking ID
        int bookingId = GenerateBookingId();

        int userId = GetUserId(id);
        int flightId = GetFlightId(destination);
        int totalPrice = GetTotalPrice(destination, passengerDetails.Count);

        List<PassengerModel> passengers = passengerDetails.Select(p => new PassengerModel(p.Name, p.SeatNumber, p.HasCheckedBaggage)).ToList();

        BookingModel newBooking = new BookingModel(bookingId, userId, flightId, totalPrice, passengerDetails);
        _bookings.Add(newBooking);
        BookingAccess.WriteAll(_bookings);
        return newBooking;
    }

    private static int GenerateBookingId()
    {
        int bookingId;
        do
        {
            bookingId = random.Next(0, 9999);
        } while (_bookings.Any(i => i.BookingId.Equals(bookingId)));

        return bookingId;
    }

    public static int GetUserId(int id)
    {
        return _accounts.FirstOrDefault(x => x.Id.Equals(id)).Id;

    }

    public static int GetFlightId(string destination)
    {
        return _flights.FirstOrDefault(f => f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase)).FlightId;
    }

    public static string GetSeatNumber(string seat)
    {
        return $"{seat}";
    }

    public static int GetTotalPrice(string destination, int passengersCount)
    {
        int base_price = _flights.FirstOrDefault(f => f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase)).Price;
        return base_price * passengersCount;
    }
}