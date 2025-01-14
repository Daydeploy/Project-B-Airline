public class BookingLogic
{
    private static readonly Random random = new Random();

    private static readonly List<FlightModel> _flights = FlightsAccess.LoadAll();
    private static readonly List<AccountModel> _accounts = AccountsAccess.LoadAll();
    private static readonly List<BookingModel> _bookings = BookingAccess.LoadAll();
    
    public static bool HasInsurance { get; set; }	

    public static List<BookingModel> GetBookingsForFlight(int flightId)
    {
        return _bookings
            .Where(booking => booking.FlightId == flightId)
            .ToList();
    }

    public static List<BookingModel> GetBookingsForUser(int userId)
    {
        return _bookings
            .Where(booking => booking.UserId == userId)
            .ToList();
    }

    public static BookingModel CreateBooking(int userId, int flightId, List<PassengerModel> passengerDetails, List<PetModel> petDetails, bool includeInsurance = false, 
        bool isPrivateJet = false, string jetType = null)
    {
        int bookingId = GenerateBookingId();
        int totalPrice = 0;

        if (isPrivateJet && jetType != null)
        {
            var privateJetPrices = new Dictionary<string, int>
            {
                { "Bombardier Learjet 75", 15000 },
                { "Bombardier Global 8280", 25000 }
            };

            totalPrice = privateJetPrices.GetValueOrDefault(jetType, 0);
            if (totalPrice == 0) return null;
        }
        else
        {
            var flight = _flights.FirstOrDefault(f => f.FlightId == flightId);
            if (flight == null) return null;

            if (includeInsurance){
                HasInsurance = includeInsurance;
            }
            totalPrice = CalculateTotalPrice(flight.Destination, passengerDetails, includeInsurance);
        }

        List<PassengerModel> passengers = passengerDetails.Select(p => 
            new PassengerModel(p.Name, p.SeatNumber, p.HasCheckedBaggage, p.HasPet, p.PetDetails, p.SpecialLuggage)
            {
                NumberOfBaggage = p.NumberOfBaggage,
                ShopItems = p.ShopItems
            }).ToList(); 

        BookingModel newBooking = new BookingModel(bookingId, userId, flightId, totalPrice, passengers, petDetails);
        if (isPrivateJet)
        {
            newBooking.PlaneType = jetType;
            _bookings.Add(newBooking);
            BookingAccess.WriteAll(_bookings);
        }
        return newBooking;
    }

    public static bool SaveBooking(BookingModel booking)
    {
        if (booking == null) return false;
        _bookings.Add(booking);
        BookingAccess.WriteAll(_bookings);
        return true;
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

    public static (int userId, bool success) GetUserId(int id)
    {
        var account = _accounts.FirstOrDefault(x => x.Id.Equals(id));
        return account != null
            ? (account.Id, true)
            : (0, false);
    }

    public static (int flightId, bool success) GetFlightId(string destination)
    {
        var flight = _flights.FirstOrDefault(f =>
            f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase));
        return flight != null
            ? (flight.FlightId, true)
            : (0, false);
    }

    public static int CalculateTotalPrice(string destination, List<PassengerModel> passengers, bool includeInsurance = false)
    {
        var flight = _flights.FirstOrDefault(f => f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase));
        if (flight == null || flight.SeatClassOptions == null) return 0;
    
        const int BAGGAGE_PRICE = 30;
        const int CABIN_PET_FEE = 50;
        const int CARGO_PET_FEE = 30;
        const int INSURANCE_FEE = 10;
    
        var total = passengers.Sum(p =>
        {
            int passengerTotal = 0;
    
            var seatClass = GetSeatClass(p.SeatNumber);
            var seatOption = flight.SeatClassOptions
                .FirstOrDefault(so => so.SeatClass.Equals(seatClass, StringComparison.OrdinalIgnoreCase));
            
            if (seatOption != null)
            {
                passengerTotal += seatOption.Price;
            }
    
            if (p.HasCheckedBaggage && p.NumberOfBaggage > 1) // eerste tas gratis
            {
                passengerTotal += BAGGAGE_PRICE * (p.NumberOfBaggage - 1);
            }
    
            if (p.HasPet && p.PetDetails != null)
            {
                foreach (var pet in p.PetDetails)
                {
                    passengerTotal += pet.StorageLocation == "Cabin" ? CABIN_PET_FEE : CARGO_PET_FEE;
                }
            }
    
            if (p.ShopItems?.Any() == true)
            {
                passengerTotal += p.ShopItems.Sum(item => (int)item.Price);
            }
    
            return passengerTotal;
        });
    
        if (includeInsurance)
        {
            total += INSURANCE_FEE * passengers.Count;
        }
        
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

    public static List<BookingModel> GetAvailableCheckInBookings(int userId)
    {
        var flightsLogic = new FlightsLogic();
        return _bookings
            .Where(b => b.UserId == userId)
            .Where(b => !b.IsCheckedIn)
            .Where(b =>
            {
                var flight = flightsLogic.GetFlightsById(b.FlightId);
                return flight != null && DateTime.Parse(flight.DepartureTime) >= DateTime.Now;
            })
            .ToList();
    }

    public static bool CheckInBooking(int bookingId)
    {
        var booking = _bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking == null || booking.IsCheckedIn)
        {
            return false;
        }

        booking.IsCheckedIn = true;
        BookingAccess.WriteAll(_bookings);
        return true;
    }

    public static (bool success, string message) TryCheckIn(int bookingId)
    {
        var booking = _bookings.FirstOrDefault(b => b.BookingId == bookingId);
        if (booking == null)
        {
            return (false, "Booking not found.");
        }

        if (booking.IsCheckedIn)
        {
            return (false, "Booking is already checked in.");
        }

        var flightsLogic = new FlightsLogic();
        var flight = flightsLogic.GetFlightsById(booking.FlightId);
        if (flight == null)
        {
            return (false, "Flight not found.");
        }

        if (DateTime.Parse(flight.DepartureTime) < DateTime.Now)
        {
            return (false, "Flight has already departed.");
        }

        booking.IsCheckedIn = true;
        BookingAccess.WriteAll(_bookings);
        return (true, "Successfully checked in.");
    }
}