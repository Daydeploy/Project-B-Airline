using System;
using System.Collections.Generic;
using System.Linq;

static class FlightManagement
{
   public static void ShowAvailableFlights()
{
    FlightsLogic flights = new FlightsLogic();
    Console.Clear();

    // Fetch all origins
    var origins = flights.GetAllOrigins().ToArray();
    if (!origins.Any())
    {
        Console.WriteLine("No available origins found.");
        return;
    }

    Console.WriteLine("Select your starting location:");

    // Use the navigation service to select an origin
    int originIndex = MenuNavigationService.NavigateMenu(origins, "Available Origins");
    if (originIndex == -1) // Backspace pressed
    {
        Console.WriteLine("Returning to the menu.");
        return;
    }

    string selectedOrigin = origins[originIndex];
    Console.Clear();

    // Fetch destinations based on the selected origin
    var destinations = flights.GetDestinationsByOrigin(selectedOrigin).ToArray();
    if (!destinations.Any())
    {
        Console.WriteLine($"No destinations available from {selectedOrigin}.");
        return;
    }

    Console.WriteLine($"Available destinations from {selectedOrigin}:");

    // Use the navigation service to select a destination
    int destinationIndex = MenuNavigationService.NavigateMenu(destinations, "Available Destinations");
    if (destinationIndex == -1) // Backspace pressed
    {
        Console.WriteLine("Returning to the menu.");
        return;
    }

    string selectedDestination = destinations[destinationIndex];
    Console.Clear();

    // Get flights from the selected origin to the selected destination
    var flightsList = flights.GetFlightsByOriginAndDestination(selectedOrigin, selectedDestination).ToList();
    if (!flightsList.Any())
    {
        Console.WriteLine($"No flights available from {selectedOrigin} to {selectedDestination}.");
        return;
    }

    Console.WriteLine($"Available flights from {selectedOrigin} to {selectedDestination}:\n");
    FlightDisplay.DisplayFlights(flightsList);

    Console.WriteLine("\nCommands:");
    Console.WriteLine("F - Filter flights");

    // Only show the "Book a flight" option if the user is logged in
    if (UserLogin._userAccountService.IsUserLoggedIn())
    {
        Console.WriteLine("B - Book a flight");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("B - Book a flight (Login required)");
        Console.ResetColor();
    }

    Console.WriteLine("ESC - Go back");

    while (true)
    {
        var key = Console.ReadKey(intercept: true);

        if (key.Key == ConsoleKey.Escape)
            return;

        if (key.Key == ConsoleKey.F)
        {
            FilterFlightsByPriceUI(selectedOrigin, selectedDestination);
            return;
        }

        if (key.Key == ConsoleKey.B)
        {
            if (UserLogin._userAccountService.IsUserLoggedIn())
            {
                Console.Clear();
                BookFlight(flightsList);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nYou must be logged in to book a flight.");
                Console.ResetColor();
            }
            return;
        }
    }
}


