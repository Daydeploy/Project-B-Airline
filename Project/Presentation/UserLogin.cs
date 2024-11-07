using System;
using System.Collections.Generic;

static class UserLogin
{
    static private UserAccountService _userAccountService = new UserAccountService();

    private static bool _isLoggedIn = true;

    public static void Start()
    {
        AccountModel? acc = null;
        Console.WriteLine("Welcome to the login page");
        Console.WriteLine("Note: You can press F2 to toggle password visibility while typing.");
        Console.Write("Please enter your login details:\nEmail: ");
        string email = Console.ReadLine() ?? string.Empty;
        // Console.WriteLine("Please enter your password");
        // string password = Console.ReadLine() ?? string.Empty;

        string password = "";
        bool showPassword = false;
        // ConsoleKeyInfo key; // Removed unused variable

        Console.Write("Enter your password: ");
        password = ReadPassword(
            ref showPassword); // Pass showPassword by reference to allow toggling visibility within ReadPassword

        Console.Write("Confirm your password: ");


        Console.WriteLine("\nPasswords match!");
        acc = _userAccountService.Login(email, password);

        if (acc != null)
        {
            Console.WriteLine($"Welcome back {acc.FirstName} {acc.LastName}");
            Console.WriteLine($"Your email is {acc.EmailAddress}");
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
        } while (key.Key != ConsoleKey.Enter); // Stop on Enter key

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
            "Logout",
            "Show Seat Upgrade Options"
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
                        case 10:
                            ShowSeatUpgradeOptions();
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
        var bookings = BookingAccess.LoadAll();
        if (!DisplayBookings(bookings)) return;

        int bookingNumber = GetBookingSelection(bookings.Count);
        if (bookingNumber == -1) return;

        var booking = bookings[bookingNumber - 1];
        if (!DisplayPassengers(booking)) return;

        int passengerNumber = GetPassengerSelection(booking.Passengers.Count);
        if (passengerNumber == -1) return;

        ModifyPassengerDetails(booking.FlightId, passengerNumber - 1);
    }

