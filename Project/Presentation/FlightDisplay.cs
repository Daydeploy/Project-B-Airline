using System;
using System.Collections.Generic;

static class FlightDisplay
{
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

        foreach (var flight in flights)
        {
            DisplayFlightDetails(flight);
        }

        Console.WriteLine(new string('─', Console.WindowWidth - 1));
    }

    // Draws the header for the flight table
    public static void DrawTableHeader()
    {
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(
            $"{"Flight ID",-10} {"Route",-30} {"Departure",-18} {"Arrival",-18} {"Duration",-12} {"Prices (EUR)"}");
        Console.ResetColor();
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
    }

    // Displays details of a single flight in a formatted manner
    public static void DisplayFlightDetails(FlightModel flight)
    {
        DateTime departureDateTime = DateTime.Parse(flight.DepartureTime);
        DateTime arrivalDateTime = DateTime.Parse(flight.ArrivalTime);
        TimeSpan duration = arrivalDateTime - departureDateTime;

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

        string durationStr = $"{duration.Hours}h {duration.Minutes}m";
        Console.Write($"{durationStr,-12} ");

        string currentSeason = GetCurrentSeason();

        foreach (var seatOption in flight.SeatClassOptions)
        {
            double seasonalPrice = seatOption.Price * 
                (currentSeason == "summer" ? seatOption.SeasonalMultiplier.Summer : seatOption.SeasonalMultiplier.Winter);
            double totalPrice = seasonalPrice * (1 + flight.Taxes.Country) +
                flight.Taxes.Airport[flight.OriginCode] +
                flight.Taxes.Airport[flight.DestinationCode];

            Console.ForegroundColor = GetPriceColor(seatOption.SeatClass);
            Console.Write($"{seatOption.SeatClass}: {totalPrice:F2} ");
            Console.ResetColor();
        }

        Console.WriteLine();
    }


    // Draws the header for the bookings table
    public static void DrawBookingsTableHeader()
    {
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("BOOKED FLIGHTS DETAILS");
        Console.ResetColor();
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
    }

    // Displays details for a specific booking
        public static void DisplayBookingDetails(BookingModel booking, FlightModel flight)
    {
        if (booking == null) // anders krijg je een NullReferenceException 
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
    
        try
        {
            DateTime departureDateTime = DateTime.Parse(flight.DepartureTime);
            DateTime arrivalDateTime = DateTime.Parse(flight.ArrivalTime);
            TimeSpan duration = arrivalDateTime - departureDateTime;
    
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

            Console.WriteLine("Purchased Entertainment:" );
            if (booking.Entertainment?.Any() == true)
            {
                foreach (var entertainment in booking.Entertainment)
                {
                    Console.WriteLine($" - {entertainment.Name} | Price: {entertainment.Cost} EUR");
                }
            }
            else
            {
                Console.WriteLine("No entertainment purchased");
            }
            
            Console.WriteLine(new string('-', 30));
            if (booking.Passengers?.Any() == true)
            {   
                Console.WriteLine("\nPurchased items:\n");
                foreach (var passenger in booking.Passengers)
                {   
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    if (passenger.ShopItems?.Any() == true)
                    {
                        foreach (var item in passenger.ShopItems)
                        {   
                            if(item != null)
                            {
                                Console.WriteLine($" - {item.Name} | Price: {item.Price} EUR");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No items purchased");
                    }
                    Console.ResetColor();
    
                    if (!string.IsNullOrEmpty(passenger.SeatNumber))
                    {
                        var seatClass = new SeatSelectionUI().GetSeatClass(passenger.SeatNumber, flight.PlaneType);
                        var basePrice = flight.SeatClassOptions?
                            .FirstOrDefault(so => so.SeatClass.Equals(seatClass, StringComparison.OrdinalIgnoreCase))
                            ?.Price ?? 0;
                        
                        Console.WriteLine($"\nBase ticket price for {passenger.Name} ({seatClass}): {basePrice:F2} EUR");
                    }
                }
            }
    
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Total Price including purchased items and taxes: {booking.TotalPrice} EUR");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error displaying booking {booking.BookingId}: {ex.Message}");
            Console.ResetColor();
        }
    }
    

    // Displays details of each passenger in a booking
    public static void DisplayPassengerDetails(List<PassengerModel> passengers)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("\nPassengers:");
        Console.ResetColor();

        foreach (var passenger in passengers)
        {
            Console.Write($"• {passenger.Name}");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write($" | Seat: {passenger.SeatNumber}");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write($" | Checked Baggage: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");

            if (!string.IsNullOrEmpty(passenger.SpecialLuggage))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($" | Special Luggage: {passenger.SpecialLuggage}");
            }

            if (passenger.HasPet)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write(
                    $" | Pet: {passenger.PetDetails.Type} ({passenger.PetDetails.Weight}kg, {passenger.PetDetails.StorageLocation})");
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
                {
                    Console.WriteLine($"  ◈ {item.Name} ({item.Price:F2} EUR)");
                }
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
        return (currentMonth >= 6 && currentMonth <= 8) ? "summer" : "winter";
    }

}