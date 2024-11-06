using System;
using System.Collections.Generic;

static class UserLogin
{
    static private UserAccountService _userAccountService = new UserAccountService();

    private static bool _isLoggedIn = true;

    public static void Start()
    {
        AccountModel acc = null;
        Console.WriteLine("Welcome to the login page");
        Console.WriteLine("Note: You can press F2 to toggle password visibility while typing.");
        Console.Write("Please enter your login details:\nEmail: ");
        string email = Console.ReadLine() ?? string.Empty;
        // Console.WriteLine("Please enter your password");
        // string password = Console.ReadLine() ?? string.Empty;
        
        string password = "";
        bool showPassword = false;
        ConsoleKeyInfo key; 

        Console.Write("Enter your password: ");
        password = ReadPassword(ref showPassword); // Pass showPassword by reference to allow toggling visibility within ReadPassword

        Console.Write("Confirm your password: ");
        

        Console.WriteLine("\nPasswords match!");
        acc = _userAccountService.Login(email, password);
        
        if (acc != null)
        {
            Console.WriteLine("Welcome back " + acc.FullName);
            Console.WriteLine("Your email is " + acc.EmailAddress);
            ShowLoggedInMenu(acc);
        }
        else
        {
            Console.WriteLine("No account found with that email and password");
            Menu.Start();
        }
    }

