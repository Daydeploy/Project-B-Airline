using System;
using System.Collections.Generic;
using System.Linq;

static class FlightManagement
{
    public static void ShowAvailableFlights()
    {
        FlightsLogic flights = new FlightsLogic();
        Console.Clear();

        string origin = SelectOrigin(flights);
        if (string.IsNullOrEmpty(origin)) return;

        string destination = SelectDestination(flights, origin);
        if (string.IsNullOrEmpty(destination)) return;

        List<FlightModel> flightsList = flights.GetFlightsByOriginAndDestination(origin, destination).ToList();
        if (!flightsList.Any())
        {
            Console.WriteLine($"No flights available from {origin} to {destination}.");
            return;
        }

        DisplayFlightsWithActions(flightsList, UserLogin.UserAccountServiceLogic.IsUserLoggedIn());
        HandleFlightCommands(flightsList, origin, destination);
    }

    private static string SelectOrigin(FlightsLogic flights)
    {
        var origins = flights.GetAllOrigins().ToArray();
        if (!origins.Any())
        {
            Console.WriteLine("No available origins found.");
            return null;
        }

        Console.WriteLine("Select your starting location:");
        int originIndex = MenuNavigationService.NavigateMenu(origins, "Available Origins");

        return originIndex != -1 ? origins[originIndex] : null;
    }

    private static string SelectDestination(FlightsLogic flights, string origin)
    {
        var destinations = flights.GetDestinationsByOrigin(origin).ToArray();
        if (!destinations.Any())
        {
            Console.WriteLine($"No destinations available from {origin}.");
            return null;
        }

        Console.WriteLine($"Available destinations from {origin}:");
        int destinationIndex = MenuNavigationService.NavigateMenu(destinations, "Available Destinations");

        return destinationIndex != -1 ? destinations[destinationIndex] : null;
    }

    private static void DisplayFlightsWithActions(List<FlightModel> flightsList, bool allowBooking)
    {
        FlightDisplay.DisplayFlights(flightsList);

        Console.WriteLine("\nCommands:");
        Console.WriteLine("F - Filter flights");
        if (allowBooking)
            Console.WriteLine("B - Book a flight");
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("B - Book a flight (Login required)");
            Console.ResetColor();
        }