    private static void BookFlight(List<FlightModel> flightsList)
    {
        Console.WriteLine("Enter the Flight ID to book:");
        if (int.TryParse(Console.ReadLine(), out int flightId))
        {
            var selectedFlight = flightsList.FirstOrDefault(f => f.FlightId == flightId);
            if (selectedFlight != null)
            {
                Console.WriteLine("How many passengers? (1-8):");
                if (int.TryParse(Console.ReadLine(), out int passengerCount) && passengerCount > 0 &&
                    passengerCount <= 8)
                {
                    var seatSelector = new SeatSelectionUI();
                    var passengerDetails = CollectPassengerDetails(selectedFlight, passengerCount, seatSelector);
                    CompleteBooking(flightId, passengerDetails, selectedFlight, seatSelector);
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
        else
        {
            Console.WriteLine("Invalid Flight ID format.");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }

    public static void ViewBookedFlights(int userId)
    {
        while (true)
        {
            Console.Clear();
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
                var flightsLogic = new FlightsLogic();
                var flight = flightsLogic.GetFlightsById(booking.FlightId);
                if (flight == null) continue;

                FlightDisplay.DisplayBookingDetails(booking, flight);
                FlightDisplay.DisplayPassengerDetails(booking.Passengers);
                Console.WriteLine(new string('─', Console.WindowWidth - 1));
            }

            BookingModifications.DisplayBookingLegend();

            Console.Write("\nEnter command: ");
            string command = Console.ReadLine()?.ToLower() ?? "";

            switch (command)
            {
                case "m":
                    BookingModifications.ModifyBookingInteractive(userId);
                    break;
                case "b":
                    return;
                default:
                    Console.WriteLine("Invalid command. Press any key to continue...");
                    Console.ReadKey();
                    break;
            }
        }
    }


    // Allows the user to check in for a specific flight
    public static void CheckInForFlight()
    {
        Console.WriteLine("Enter the Flight ID to check in:");
        if (int.TryParse(Console.ReadLine(), out int flightId))
        {
            bool success = UserLogin._userAccountService.CheckIn(flightId);
            Console.WriteLine(
                success ? "Check-in successful." : "Check-in failed. Please try again or contact support.");
        }
        else
        {
            Console.WriteLine("Invalid Flight ID.");
        }
    }

    // UI to filter flights by price or other options
   public static void FilterFlightsByPriceUI(string selectedOrigin, string selectedDestination)
{
    FlightsLogic flights = new FlightsLogic();
    string[] filterOptions = new[]
    {
        "Price from low-high",
        "Price from high-low",
        "Price between input range",
        "Filter by date range",
        "Back to Main Menu"
    };

    int selectedIndex = MenuNavigationService.NavigateMenu(filterOptions, "Filter Flights:");

    string[] seatClassOptions = { "Economy", "Business", "First" };

    switch (selectedIndex)
    {
        case 0:
            int seatClassAscIndex = MenuNavigationService.NavigateMenu(seatClassOptions, "Seat Class");
            string seatClassAsc = seatClassOptions[seatClassAscIndex];
            FlightDisplay.DisplayFlights(flights.FilterFlightsByPriceUp(selectedOrigin, selectedDestination, seatClassAsc));
            break;
        case 1:
            int seatClassDescIndex = MenuNavigationService.NavigateMenu(seatClassOptions, "Seat Class");
            string seatClassDesc = seatClassOptions[seatClassDescIndex];
            FlightDisplay.DisplayFlights(flights.FilterFlightsByPriceDown(selectedOrigin, selectedDestination, seatClassDesc));
            break;
        case 2:
            int seatClassRangeIndex = MenuNavigationService.NavigateMenu(seatClassOptions, "Seat Class");
            string seatClassRange = seatClassOptions[seatClassRangeIndex];
            Console.WriteLine("Enter minimum price: ");
            if (int.TryParse(Console.ReadLine(), out int min))
            {
                Console.WriteLine("Enter maximum price: ");
                if (int.TryParse(Console.ReadLine(), out int max))
                {
                    FlightDisplay.DisplayFlights(
                        flights.FilterFlightsByPriceRange(selectedOrigin, selectedDestination, seatClassRange, min, max));
                }
            }
            break;
        case 3:
            var calendarUI = new CalendarUI();
            var (startDate, endDate) = calendarUI.SelectDateRange();
            FlightDisplay.DisplayFlights(flights.FilterByDateRange(selectedOrigin, selectedDestination, startDate, endDate));
            break;
        case 4:
            return;
    }
}

    private static List<PassengerModel> CollectPassengerDetails(FlightModel selectedFlight, int passengerCount,
        SeatSelectionUI seatSelector)
    {
        var passengerDetails = new List<PassengerModel>();

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

    private static void CompleteBooking(int flightId, List<PassengerModel> passengerDetails, FlightModel selectedFlight,
        SeatSelectionUI seatSelector)
    {
        try
        {
            BookingModel booking = BookingLogic.CreateBooking(UserLogin._userAccountService.CurrentUserId, flightId,
                passengerDetails, new List<PetModel>()); // Modify as needed

            Console.WriteLine("\nFlight booked successfully!\n");
            Console.WriteLine($"Booking ID: {booking.BookingId}");
            Console.WriteLine($"Flight: {selectedFlight.Origin} to {selectedFlight.Destination}");
            Console.WriteLine($"Departure: {selectedFlight.DepartureTime}");
            Console.WriteLine("\nPassengers:");

            foreach (var passenger in booking.Passengers)
            {
                Console.WriteLine($"\nName: {passenger.Name}");
                Console.WriteLine(
                    $"Seat: {passenger.SeatNumber} ({seatSelector.GetSeatClass(passenger.SeatNumber)} Class)");
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