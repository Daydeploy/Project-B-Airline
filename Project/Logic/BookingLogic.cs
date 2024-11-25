public class BookingLogic
{
    private static readonly Random random = new Random();

    private static readonly List<FlightModel> _flights = FlightsAccess.LoadAll();
    private static readonly List<AccountModel> _accounts = AccountsAccess.LoadAll();
    private static readonly List<BookingModel> _bookings = BookingAccess.LoadAll();

    public static List<BookingModel> GetBookingsForFlight(int flightId)
    {
        try
        {
            return _bookings
                .Where(booking => booking.FlightId == flightId)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading bookings for flight {flightId}: {ex.Message}");
            return new List<BookingModel>();
        }
    }

    public static List<BookingModel> GetBookingsForUser(int userId)
    {
        try
        {
            return _bookings
                .Where(booking => booking.UserId == userId)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading bookings for user {userId}: {ex.Message}");
            return new List<BookingModel>();
        }
    }

    public static BookingModel CreateBooking(int userId, int flightId, List<PassengerModel> passengerDetails, List<PetModel> petDetails)
    {
        // Generate a new booking ID
        int bookingId = GenerateBookingId();
        
        // Get the flight to calculate price
        var flight = _flights.FirstOrDefault(f => f.FlightId == flightId) 
            ?? throw new Exception("Flight not found");
        
        // Calculate total price including pets
        int totalPrice = CalculateTotalPrice(flight.Destination, passengerDetails);
        foreach (var pet in petDetails)
        {
            totalPrice += (int)PetDataAccess.GetPetFees(pet.Type, pet.SeatingLocation); // ik weet niet of locatie correct is
        }

        List<PassengerModel> passengers = passengerDetails
            .Select(p => new PassengerModel(p.Name, p.SeatNumber, p.HasCheckedBaggage))
            .ToList();

        BookingModel newBooking = new BookingModel(bookingId, userId, flightId, totalPrice, passengers, petDetails);
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
        return _accounts.FirstOrDefault(x => x.Id.Equals(id))?.Id 
            ?? throw new Exception("User not found");
    }

    public static int GetFlightId(string destination)
    {
        return _flights.FirstOrDefault(f => f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase))?.FlightId 
            ?? throw new Exception("Flight not found");
    }

    private static int CalculateTotalPrice(string destination, List<PassengerModel> passengers)
    {
        var flight = _flights.FirstOrDefault(f => f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase));
        if (flight == null || flight.SeatClassOptions == null) return 0;
    
        int baggagePrice = 30; // Additional price for checked baggage
    
        return passengers.Sum(p =>
        {
            // Get seat class based on row number
            var seatClass = GetSeatClass(p.SeatNumber);
            
            // Find matching price from flight's SeatClassOptions
            var basePrice = flight.SeatClassOptions
                .FirstOrDefault(so => so.Class.Equals(seatClass, StringComparison.OrdinalIgnoreCase))
                ?.Price ?? 0;

            return basePrice + (p.HasCheckedBaggage ? baggagePrice : 0);
        });
    }
    
    private static string GetSeatClass(string seatNumber)
    {
        // Extract row number from seat number (voorbeeld "15A" -> 15)
        int row = int.Parse(new string(seatNumber.Where(char.IsDigit).ToArray()));
        
        // Class based on row number
        if (row <= 3) return "First";
        if (row <= 8) return "Business";
        return "Economy";
    }

    public bool ModifyBooking(int flightId, int passengerId, BookingDetails newDetails)
    {
        var booking = _bookings.FirstOrDefault(b => b.FlightId == flightId);
        if (booking == null || booking.Passengers == null || passengerId >= booking.Passengers.Count)
        {
            return false;
        }

        var passenger = booking.Passengers[passengerId];
        passenger.SeatNumber = newDetails.SeatNumber;
        passenger.HasCheckedBaggage = newDetails.HasCheckedBaggage;

        // Recalculate total price after modification
        booking.TotalPrice = CalculateTotalPrice(
            _flights.First(f => f.FlightId == flightId).Destination, 
            booking.Passengers
        );

        BookingAccess.WriteAll(_bookings);
        return true;
    }
}