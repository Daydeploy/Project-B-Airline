internal static class FlightDisplay
{
    private static readonly Dictionary<string, int> MaxCargoWeight = new()
    {
        { "Boeing 737", 35000 },
        { "Boeing 787", 50000 },
        { "Airbus A330", 75000 }
    };

    private static (int usedWeight, int maxWeight) GetFlightWeightInfo(FlightModel flight)
    {
        IBookingAccess _bookingAccess = new BookingAccess();
        var bookings = _bookingAccess.LoadAll()
            .Where(b => b.FlightId == flight.FlightId)
            .ToList();

        var usedWeight = 0;
        foreach (var booking in bookings)
        {
            usedWeight += booking.Passengers.Count(p => p.HasCheckedBaggage) * 23; //23kg per bagage
            usedWeight += booking.Pets.Sum(p => (int)p.Weight);
            usedWeight +=
                booking.Passengers.Count(p => !string.IsNullOrEmpty(p.SpecialLuggage)) *
                30; //30kg per special luggage voor berekening
        }

        var maxWeight = MaxCargoWeight.TryGetValue(flight.PlaneType, out var weight) ? weight : 35000;
        return (usedWeight, maxWeight);
    }

    // Displays a list of flights
    public static void DisplayFlights(List<FlightModel> flights)
    {
        if (!flights.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nNo flights found matching your criteria.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"\nFound {flights.Count} flights matching your criteria:\n");
        DrawTableHeader();

        foreach (var flight in flights) DisplayFlightDetails(flight);

        Console.WriteLine(new string('─', Console.WindowWidth - 1));
    }

    public static void DrawTableHeader()
    {
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(
            $"{"Flight ID",-10} {"Route",-50} {"Departure",-15} {"Arrival",-17} {"Duration",-11} {"Cargo (kg)",-10} {"Prices (EUR)"}");
        Console.ResetColor();
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
    }

    public static void DisplayFlightDetails(FlightModel flight)
    {
        var departureDateTime = DateTime.Parse(flight.DepartureTime);
        var arrivalDateTime = DateTime.Parse(flight.ArrivalTime);
        var duration = arrivalDateTime - departureDateTime;
        var (usedWeight, maxWeight) = GetFlightWeightInfo(flight);

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{flight.FlightId,-10} ");
        Console.ResetColor();

        Console.Write($"{flight.Origin} ");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("→");
        Console.ResetColor();
        Console.Write($" {flight.Destination,-22} ");

        Console.Write($"{departureDateTime:HH:mm dd MMM} ");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("→");
        Console.ResetColor();
        Console.Write($" {arrivalDateTime:HH:mm dd MMM} ");

        var durationStr = $"\t{duration.Hours}h {duration.Minutes}m";
        Console.Write($"{durationStr,-12} ");

        // Display cargo weight with color coding
        var weightPercentage = (double)usedWeight / maxWeight;
        Console.ForegroundColor = weightPercentage switch
        {
            >= 0.9 => ConsoleColor.Red, // over 90% capacity
            >= 0.7 => ConsoleColor.Yellow, // over 70% capacity
            _ => ConsoleColor.DarkGreen // onder 70% capacity
        };
        Console.Write($"{usedWeight}/{maxWeight,-7}  ");
        Console.ResetColor();

        var currentSeason = GetCurrentSeason();

        foreach (var seatOption in flight.SeatClassOptions)
        {
            var seasonalPrice = seatOption.Price *
                                (currentSeason == "summer"
                                    ? seatOption.SeasonalMultiplier.Summer
                                    : seatOption.SeasonalMultiplier.Winter);
            var totalPrice = (int)(seasonalPrice * (1 + flight.Taxes.Country) +
                                   flight.Taxes.Airport[flight.OriginCode] +
                                   flight.Taxes.Airport[flight.DestinationCode]);

            Console.ForegroundColor = GetPriceColor(seatOption.SeatClass);
            Console.Write($"{seatOption.SeatClass}: {totalPrice} ");
            Console.ResetColor();
        }

        Console.WriteLine();
    }


    public static void DrawBookingsTableHeader()
    {
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("BOOKED FLIGHTS DETAILS");
        Console.ResetColor();
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
    }

    public static void DisplayBookingDetails(BookingModel booking, FlightModel flight)
    {
        if (booking == null)
        {
            Console.WriteLine("Error: Invalid booking");
            return;
        }

        if (booking.FlightId == 0)
        {
            DisplayPrivateJetBookingDetails(booking);
            return;
        }

        if (flight == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Booking ID: {booking.BookingId} | ERROR: Flight details not found");
            Console.ResetColor();
            return;
        }

        if (!DateTime.TryParse(flight.DepartureTime, out var departureDateTime) ||
            !DateTime.TryParse(flight.ArrivalTime, out var arrivalDateTime))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Booking ID: {booking.BookingId} | ERROR: Invalid date format");
            Console.ResetColor();
            return;
        }

        var duration = arrivalDateTime - departureDateTime;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(
            $"Booking ID: {booking.BookingId} | Flight ID: {flight.FlightId} | Aircraft type: {flight.PlaneType}");
        Console.ResetColor();

        Console.Write($"Route: {flight.Origin} ");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("→");
        Console.ResetColor();
        Console.WriteLine($" {flight.Destination}");

        Console.Write($"Departure: {departureDateTime:HH:mm dd MMM} ");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("→");
        Console.ResetColor();
        Console.WriteLine($" Arrival: {arrivalDateTime:HH:mm dd MMM}");
        Console.WriteLine($"Duration: {duration.Hours}h {duration.Minutes}m");

        Console.WriteLine(new string('-', 30));
        Console.WriteLine("Purchased Entertainment:");
        if (booking.Entertainment?.Any() == true)
        {
            foreach (var entertainment in booking.Entertainment)
                if (entertainment != null)
                    Console.WriteLine($" - {entertainment.Name} | Price: {entertainment.Cost} EUR");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n\tNo entertainment purchased");
            Console.ResetColor();
        }

        Console.WriteLine(new string('-', 30));

        Console.WriteLine("Purchased Comfort Packages:");
        if (booking.ComfortPackages?.Any() == true)
        {
            foreach (var package in booking.ComfortPackages)
                if (package != null)
                    Console.WriteLine($" - {package.Name} | Price: {package.Cost} EUR");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\n\tNo comport packages purchased");
            Console.ResetColor();
        }

        Console.WriteLine(new string('-', 30));

        if (booking.Passengers?.Any() == true)
        {
            Console.WriteLine("\nPurchased items:\n");
            foreach (var passenger in booking.Passengers)
                if (passenger != null)
                    DisplayPassengerPurchases(passenger, flight);
        }

        decimal calculatedTotal = 0;
        Console.WriteLine("\nPrice Breakdown:");
        Console.WriteLine(new string('─', 50));

        foreach (var passenger in booking.Passengers)
        {
            if (passenger == null) continue;

            // Base ticket price
            var seatSelector = new SeatSelectionUI();
            var seatClass = seatSelector.GetSeatClass(passenger.SeatNumber, flight.PlaneType);
            var seatOption = flight.SeatClassOptions
                .FirstOrDefault(so => so.SeatClass.Equals(seatClass, StringComparison.OrdinalIgnoreCase));

            calculatedTotal += seatOption.Price;
            Console.WriteLine($"\nBase ticket price for {passenger.Name} ({seatClass}): {seatOption.Price:C}");

            if (passenger.HasCheckedBaggage)
            {
                var baggageCost = (passenger.NumberOfBaggage - 1) * 30m;
                calculatedTotal += baggageCost;
                Console.WriteLine($"Baggage fee ({passenger.NumberOfBaggage} pieces): {baggageCost:C}");
            }

            if (passenger.HasPet && passenger.PetDetails?.Any() == true)
                foreach (var pet in passenger.PetDetails)
                {
                    var petFee = pet.StorageLocation == "Cabin" ? 50m : 30m;
                    calculatedTotal += petFee;
                    Console.WriteLine($"Pet fee ({pet.Type} - {pet.StorageLocation}): {petFee:C}");
                }

            if (passenger.ShopItems?.Any() == true)
            {
                var shopTotal = passenger.ShopItems.Sum(item => item.Price);
                calculatedTotal += shopTotal;
                Console.WriteLine($"Shop items total: {shopTotal:C}");
            }
        }

        if (booking.Entertainment?.Any() == true)
        {
            var entertainmentTotal = booking.Entertainment.Sum(e => e.Cost);
            calculatedTotal += entertainmentTotal;
            Console.WriteLine($"\nEntertainment total: {entertainmentTotal:C}");
        }

        Console.WriteLine(new string('─', 50));
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Calculated Total: {booking.TotalPrice:C}");
        Console.ResetColor();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Total Price including purchased items and taxes: {booking.TotalPrice:C} EUR");
        Console.ResetColor();
    }

    private static void DisplayPassengerPurchases(PassengerModel passenger, FlightModel flight)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        if (passenger.ShopItems?.Any() == true)
        {
            foreach (var item in passenger.ShopItems)
                if (item != null)
                    Console.WriteLine($" - {item.Name} | Price: {item.Price} EUR");
        }
        else
        {
            Console.WriteLine("No items purchased");
        }

        Console.ResetColor();

        if (!string.IsNullOrEmpty(passenger.SeatNumber))
        {
            var seatSelectionUI = new SeatSelectionUI();
            var seatClass = seatSelectionUI.GetSeatClass(passenger.SeatNumber, flight.PlaneType);
            var seatOption = flight.SeatClassOptions?
                .FirstOrDefault(so => so.SeatClass.Equals(seatClass, StringComparison.OrdinalIgnoreCase));

            if (seatOption != null)
                Console.WriteLine($"\nBase ticket price for {passenger.Name} ({seatClass}): {seatOption.Price:F2} EUR");
        }
    }

    public static void DisplayPassengerDetails(List<PassengerModel> passengers, BookingModel booking)
    {
        Console.ForegroundColor = booking.FlightId == 0 ? ConsoleColor.Magenta : ConsoleColor.Cyan;
        Console.WriteLine("\nPassengers:");
        Console.ResetColor();

        foreach (var passenger in passengers)
        {
            Console.Write($"• {passenger.Name}");

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            if (booking.FlightId == 0)
                Console.Write($" | Suite: {passenger.SeatNumber}");
            else
                Console.Write($" | Seat: {passenger.SeatNumber}");

            if (booking.FlightId != 0)
            {
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                if (passenger.HasCheckedBaggage)
                    Console.Write($" | Baggage: {passenger.NumberOfBaggage} piece(s)");
                else
                    Console.Write(" | No Checked Baggage");
            }

            if (!string.IsNullOrEmpty(passenger.SpecialLuggage))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($" | Special Luggage: {passenger.SpecialLuggage}");
            }

            if (passenger.HasPet && passenger.PetDetails.Count != 0)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(" | Pets: ");
                var petDescriptions = passenger.PetDetails.Select(pet =>
                    $"{pet.Type} ({pet.Weight}kg, {pet.StorageLocation})");
                Console.Write(string.Join(", ", petDescriptions));
            }

            if (booking.FlightId != 0)
            {
                Console.ForegroundColor = booking.IsCheckedIn ? ConsoleColor.Green : ConsoleColor.Red;
                Console.Write($" | Check-in: {(booking.IsCheckedIn ? "Completed" : "Pending")}");
            }

            Console.WriteLine();
            Console.ResetColor();
        }
    }

    private static void DisplayPrivateJetBookingDetails(BookingModel booking)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("╔═ PRIVATE JET BOOKING ═╗");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Booking ID: {booking.BookingId}");
        Console.ResetColor();

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"Aircraft: {booking.PlaneType}");
        Console.ResetColor();

        // Display passenger list with luxury styling
        Console.WriteLine("\nPassenger Manifest:");
        foreach (var passenger in booking.Passengers)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"♦ {passenger.Name}");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($" | Suite: {passenger.SeatNumber}");
            Console.ResetColor();

            // Display any purchased items
            if (passenger.ShopItems.Any())
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                foreach (var item in passenger.ShopItems.Where(i => i != null))
                    Console.WriteLine($"  ◈ {item.Name} ({item.Price:F2} EUR)");

                Console.ResetColor();
            }
        }


        Console.WriteLine(new string('═', 30));
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Charter Price: {booking.TotalPrice:N2} EUR");
        Console.ResetColor();
        Console.WriteLine(new string('═', 30));
    }

    // Determines the color to display based on seat class
    private static ConsoleColor GetPriceColor(string seatClass)
    {
        return seatClass switch
        {
            "Economy" => ConsoleColor.Green,
            "Business" => ConsoleColor.Blue,
            "First" => ConsoleColor.Magenta,
            _ => ConsoleColor.Gray
        };
    }

    private static string GetCurrentSeason()
    {
        var currentMonth = DateTime.Now.Month;
        return currentMonth >= 6 && currentMonth <= 8 ? "summer" : "winter";
    }
}