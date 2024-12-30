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
            return new List<BookingModel>();
        }
    }

    public static BookingModel CreateBooking(int userId, int flightId, List<PassengerModel> passengerDetails, 
        List<PetModel> petDetails, bool isPrivateJet = false, string jetType = null)
    {
        int bookingId = GenerateBookingId();
        int totalPrice;

        if (isPrivateJet)
        {
            var privateJetPrices = new Dictionary<string, int>
            {
                { "Bombardier Learjet 75", 15000 },
                { "Bombardier Global 8280", 25000 }
            };

            totalPrice = privateJetPrices[jetType];
        }
        else
        {
            var flight = _flights.FirstOrDefault(f => f.FlightId == flightId);
            totalPrice = CalculateTotalPrice(flight.Destination, passengerDetails);
            
            foreach (var pet in petDetails)
            {
                totalPrice += (int)PetDataAccess.GetPetFees(pet.Type, pet.SeatingLocation);
            }
        }

        List<PassengerModel> passengers = passengerDetails
            .Select(p => new PassengerModel(
                p.Name, 
                p.SeatNumber, 
                p.HasCheckedBaggage,
                p.HasPet,
                p.PetDetails,
                p.SpecialLuggage))
            .ToList();

        BookingModel newBooking = new BookingModel(bookingId, userId, flightId, totalPrice, passengers, petDetails);
        if (isPrivateJet)
        {
            newBooking.PlaneType = jetType;
        }
        
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
    
        const int baggagePrice = 30;
    
        var total = passengers.Sum(p =>
        {
            var seatClass = GetSeatClass(p.SeatNumber);
            
            var basePrice = flight.SeatClassOptions
                .FirstOrDefault(so => so.SeatClass.Equals(seatClass, StringComparison.OrdinalIgnoreCase))
                ?.Price ?? 0;
    
            var totalPrice = basePrice + (p.HasCheckedBaggage ? baggagePrice : 0);
            return totalPrice;
        });
    
        // Ensure changes are saved to json
        BookingAccess.WriteAll(_bookings);
        return total;
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