        Console.WriteLine("ESC - Go back");
    }

    private static void HandleFlightCommands(List<FlightModel> flightsList, string origin, string destination)
    {
        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Escape)
                return;

            if (key.Key == ConsoleKey.F)
            {
                FilterFlightsByPriceUI(origin, destination);
                return;
            }

            if (key.Key == ConsoleKey.B)
            {
                if (UserLogin.UserAccountServiceLogic.IsUserLoggedIn())
                {
                    Console.Clear();
                    //  BookFlight(flightsList);
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


    public static void FilterFlightsByPriceUI(string origin, string destination)
    {
        FlightsLogic flights = new FlightsLogic();
        string[] filterOptions =
        {
            "Price from low-high",
            "Price from high-low",
            "Price between input range",
            "Filter by date range",
            "Back to Main Menu"
        };

        string[] seatClassOptions = { "Economy", "Business", "First" };

        while (true)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(filterOptions, "Filter Flights:");
            if (selectedIndex == 4)
                return;

            List<FlightModel> filteredFlights = new List<FlightModel>();
            switch (selectedIndex)
            {
                case 0:
                    filteredFlights = FilterByPriceAscending(flights, origin, destination, seatClassOptions);
                    break;
                case 1:
                    filteredFlights = FilterByPriceDescending(flights, origin, destination, seatClassOptions);
                    break;
                case 2:
                    filteredFlights = FilterByPriceRange(flights, origin, destination, seatClassOptions);
                    break;
                case 3:
                    //filteredFlights = FilterByDateRange(flights, origin, destination);
                    break;
            }

            if (filteredFlights.Count != 0)
            {
                DisplayFlightsWithActions(filteredFlights, UserLogin.UserAccountServiceLogic.IsUserLoggedIn());
                HandleFlightCommands(filteredFlights, origin, destination);
            }
            else
            {
                Console.WriteLine("No flights found matching your criteria.");
                Console.WriteLine("\nPress any key to return to the filter menu...");
                Console.ReadKey();
            }
        }
    }

    private static List<FlightModel> FilterByPriceAscending(FlightsLogic flights, string origin, string destination,
        string[] seatClassOptions)
    {
        int seatClassIndex = MenuNavigationService.NavigateMenu(seatClassOptions, "Seat Class");
        string seatClass = seatClassOptions[seatClassIndex];
        return flights.FilterFlightsByPriceUp(origin, destination, seatClass).ToList();
    }

    private static List<FlightModel> FilterByPriceDescending(FlightsLogic flights, string origin, string destination,
        string[] seatClassOptions)
    {
        int seatClassIndex = MenuNavigationService.NavigateMenu(seatClassOptions, "Seat Class");
        string seatClass = seatClassOptions[seatClassIndex];
        return flights.FilterFlightsByPriceDown(origin, destination, seatClass).ToList();
    }

    private static List<FlightModel> FilterByPriceRange(FlightsLogic flights, string origin, string destination,
        string[] seatClassOptions)
    {
        int seatClassIndex = MenuNavigationService.NavigateMenu(seatClassOptions, "Seat Class");
        string seatClass = seatClassOptions[seatClassIndex];

        Console.WriteLine("Enter minimum price:");
        if (int.TryParse(Console.ReadLine(), out int min))
        {
            Console.WriteLine("Enter maximum price:");
            if (int.TryParse(Console.ReadLine(), out int max))
            {
                return flights.FilterFlightsByPriceRange(origin, destination, seatClass, min, max).ToList();
            }
        }

        Console.WriteLine("Invalid price range entered.");
        return new List<FlightModel>();
    }

    // private static List<FlightModel> FilterByDateRange(FlightsLogic flights, string origin, string destination)
    // {
    //     var calendarUI = new CalendarUI();
    //     var (startDate, endDate) = calendarUI.SelectDateRange();
    //     return flights.FilterByDateRange(origin, destination, startDate, endDate).ToList();
    // }

    public static void BookAFlight(AccountModel account)
    {
        Console.Clear();
        FlightsLogic flightsLogic = new FlightsLogic();

        var origins = flightsLogic.GetAllOrigins();
        if (origins.Count == 0)
        {
            Console.WriteLine("No available origins found.");
            return;
        }

        Console.WriteLine("Select your starting location:");
        int originIndex = MenuNavigationService.NavigateMenu(origins.ToArray(), "Available Origins");
        if (originIndex == -1) return;
        string origin = origins[originIndex];

        var destinations = flightsLogic.GetDestinationsByOrigin(origin);
        if (destinations.Count == 0)
        {
            Console.WriteLine($"No destinations available from {origin}.");
            return;
        }

        Console.WriteLine($"Available destinations from {origin}:");
        int destinationIndex = MenuNavigationService.NavigateMenu(destinations.ToArray(), "Available Destinations");
        if (destinationIndex == -1) return;
        string destination = destinations[destinationIndex];

        Console.Clear();
        string[] tripOptions = { "Yes, it's a round-trip booking", "No, one-way trip only" };
        int tripChoice = MenuNavigationService.NavigateMenu(tripOptions, "Is this a round-trip booking?");
        bool isRoundTrip = tripChoice == 0;

        Console.Clear();
        Console.WriteLine("Please select a departure date:");
        CalendarUI calendar = new CalendarUI();
        DateTime departureDate = calendar.SelectDate();
        if (departureDate == DateTime.MinValue)
        {
            Console.WriteLine("Date selection cancelled.");
            return;
        }

        DateTime? returnDate = null;
        if (isRoundTrip)
        {
            Console.Clear();
            Console.WriteLine("Please select a return date:");
            returnDate = calendar.SelectDate();
            if (returnDate == DateTime.MinValue)
            {
                Console.WriteLine("Return date selection cancelled.");
                return;
            }

            if (returnDate <= departureDate)
            {
                Console.WriteLine("Return date must be after the departure date. Please start again.");
                return;
            }
        }

        var availableFlights = flightsLogic.FilterFlightsByDate(origin, destination, departureDate);
        if (availableFlights.Count == 0)
        {
            Console.WriteLine($"No flights found from {origin} to {destination} on {departureDate:dd MMM yyyy}.");
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("\nAvailable flights:");
        string[] flightOptions = availableFlights
            .Select((f, index) =>
                $"{f.Origin} → {f.Destination}\n" +
                $"Departure: {DateTime.Parse(f.DepartureTime):HH:mm dd MMM yyyy}  |  Arrival: {DateTime.Parse(f.ArrivalTime):HH:mm dd MMM yyyy}\n" +
                $"Economy: {f.SeatClassOptions[0].Price} EUR  |  Business: {f.SeatClassOptions[1].Price} EUR  |  First: {f.SeatClassOptions[2].Price} EUR\n" +
                $"{new string('─', 80)}")
            .ToArray();


        int selectedIndex = MenuNavigationService.NavigateMenu(flightOptions, "Select a flight:");

        if (selectedIndex == -1)
        {
            Console.WriteLine("Flight selection cancelled.");
            return;
        }

        var selectedFlight = availableFlights[selectedIndex];

        Console.WriteLine("How many passengers? (1-8):");
        if (!int.TryParse(Console.ReadLine(), out int passengerCount) || passengerCount <= 0 || passengerCount > 8)
        {
            Console.WriteLine("Invalid number of passengers.");
            return;
        }

        var seatSelector = new SeatSelectionUI();
        var passengerDetails = CollectPassengerDetails(selectedFlight, passengerCount, seatSelector);

        CompleteBooking(selectedFlight.FlightId, passengerDetails, selectedFlight, seatSelector);

        if (isRoundTrip)
        {
            HandleReturnFlightBooking(destination, origin, departureDate, returnDate.Value, passengerDetails,
                seatSelector);
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }


    private static void HandleReturnFlightBooking(string destination, string origin, DateTime departureDate,
        DateTime returnDate, List<PassengerModel> passengerDetails, SeatSelectionUI seatSelector
    )
    {
        Console.Clear();
        Console.WriteLine($"Finding return flights from {destination} to {origin}...");

        FlightsLogic flightsLogic = new FlightsLogic();
        var returnFlights = flightsLogic.FilterFlightsByDate(destination, origin, returnDate);

        if (returnFlights.Count == 0)
        {
            Console.WriteLine(
                $"No return flights available from {destination} to {origin} on {returnDate:dd MMM yyyy}.");
            return;
        }

        Console.WriteLine("\nAvailable return flights:");
        string[] flightOptions = returnFlights
            .Select((f, index) =>
                $"{f.Origin} → {f.Destination}\n" +
                $"Departure: {DateTime.Parse(f.DepartureTime):HH:mm dd MMM yyyy}  |  Arrival: {DateTime.Parse(f.ArrivalTime):HH:mm dd MMM yyyy}\n" +
                $"Economy: {f.SeatClassOptions[0].Price} EUR  |  Business: {f.SeatClassOptions[1].Price} EUR  |  First: {f.SeatClassOptions[2].Price} EUR\n" +
                $"{new string('─', 80)}")
            .ToArray();

        int selectedIndex = MenuNavigationService.NavigateMenu(flightOptions, "Select a return flight:");

        if (selectedIndex == -1)
        {
            Console.WriteLine("Return flight selection cancelled.");
            return;
        }

        var selectedReturnFlight = returnFlights[selectedIndex];

        foreach (var passenger in passengerDetails)
        {
            Console.WriteLine($"\nSelect a seat for {passenger.Name} on the return flight:");
            string seatNumber = seatSelector.SelectSeat(selectedReturnFlight.PlaneType);
            seatSelector.SetSeatOccupied(seatNumber);

            if (passenger.HasPet)
            {
                seatSelector.SetPetSeat(seatNumber);
            }

            passenger.SeatNumber = seatNumber;
        }

        CompleteBooking(selectedReturnFlight.FlightId, passengerDetails, selectedReturnFlight, seatSelector);
        Console.WriteLine("\nRound-trip booking completed successfully!");
    }


    private static List<PassengerModel> CollectPassengerDetails(FlightModel selectedFlight, int passengerCount,
        SeatSelectionUI seatSelector)
    {
        var passengerDetails = new List<PassengerModel>();
        string[] petTypes = new[] { "Dog", "Cat", "Bird", "Rabbit", "Hamster" };
        Dictionary<string, double> maxWeights = new()
        {
            { "Dog", 32.0 },
            { "Cat", 15.0 },
            { "Bird", 2.0 },
            { "Rabbit", 8.0 },
            { "Hamster", 1.0 }
        };

        for (int i = 0; i < passengerCount; i++)
        {
            Console.Clear();
            Console.WriteLine($"Passenger {i + 1} Details:");
            Console.WriteLine("Enter passenger name:");
            string name = Console.ReadLine() ?? string.Empty;


            Console.WriteLine("Does this passenger have checked baggage? (y/n):");
            bool hasCheckedBaggage = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;


            Console.WriteLine("Does this passenger have a pet? (y/n):");
            bool hasPet = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;

            PetModel petDetails = null;
            if (hasPet)
            {
                petDetails = SelectPetDetails(petTypes, maxWeights);
            }

            Console.WriteLine("\nSelect a seat for the passenger:");
            string seatNumber = seatSelector.SelectSeat(selectedFlight.PlaneType);
            seatSelector.SetSeatOccupied(seatNumber);

            if (hasPet)
            {
                seatSelector.SetPetSeat(seatNumber);
            }


            var passenger = new PassengerModel(name, seatNumber, hasCheckedBaggage, hasPet, petDetails);
            passengerDetails.Add(passenger);
        }


        return passengerDetails;
    }

    private static PetModel SelectPetDetails(string[] petTypes, Dictionary<string, double> maxWeights)
    {
        int selectedIndex = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            Console.WriteLine("Select pet type:");
            for (int i = 0; i < petTypes.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("> ");
                }
                else
                {
                    Console.Write("  ");
                }

                Console.WriteLine(petTypes[i]);
                Console.ResetColor();
            }

            key = Console.ReadKey(true).Key;
            if (key == ConsoleKey.UpArrow && selectedIndex > 0) selectedIndex--;
            if (key == ConsoleKey.DownArrow && selectedIndex < petTypes.Length - 1) selectedIndex++;
        } while (key != ConsoleKey.Enter);

        string selectedPetType = petTypes[selectedIndex];
        double maxWeight = maxWeights[selectedPetType];

        Console.WriteLine($"\nEnter {selectedPetType}'s weight in kg (max {maxWeight}kg):");
        double weight;
        while (!double.TryParse(Console.ReadLine(), out weight) || weight <= 0 || weight > 100)
        {
            Console.WriteLine("Please enter a valid weight (0-100kg):");
        }

        string storageLocation = weight > maxWeight / 2 ? "Cargo" : "Storage";
        if (weight > maxWeight)
        {
            Console.WriteLine($"\nWarning: Pet exceeds maximum weight for {selectedPetType}.");
            Console.WriteLine("Pet must be transported in cargo area.");
            storageLocation = "Cargo";
        }
        else
        {
            Console.WriteLine($"\nPet will be transported in {storageLocation}.");
        }

        return new PetModel
        {
            Type = selectedPetType,
            Weight = weight,
            StorageLocation = storageLocation
        };
    }

    private static void CompleteBooking(int flightId, List<PassengerModel> passengerDetails, FlightModel selectedFlight,
        SeatSelectionUI seatSelector)
    {
        try
        {
            BookingModel booking = BookingLogic.CreateBooking(UserLogin.UserAccountServiceLogic.CurrentUserId, flightId,
                passengerDetails, new List<PetModel>());
            Console.WriteLine("\nFlight booked successfully!\n");
            Console.WriteLine($"Booking ID: {booking.BookingId}");
            Console.WriteLine($"Flight: {selectedFlight.Origin} to {selectedFlight.Destination}");
            Console.WriteLine($"Departure: {DateTime.Parse(selectedFlight.DepartureTime):HH:mm dd MMM yyyy}");
            Console.WriteLine("\nPassengers:");
            foreach (var passenger in booking.Passengers)
            {
                Console.WriteLine($"\nName: {passenger.Name}");
                Console.WriteLine(
                    $"Seat: {passenger.SeatNumber} ({seatSelector.GetSeatClass(passenger.SeatNumber)} Class)");
                Console.WriteLine($"Checked Baggage: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");
            }

            if (booking.Passengers?.Any() == true)
            {
                foreach (var passenger in booking.Passengers)
                {
                    if (!string.IsNullOrEmpty(passenger.SeatNumber))
                    {
                        var seatClass = new SeatSelectionUI().GetSeatClass(passenger.SeatNumber, selectedFlight.PlaneType);
                        var basePrice = selectedFlight.SeatClassOptions
                            .FirstOrDefault(so => so.SeatClass.Equals(seatClass, StringComparison.OrdinalIgnoreCase))
                            ?.Price ?? 0;
                        
                        Console.WriteLine($"\nBase Price: ({seatClass}): {basePrice:F2} EUR");
                    }
                }
            }
            Console.WriteLine($"Total Price: {booking.TotalPrice} EUR");

            // Calculate miles and apply points redemption
            int milesEarned = MilesLogic.CalculateMilesFromBooking(UserLogin.UserAccountServiceLogic.CurrentUserId);
            Console.WriteLine($"Miles Earned: {milesEarned}");

            booking.TotalPrice = MilesLogic.BasicPointsRedemption(UserLogin.UserAccountServiceLogic.CurrentUserId,
                booking.TotalPrice, booking.BookingId);

            // Display the updated price
            Console.WriteLine($"\nDiscounted Total Price: {booking.TotalPrice} EUR");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating booking: {ex.Message}");
        }
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

    public static void CheckInForFlight()
    {
        Console.WriteLine("Enter the Flight ID to check in:");
        if (int.TryParse(Console.ReadLine(), out int flightId))
        {
            bool success = UserLogin.UserAccountServiceLogic.CheckIn(flightId);
            Console.WriteLine(
                success ? "Check-in successful." : "Check-in failed. Please try again or contact support.");
        }
        else
        {
            Console.WriteLine("Invalid Flight ID.");
        }
    }
}