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
        Console.WriteLine($"{"Flight ID",-10} {"Route",-30} {"Departure",-18} {"Arrival",-18} {"Duration",-12} {"Prices (EUR)"}");
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

        foreach (var seatOption in flight.SeatClassOptions)
        {
            Console.ForegroundColor = GetPriceColor(seatOption.Class);
            Console.Write($"{seatOption.Class}: {seatOption.Price,4} ");
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
        DateTime departureDateTime = DateTime.Parse(flight.DepartureTime);
        DateTime arrivalDateTime = DateTime.Parse(flight.ArrivalTime);
        TimeSpan duration = arrivalDateTime - departureDateTime;

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Booking ID: {booking.BookingId} | Flight ID: {flight.FlightId} | Aircraft type: {flight.PlaneType}");
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
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Total Price: {booking.TotalPrice} EUR");
        Console.ResetColor();
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
            Console.WriteLine($" | Checked Baggage: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");
            Console.ResetColor();
        }
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
}