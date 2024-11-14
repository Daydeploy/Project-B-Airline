using System;
using System.Collections.Generic;
using System.Linq;

static class FlightManagement
{
    // Displays all available flights and provides options to filter or book
    public static void ShowAvailableFlights()
    {
        FlightsLogic flights = new FlightsLogic();
        var flightsList = flights.GetAllFlights().ToList();
        
        FlightDisplay.DisplayFlights(flightsList);

        Console.WriteLine("\nCommands:");
        Console.WriteLine("F - Filter flights");
        Console.WriteLine("B - Book a flight");
        Console.WriteLine("ESC - Go back");

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Escape)
                return;

            if (key.Key == ConsoleKey.F)
            {
                Menu.FilterFlightsByPriceUI();
                return;
            }

            if (key.Key == ConsoleKey.B)
            {
                Console.Clear();
                Console.WriteLine("Enter the Flight ID to book:");
                if (int.TryParse(Console.ReadLine(), out int flightId))
                {
                    BookFlight(flightId, flightsList);
                }
                else
                {
                    Console.WriteLine("Invalid Flight ID format.");
                    Console.WriteLine("Press BACKSPACE to return to the menu...");
                }

                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }
        }
    }

    // Displays the booked flights for the user
    public static void ViewBookedFlights(int userId)
    {
        var bookings = BookingAccess.LoadAll().Where(b => b.UserId == userId).ToList();

        if (bookings.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nYou have no booked flights.");
            Console.ResetColor();
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"\nYou have {bookings.Count} booked flight(s):\n");
        FlightDisplay.DrawBookingsTableHeader();

        foreach (var booking in bookings)
        {
            var flight = new FlightsLogic().GetFlightsById(booking.FlightId);
            if (flight == null) continue;

            FlightDisplay.DisplayBookingDetails(booking, flight);
            FlightDisplay.DisplayPassengerDetails(booking.Passengers);
            Console.WriteLine(new string('─', Console.WindowWidth - 1));
        }
    }

    // Allows the user to check in for a specific flight
    public static void CheckInForFlight()
    {
        Console.WriteLine("Enter the Flight ID to check in:");
        if (int.TryParse(Console.ReadLine(), out int flightId))
        {
            bool success = _userAccountService.CheckIn(flightId);
            Console.WriteLine(success ? "Check-in successful." : "Check-in failed. Please try again or contact support.");
        }
        else
        {
            Console.WriteLine("Invalid Flight ID.");
        }
    }

    // Handles the booking process for a specific flight
    private static void BookFlight(int flightId, List<FlightModel> flightsList)
    {
        var selectedFlight = flightsList.FirstOrDefault(f => f.FlightId == flightId);
        if (selectedFlight != null)
        {
            Console.WriteLine("How many passengers? (1-8):");
            if (int.TryParse(Console.ReadLine(), out int passengerCount) && passengerCount > 0 && passengerCount <= 8)
            {
                var passengerDetails = CollectPassengerDetails(selectedFlight, passengerCount);
                CompleteBooking(flightId, passengerDetails, selectedFlight);
            }
            else
            {
                Console.WriteLine("Invalid number of passengers.");
            }
        }
        else
        {
            Console.WriteLine("Flight not found with the specified ID.");
        }
    }

    // Collects passenger details for the booking process
    private static List<PassengerModel> CollectPassengerDetails(FlightModel selectedFlight, int passengerCount)
    {
        var passengerDetails = new List<PassengerModel>();
        var seatSelector = new SeatSelectionUI();

        // Load existing booked seats for this flight
        var existingBookings = BookingLogic.GetBookingsForFlight(selectedFlight.FlightId);
        foreach (var booking in existingBookings)
        {
            foreach (var passenger in booking.Passengers)
            {
                seatSelector.SetSeatOccupied(passenger.SeatNumber);
            }
        }

        for (int i = 0; i < passengerCount; i++)
        {
            Console.Clear();
            Console.WriteLine($"Passenger {i + 1} Details:");

            Console.WriteLine("Enter passenger name:");
            string name = Console.ReadLine() ?? string.Empty;

            Console.WriteLine("\nSelect a seat for the passenger:");
            string seatNumber = seatSelector.SelectSeat(selectedFlight.PlaneType);
            seatSelector.SetSeatOccupied(seatNumber);

            Console.WriteLine("Does this passenger have checked baggage? (y/n):");
            bool hasCheckedBaggage = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;

            passengerDetails.Add(new PassengerModel(name, seatNumber, hasCheckedBaggage));
        }

        return passengerDetails;
    }

    // Completes the booking by saving the details
    private static void CompleteBooking(int flightId, List<PassengerModel> passengerDetails, FlightModel selectedFlight)
    {
        try
        {
            BookingModel booking = BookingLogic.CreateBooking(_userAccountService.CurrentUserId, flightId, passengerDetails);

            Console.WriteLine("\nFlight booked successfully!\n");
            Console.WriteLine($"Booking ID: {booking.BookingId}");
            Console.WriteLine($"Flight: {selectedFlight.Origin} to {selectedFlight.Destination}");
            Console.WriteLine($"Departure: {selectedFlight.DepartureTime}");
            Console.WriteLine("\nPassengers:");

            foreach (var passenger in booking.Passengers)
            {
                Console.WriteLine($"\nName: {passenger.Name}");
                Console.WriteLine($"Seat: {passenger.SeatNumber} ({seatSelector.GetSeatClass(passenger.SeatNumber)} Class)");
                Console.WriteLine($"Checked Baggage: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");
            }

            Console.WriteLine($"\nTotal Price: {booking.TotalPrice} EUR");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating booking: {ex.Message}");
        }
    }
}