    public static string ReadPassword(ref bool showPassword)
    {
        string pass = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.F2) // Toggle password visibility on F2 key press
            {
                showPassword = !showPassword; // Toggle visibility
                Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r"); // Clear current line
                Console.Write("Enter your password: " + (showPassword ? pass : new string('*', pass.Length)));
            }
            // Handle backspace
            else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
            {
                pass = pass.Substring(0, pass.Length - 1);
                Console.Write("\b \b"); // Erase the last character or asterisk
            }
            // Normal character input
            else if (key.Key != ConsoleKey.Enter)
            {
                pass += key.KeyChar;
                if (showPassword)
                {
                    Console.Write(key.KeyChar); // Show actual character
                }
                else
                {
                    Console.Write("*");
                }
            }
        }
        while (key.Key != ConsoleKey.Enter); // Stop on Enter key

        Console.WriteLine();
        return pass;
    }

    private static void ShowLoggedInMenu(AccountModel account)
    {
        string[] menuItems = new[]
        {
            "View Booked Flights",
            "Check-in for a Flight",
            "Modify Booking",
            "Manage Account",
            "Show Available Flights",
            "View Airport Information",
            "Browse Destinations",
            "Search Flights by Destination",
            "View Direct vs. Connecting Flights",
            "Logout"
        };

        int selectedIndex = 0; // Start at the first menu item

        while (_isLoggedIn)
        {
            // Display the menu
            Menu.DisplayMenu(menuItems, selectedIndex, "Logged In Menu");

            // Handle user input
            var key = Console.ReadKey(intercept: true).Key; // Read key input without displaying it

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex == 0) ? menuItems.Length - 1 : selectedIndex - 1; // Loop to the end
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex =
                        (selectedIndex == menuItems.Length - 1) ? 0 : selectedIndex + 1; // Loop to the beginning
                    break;
                case ConsoleKey.Enter:
                    switch (selectedIndex)
                    {
                        case 0:
                            ViewBookedFlights(account.Id);
                            break;
                        case 1:
                            CheckInForFlight();
                            break;
                        case 2:
                            ModifyBooking();
                            break;
                        case 3:
                            ManageAccount(account);
                            break;
                        case 4:
                            ShowAvailableFlights();
                            break;
                        case 5:
                            ViewAirportInformation();
                            break;
                        case 6:
                            BrowseDestinations();
                            break;
                        case 7:
                            SearchFlightsByDestination();
                            break;
                        case 8:
                            ViewDirectVsConnectingFlights();
                            break;
                        case 9:
                            Console.WriteLine("Logging out...");
                            _isLoggedIn = false;
                            Menu.Start();
                            break;
                    }

                    break;
                case ConsoleKey.Escape:
                    Console.WriteLine("Exiting menu...");
                    _isLoggedIn = false; // or you can implement another way to exit
                    Menu.Start();
                    break;
                default:
                    break; // Ignore any other keys
            }
        }
    }


    private static void ViewBookedFlights(int userId)
    {
        var bookedFlights = _userAccountService.GetBookedFlights(userId);
        if (bookedFlights.Count == 0)
        {
            Console.WriteLine("You have no booked flights.");
        }
        else
        {
            Console.WriteLine("Your booked flights:");
            foreach (var flight in bookedFlights)
            {
                Console.WriteLine(
                    $"Flight ID: {flight.FlightId}, Number: {flight.FlightNumber}, Departure: {flight.DepartureTime}, Arrival: {flight.ArrivalTime}");
            }
        }
    }

    private static void CheckInForFlight()
    {
        Console.WriteLine("Enter the Flight ID to check in:");
        if (int.TryParse(Console.ReadLine(), out int flightId))
        {
            bool success = _userAccountService.CheckIn(flightId);
            if (success)
            {
                Console.WriteLine("Check-in successful.");
            }
            else
            {
                Console.WriteLine("Check-in failed. Please try again or contact support.");
            }
        }
        else
        {
            Console.WriteLine("Invalid Flight ID.");
        }
    }

    private static void ModifyBooking()
    {
        Console.WriteLine("Enter the Flight ID to modify:");
        if (int.TryParse(Console.ReadLine(), out int flightId))
        {
            // Get the booking first to show existing passengers
            var bookings = BookingAccess.LoadAll();
            var booking = bookings.FirstOrDefault(b => b.FlightId == flightId);

            if (booking == null)
            {
                Console.WriteLine("No booking found for this flight ID.");
                return;
            }

            // Display current passengers
            Console.WriteLine("\nCurrent passengers:");
            for (int i = 0; i < booking.Passengers.Count; i++)
            {
                var passenger = booking.Passengers[i];
                Console.WriteLine(
                    $"{i + 1}. {passenger.Name} - Seat: {passenger.SeatNumber} - Checked Baggage: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");
            }

            Console.WriteLine("\nEnter passenger number to modify (1-" + booking.Passengers.Count + "):");
            if (int.TryParse(Console.ReadLine(), out int passengerNumber) &&
                passengerNumber > 0 &&
                passengerNumber <= booking.Passengers.Count)
            {
                int passengerId = passengerNumber - 1; // Convert to 0-based index

                Console.WriteLine("Enter new seat number:");
                string seatNumber = Console.ReadLine();

                Console.WriteLine("Do you have checked baggage? (y/n):");
                bool hasCheckedBaggage = Console.ReadLine().ToLower() == "y";

                var newDetails = new BookingDetails
                {
                    SeatNumber = seatNumber,
                    HasCheckedBaggage = hasCheckedBaggage
                };

                bool success = _userAccountService.ModifyBooking(flightId, passengerId, newDetails);
                if (success)
                {
                    Console.WriteLine("Booking modified successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to modify booking. Please try again or contact support.");
                }
            }
            else
            {
                Console.WriteLine("Invalid passenger number.");
            }
        }
        else
        {
            Console.WriteLine("Invalid Flight ID.");
        }
    }

    private static void ManageAccount(AccountModel account)
    {
        Console.WriteLine("Manage Account:");
        Console.WriteLine("1. Change Email");
        Console.WriteLine("2. Change Password");
        Console.WriteLine("3. Change Full Name");
        Console.WriteLine("4. Back to Main Menu");

        string choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Console.WriteLine("Enter new email:");
                string newEmail = Console.ReadLine();
                if (_userAccountService.ManageAccount(account.Id, newEmail: newEmail))
                {
                    Console.WriteLine("Email updated successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to update email.");
                }

                break;
            case "2":
                Console.WriteLine("Enter new password:");
                string newPassword = Console.ReadLine();
                if (_userAccountService.ManageAccount(account.Id, newPassword: newPassword))
                {
                    Console.WriteLine("Password updated successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to update password.");
                }

                break;
            case "3":
                Console.WriteLine("Enter new full name:");
                string newFullName = Console.ReadLine();
                if (_userAccountService.ManageAccount(account.Id, newFullName: newFullName))
                {
                    Console.WriteLine("Full name updated successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to update full name.");
                }

                break;
            case "4":
                return;
            default:
                Console.WriteLine("Invalid choice.");
                break;
        }
    }


    private static void ViewAirportInformation()
    {
        var airportLogic = new AirportLogic();
        var airports = airportLogic.GetAllAirports();

        Console.WriteLine("\nAirport Information:");
        foreach (var airport in airports)
        {
            Console.WriteLine($"Name: {airport.Name}");
            Console.WriteLine($"Address: {airport.Address}");
            Console.WriteLine($"Phone Number: {airport.PhoneNumber}");
            Console.WriteLine($"Country: {airport.Country}");
            Console.WriteLine($"City: {airport.City}");
            Console.WriteLine();

            // Show transportation options
            var airportService = new AirportService(new List<AirportModel> { airport });
            Console.WriteLine("Transportation Options: " + airportService.GetAirportTransportationOptions(airport));

            // Show nearby hotels
            Console.WriteLine("Nearby Hotels: " + airportService.GetNearbyHotels(airport));

            // Show additional services
            Console.WriteLine("Additional Services: " + airportService.GetAdditionalServices(airport));

            // Make the page more interesting
            Console.WriteLine($"Description: {airportService.GetAirportDescription(airport)}");
            Console.WriteLine(new string('-', 50)); // Separator for clarity
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }

    static public void ShowAvailableFlights()
    {
        FlightsLogic flights = new FlightsLogic();
        Console.WriteLine("Available Flights:");
        foreach (var flight in flights.GetAllFlights())
        {
            // Display all flight information
            Console.WriteLine(
                $"ID: {flight.FlightId} {flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.Price} EUR");
        }

        Console.WriteLine("Do you want to filter the flights (y/n)");
        string input = Console.ReadLine()?.ToLower() ?? "";
        if (input == "y" || input == "yes")
        {
            Menu.FilterFlightsByPriceUI();
        }

        // If user is logged allow the user to book a flight
        if (_userAccountService.IsLoggedIn == true)
        {
            Console.Clear();
            Console.WriteLine("Do you want to book a flight (y/n)");

            string input2 = Console.ReadLine()?.ToLower() ?? "";

            if (input2 == "y" || input2 == "yes")
            {
                Console.WriteLine("Enter the Flight ID to book:");
                if (int.TryParse(Console.ReadLine(), out int flightId))
                {
                    var selectedFlight = flights.GetAllFlights().FirstOrDefault(f => f.FlightId == flightId);
                    if (selectedFlight != null)
                    {
                        Console.WriteLine("How many passengers? (1-8):");
                        if (int.TryParse(Console.ReadLine(), out int passengerCount) && passengerCount > 0 &&
                            passengerCount <= 8)
                        {
                            var passengerDetails = new List<PassengerModel>();

                            for (int i = 0; i < passengerCount; i++)
                            {
                                Console.WriteLine($"\nPassenger {i + 1} Details:");

                                Console.WriteLine("Enter passenger name:");
                                string name = Console.ReadLine() ?? string.Empty;

                                Console.WriteLine("Enter desired seat number:");
                                string seatNumber = Console.ReadLine() ?? string.Empty;

                                Console.WriteLine("Does this passenger have checked baggage? (y/n):");
                                bool hasCheckedBaggage = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;

                                // Create PassengerModel using the constructor
                                passengerDetails.Add(new PassengerModel(
                                    name: name,
                                    seatNumber: seatNumber,
                                    hasCheckedBaggage: hasCheckedBaggage
                                ));
                            }

                            Console.Clear();
                            try
                            {
                                BookingModel booking = BookingLogic.CreateBooking(
                                    _userAccountService.CurrentUserId,
                                    selectedFlight.Destination,
                                    passengerDetails
                                );

                                Console.WriteLine("\nFlight booked successfully!\n");
                                Console.WriteLine($"Booking ID: {booking.BookingId}");
                                Console.WriteLine($"Flight: {selectedFlight.Origin} to {selectedFlight.Destination}");
                                Console.WriteLine($"Departure: {selectedFlight.DepartureTime}");
                                Console.WriteLine("\nPassengers:");

                                foreach (var passenger in booking.Passengers)
                                {
                                    Console.WriteLine($"\nName: {passenger.Name}");
                                    Console.WriteLine($"Seat Number: {passenger.SeatNumber}");
                                    Console.WriteLine(
                                        $"Checked Baggage: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");
                                }

                                Console.WriteLine($"\nTotal Price: {booking.TotalPrice} EUR");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Error creating booking: {ex.Message}");
                            }
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
        }
    }

    private static void BrowseDestinations()
    {
        FlightsLogic flightsLogic = new FlightsLogic();
        var destinations = flightsLogic.GetAllDestinations();

        Console.WriteLine("\nAvailable Destinations:");
        foreach (var destination in destinations)
        {
            Console.WriteLine(destination);
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }

    private static void SearchFlightsByDestination()
    {
        Console.WriteLine("\nEnter the destination you want to search for:");
        string? destination = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(destination))
        {
            FlightsLogic flightsLogic = new FlightsLogic();
            var flights = flightsLogic.SearchFlightsByDestination(destination);

            if (flights.Count == 0)
            {
                Console.WriteLine($"No flights found for destination: {destination}");
            }
            else
            {
                Console.WriteLine($"\nFlights to {destination}:");
                foreach (var flight in flights)
                {
                    Console.WriteLine(
                        $"Flight ID: {flight.FlightId}, From: {flight.Origin}, Departure: {flight.DepartureTime}, Price: {flight.Price} EUR");
                }
            }
        }
        else
        {
            Console.WriteLine("Invalid destination.");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }

    private static void ViewDirectVsConnectingFlights()
    {
        FlightsLogic flightsLogic = new FlightsLogic();
        var directFlights = flightsLogic.GetDirectFlights();
        var connectingFlights = flightsLogic.GetConnectingFlights();

        Console.WriteLine("\nDirect Flights:");
        foreach (var flight in directFlights)
        {
            Console.WriteLine(
                $"Flight ID: {flight.FlightId}, From: {flight.Origin}, To: {flight.Destination}, Departure: {flight.DepartureTime}, Price: {flight.Price} EUR");
        }

        Console.WriteLine("\nConnecting Flights:");
        foreach (var flight in connectingFlights)
        {
            Console.WriteLine(
                $"Flight ID: {flight.FlightId}, From: {flight.Origin}, To: {flight.Destination}, Departure: {flight.DepartureTime}, Price: {flight.Price} EUR");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}
