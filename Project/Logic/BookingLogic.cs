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
        int bookingId = GenerateBookingId();
        
        var flight = _flights.FirstOrDefault(f => f.FlightId == flightId) 
            ?? throw new Exception("Flight not found");
        
        int totalPrice = CalculateTotalPrice(flight.Destination, passengerDetails);
        foreach (var pet in petDetails)
        {
            totalPrice += (int)PetDataAccess.GetPetFees(pet.Type, pet.SeatingLocation); // ik weet niet of locatie correct is
        }

        List<PassengerModel> passengers = passengerDetails
            .Select(p => new PassengerModel(
                p.Name, 
                p.SeatNumber, 
                p.HasCheckedBaggage,
                p.HasPet,
                p.PetDetails))
            .ToList();

        BookingModel newBooking = new BookingModel(bookingId, userId, flightId, totalPrice, passengers, petDetails);
        _bookings.Add(newBooking);
        BookingAccess.WriteAll(_bookings);
        return newBooking;
    }

    public static BookingModel CreatePrivateJetBooking(int userId, List<PassengerModel> passengerDetails, string jetType)
    {
    int bookingId = GenerateBookingId();
    int totalPrice = 2500 * passengerDetails.Count; // prijs voor nu op 2500 gezet x aantal passengerss

    var passengers = passengerDetails
        .Select(p => new PassengerModel(
            p.Name, 
            p.SeatNumber, 
            p.HasCheckedBaggage,
            p.HasPet,
            p.PetDetails))
        .ToList();

    BookingModel newBooking = new BookingModel(
        bookingId,
        userId, 
        0, // Flight ID staat gewoon op 0 nu
        totalPrice, 
        passengers, 
        new List<PetModel>(), // geen pets
        null, // er zijn voor nu geen comfort packages op private jets, want die zijn standaard al luxer
        jetType
    );

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
    
        int baggagePrice = 30;
    
        return passengers.Sum(p =>
        {
            var seatClass = GetSeatClass(p.SeatNumber);
            
            var basePrice = (int)(flight.SeatClassOptions
                .FirstOrDefault(so => so.SeatClass.Equals(seatClass, StringComparison.OrdinalIgnoreCase))
                ?.Price ?? 0);
    
            return basePrice + (p.HasCheckedBaggage ? baggagePrice : 0);
        });
    }
    
    private static string GetSeatClass(string seatNumber)
    {
        int row = int.Parse(new string(seatNumber.Where(char.IsDigit).ToArray()));
        
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

        booking.TotalPrice = CalculateTotalPrice(
            _flights.First(f => f.FlightId == flightId).Destination, 
            booking.Passengers
        );

        BookingAccess.WriteAll(_bookings);
        return true;
    }
}