    static public void ShowSeatUpgradeOptions()
    {
        string[] upgradeOptions = new[]
        {
            "View Available Upgrades",
            "Request Upgrade",
            "Use Miles for Upgrade",
            "Confirm Upgrade",
            "View Upgrade Benefits",
            "Back to Main Menu"
        };

        int selectedIndex = 0;
        bool exit = false;

        while (!exit)
        {
            Menu.DisplayMenu(upgradeOptions, selectedIndex, "Seat Upgrade Options");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : upgradeOptions.Length - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex < upgradeOptions.Length - 1) ? selectedIndex + 1 : 0;
                    break;
                case ConsoleKey.Enter:
                    switch (selectedIndex)
                    {
                        case 0:
                            ViewAvailableUpgrades();
                            break;
                        case 1:
                            RequestUpgrade();
                            break;
                        case 2:
                            UseMilesForUpgrade();
                            break;
                        case 3:
                            ConfirmUpgrade();
                            break;
                        case 4:
                            ViewUpgradeBenefits();
                            break;
                        case 5:
                            exit = true; // Exit to main menu
                            break;
                    }
                    break;
            }
        }
    }

    // New method to view available upgrades
    private static void ViewAvailableUpgrades()
    {
        Console.WriteLine("Enter your flight ID to view available upgrades:");
        if (int.TryParse(Console.ReadLine(), out int flightId))
        {
            var seatUpgradeService = new SeatUpgradeService();
            var availableUpgrades = seatUpgradeService.ViewAvailableUpgrades(flightId);

            if (availableUpgrades.Count > 0)
            {
                Console.WriteLine("Available upgrades:");
                foreach (var upgrade in availableUpgrades)
                {
                    Console.WriteLine(upgrade);
                }
            }
            else
            {
                Console.WriteLine("No available upgrades for this flight.");
            }
        }
        else
        {
            Console.WriteLine("Invalid flight ID.");
        }
    }

    // New method to request an upgrade
    private static void RequestUpgrade()
    {
        Console.WriteLine("Enter your desired upgrade class:");
        string newSeatClass = Console.ReadLine() ?? string.Empty;
        bool upgradeSuccess = _userAccountService.RequestUpgrade(_userAccountService.CurrentUserId, newSeatClass); // Use the existing instance

        Console.WriteLine(upgradeSuccess ? "Upgrade request successful!" : "Upgrade request failed. Please check your miles or payment options.");
    }

    // New method to confirm the upgrade
    private static void ConfirmUpgrade()
    {
        Console.WriteLine("Confirming upgrade...");
        // Logic to confirm the upgrade can be added here
        Console.WriteLine("Upgrade confirmed successfully!");
    }

    // New method to view upgrade benefits
    private static void ViewUpgradeBenefits()
    {
        string seatClass;
        do
        {
            Console.WriteLine("Enter the class you want to view benefits for:");
            seatClass = Console.ReadLine() ?? string.Empty;
        } while (string.IsNullOrWhiteSpace(seatClass)); // Ensure seatClass is not null or empty

        var seatUpgradeService = new SeatUpgradeService();
        string benefits = seatUpgradeService.ViewUpgradeBenefits(seatClass);
        Console.WriteLine(benefits);
    }

    private static bool DisplayBookings(List<BookingModel> bookings)
    {
        if (bookings.Count == 0)
        {
            Console.WriteLine("No bookings available.");
            return false;
        }

        Console.WriteLine("\nAvailable bookings:");
        for (int i = 0; i < bookings.Count; i++)
        {
            var booking = bookings[i];
            Console.WriteLine($"{i + 1}. Flight ID: {booking.FlightId}");
        }

        return true;
    }

    private static int GetBookingSelection(int bookingCount)
    {
        Console.WriteLine("\nEnter the number of the booking you want to modify:");
        if (int.TryParse(Console.ReadLine(), out int bookingNumber) &&
            bookingNumber > 0 && bookingNumber <= bookingCount)
        {
            return bookingNumber;
        }

        Console.WriteLine("Invalid booking selection.");
        return -1;
    }

    private static bool DisplayPassengers(BookingModel booking)
    {
        Console.WriteLine("\nCurrent passengers:");
        for (int i = 0; i < booking.Passengers.Count; i++)
        {
            var passenger = booking.Passengers[i];
            Console.WriteLine(
                $"{i + 1}. {passenger.Name} - Seat: {passenger.SeatNumber} - Checked Baggage: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");
        }

        return true;
    }

    private static int GetPassengerSelection(int passengerCount)
    {
        Console.WriteLine("\nEnter passenger number to modify (1-" + passengerCount + "):");
        if (int.TryParse(Console.ReadLine(), out int passengerNumber) &&
            passengerNumber > 0 && passengerNumber <= passengerCount)
        {
            return passengerNumber;
        }

        Console.WriteLine("Invalid passenger number.");
        return -1;
    }

    private static void ModifyPassengerDetails(int flightId, int passengerId)
    {
        Console.WriteLine("Enter new seat number:");
        string seatNumber = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Do you have checked baggage? (y/n):");
        bool hasCheckedBaggage = Console.ReadLine()?.ToLower() == "y";

        var newDetails = new BookingDetails
        {
            SeatNumber = seatNumber ?? string.Empty,
            HasCheckedBaggage = hasCheckedBaggage
        };

        bool success = _userAccountService.ModifyBooking(flightId, passengerId, newDetails);
        Console.WriteLine(success
            ? "Booking modified successfully."
            : "Failed to modify booking. Please try again or contact support.");
    }


    private static void ManageAccount(AccountModel account)
    {
        string[] options =
        {
            "Change Email",
            "Change Password",
            "Change First Name",
            "Change Last Name",
            "Change Date of Birth",
            "Change Gender",
            "Change Nationality",
            "Change Phone Number",
            "Change Passport Details",
            "View Account Details",
            "Back to Main Menu"
        };
        int selectedIndex = 0;

        while (true)
        {
            Menu.DisplayMenu(options, selectedIndex, "Manage Account");

            // Read key input for navigation
            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : options.Length - 1;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex < options.Length - 1) ? selectedIndex + 1 : 0;
                    break;
                case ConsoleKey.Enter:
                    if (selectedIndex == 10) return; // Exit immediately on "Back to Main Menu"
                    if (selectedIndex == 9) DisplayAccountDetails(account); // View Account Details
                    else HandleManageAccountOption(selectedIndex, account); // Handle other options
                    break;
            }
        }
    }


    private static void DisplayAccountDetails(AccountModel account)
    {
        Console.WriteLine("\n--- Account Details ---");
        Console.WriteLine($"Email: {account.EmailAddress}");
        Console.WriteLine($"First Name: {account.FirstName}");
        Console.WriteLine($"Last Name: {account.LastName}");
        Console.WriteLine(
            $"Date of Birth: {account.DateOfBirth:yyyy-MM-dd}");
        Console.WriteLine($"Gender: {account.Gender ?? "Not provided"}");
        Console.WriteLine($"Nationality: {account.Nationality ?? "Not provided"}");
        Console.WriteLine($"Phone Number: {account.PhoneNumber ?? "Not provided"}");

        if (account.PassportDetails != null)
        {
            Console.WriteLine("Passport Details:");
            Console.WriteLine($"  Passport Number: {account.PassportDetails.PassportNumber ?? "Not provided"}");
            Console.WriteLine(
                $"  Issue Date: {account.PassportDetails.IssueDate?.ToString("yyyy-MM-dd") ?? "Not provided"}");
            Console.WriteLine(
                $"  Expiration Date: {account.PassportDetails.ExpirationDate?.ToString("yyyy-MM-dd") ?? "Not provided"}");
            Console.WriteLine($"  Country of Issue: {account.PassportDetails.CountryOfIssue ?? "Not provided"}");
        }
        else
        {
            Console.WriteLine("Passport Details: Not provided");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }


    private static void HandleManageAccountOption(int optionIndex, AccountModel account)
    {
        bool updateSuccessful = false;

        switch (optionIndex)
        {
            case 0: // Change Email
                Console.WriteLine($"Current Email: {account.EmailAddress}");
                Console.WriteLine("Enter new email:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newEmail: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Email updated successfully." : "Failed to update email.");
                break;

            case 1: // Change Password
                Console.WriteLine("Enter new password:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newPassword: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Password updated successfully." : "Failed to update password.");
                break;

            case 2: // Change First Name
                Console.WriteLine($"Current First Name: {account.FirstName}");
                Console.WriteLine("Enter new first name:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newFirstName: Console.ReadLine());
                Console.WriteLine(
                    updateSuccessful ? "First name updated successfully." : "Failed to update first name.");
                break;

            case 3: // Change Last Name
                Console.WriteLine($"Current Last Name: {account.LastName}");
                Console.WriteLine("Enter new last name:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newLastName: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Last name updated successfully." : "Failed to update last name.");
                break;

            case 4: // Change Date of Birth
                Console.WriteLine($"Current Date of Birth: {account.DateOfBirth:yyyy-MM-dd}");
                Console.WriteLine("Enter new date of birth (yyyy-mm-dd):");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime newDateOfBirth))
                {
                    updateSuccessful = _userAccountService.ManageAccount(account.Id, newDateOfBirth: newDateOfBirth);
                    Console.WriteLine(updateSuccessful
                        ? "Date of birth updated successfully."
                        : "Failed to update date of birth.");
                }
                else
                {
                    Console.WriteLine("Invalid date format. Date of birth not updated.");
                }

                break;

            case 5: // Change Gender
                Console.WriteLine($"Current Gender: {account.Gender ?? "Not provided"}");
                Console.WriteLine("Enter new gender:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newGender: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Gender updated successfully." : "Failed to update gender.");
                break;

            case 6: // Change Nationality
                Console.WriteLine($"Current Nationality: {account.Nationality ?? "Not provided"}");
                Console.WriteLine("Enter new nationality:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newNationality: Console.ReadLine());
                Console.WriteLine(updateSuccessful
                    ? "Nationality updated successfully."
                    : "Failed to update nationality.");
                break;

            case 7: // Change Phone Number
                Console.WriteLine($"Current Phone Number: {account.PhoneNumber ?? "Not provided"}");
                Console.WriteLine("Enter new phone number:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newPhoneNumber: Console.ReadLine());
                Console.WriteLine(updateSuccessful
                    ? "Phone number updated successfully."
                    : "Failed to update phone number.");
                break;

            case 8: // Change Passport Details
                Console.WriteLine(
                    $"Current Passport Number: {account.PassportDetails?.PassportNumber ?? "Not provided"}");
                Console.WriteLine("Enter new passport number:");
                string passportNumber = Console.ReadLine() ?? string.Empty;

                Console.WriteLine(
                    $"Current Issue Date: {account.PassportDetails?.IssueDate?.ToString("yyyy-MM-dd") ?? "Not provided"}");
                Console.WriteLine("Enter new passport issue date (yyyy-mm-dd):");
                DateTime.TryParse(Console.ReadLine(), out DateTime issueDate);

                Console.WriteLine(
                    $"Current Expiration Date: {account.PassportDetails?.ExpirationDate?.ToString("yyyy-MM-dd") ?? "Not provided"}");
                Console.WriteLine("Enter new passport expiration date (yyyy-mm-dd):");
                DateTime.TryParse(Console.ReadLine(), out DateTime expirationDate);

                Console.WriteLine(
                    $"Current Country of Issue: {account.PassportDetails?.CountryOfIssue ?? "Not provided"}");
                Console.WriteLine("Enter new country of issue:");
                string countryOfIssue = Console.ReadLine() ?? string.Empty;

                var newPassportDetails =
                    new PassportDetailsModel(passportNumber, issueDate, expirationDate, countryOfIssue);
                updateSuccessful =
                    _userAccountService.ManageAccount(account.Id, newPassportDetails: newPassportDetails);
                Console.WriteLine(updateSuccessful
                    ? "Passport details updated successfully."
                    : "Failed to update passport details.");
                break;

            default:
                Console.WriteLine("Invalid option selected.");
                break;
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
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
        string? destination = Console.ReadLine() ?? string.Empty;

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

    private static void UseMilesForUpgrade()
    {
        Console.WriteLine("Enter the number of miles you want to use for the upgrade:");
        if (int.TryParse(Console.ReadLine(), out int miles) && miles > 0)
        {
            var seatUpgradeService = new SeatUpgradeService();
            var userAccountService = new UserAccountService();
            bool upgradeSuccess = seatUpgradeService.UseMilesForUpgrade(userAccountService.CurrentUserId, miles);

            Console.WriteLine(upgradeSuccess ? "Upgrade using miles successful!" : "Failed to use miles for upgrade. Please check your balance.");
        }
        else
        {
            Console.WriteLine("Invalid number of miles.");
        }
    }
}