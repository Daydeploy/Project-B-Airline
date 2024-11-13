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

        string password = "";
        bool showPassword = false;

        Console.Write("Enter your password: ");
        password = ReadPassword(ref showPassword);

        acc = _userAccountService.Login(email, password);

        if (acc != null)
        {
            Console.WriteLine($"Welcome back {acc.FirstName} {acc.LastName}");
            Console.WriteLine($"Your email is {acc.EmailAddress}");
            _isLoggedIn = true;
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

            if (key.Key == ConsoleKey.F2)
            {
                showPassword = !showPassword;
                Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
                Console.Write("Enter your password: " + (showPassword ? pass : new string('*', pass.Length)));
            }
            else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
            {
                pass = pass.Substring(0, pass.Length - 1);
                Console.Write("\b \b");
            }
            else if (key.Key != ConsoleKey.Enter)
            {
                pass += key.KeyChar;
                Console.Write(showPassword ? key.KeyChar : '*');
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return pass;
    }

    private static void ShowLoggedInMenu(AccountModel account)
    {
        string[] menuItems = new[]
        {
            "View Booked Flights",
            "Check-in for a Flight",
            "Manage Account",
            "Show Available Flights",
            "View Airport Information",
            "Browse Destinations",
            "Show Seat Upgrade Options",
            "Logout"
        };

        while (_isLoggedIn)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(menuItems, "Logged In Menu");

            switch (selectedIndex)
            {
                case 0:
                    ViewBookedFlights(account.Id);
                    break;
                case 1:
                    CheckInForFlight();
                    break;
                case 2:
                    ManageAccount(account);
                    break;
                case 3:
                    ShowAvailableFlights();
                    break;
                case 4:
                    ViewAirportInformation();
                    break;
                case 5:
                    BrowseDestinations();
                    break;
                case 6:
                    ShowSeatUpgradeOptions();
                    break;
                case 7:
                    Console.WriteLine("Logging out...");
                    Menu.Start();
                    _isLoggedIn = false;
                    return;
                
            }
        }
    }

    private static void ShowSeatUpgradeOptions()
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

        while (true)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(upgradeOptions, "Seat Upgrade Options");
            if (selectedIndex == 5) break;

            switch (selectedIndex)
            {
                case 0:
                    ViewAvailableUpgrades();
                    break;
                case 1: /* RequestUpgrade(); */ break;
                case 2:
                    UseMilesForUpgrade();
                    break;
                case 3:
                    ConfirmUpgrade();
                    break;
                case 4:
                    ViewUpgradeBenefits();
                    break;
            }
        }
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

        while (true)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(options, "Manage Account");

            if (selectedIndex == 10) return;
            if (selectedIndex == 9) DisplayAccountDetails(account);
            else HandleManageAccountOption(selectedIndex, account);
        }
    }

    private static void ViewBookedFlights(int userId)
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

            DrawBookingsTableHeader();

            foreach (var booking in bookings)
            {
                var flightsLogic = new FlightsLogic();
                var flight = flightsLogic.GetFlightsById(booking.FlightId);
                if (flight == null) continue;

                DisplayBookingDetails(booking, flight);
                DisplayPassengerDetails(booking.Passengers);
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

    private static void DrawBookingsTableHeader()
    {
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("BOOKED FLIGHTS DETAILS");
        Console.ResetColor();
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
    }

    private static void DisplayBookingDetails(BookingModel booking, FlightModel flight)
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

    private static void DisplayPassengerDetails(List<PassengerModel> passengers)
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

    private static void CheckInForFlight()
    {
        Console.WriteLine("Enter the Flight ID to check in:");
        if (int.TryParse(Console.ReadLine(), out int flightId))
        {
            bool success = _userAccountService.CheckIn(flightId);
            Console.WriteLine(
                success ? "Check-in successful." : "Check-in failed. Please try again or contact support.");
        }
        else
        {
            Console.WriteLine("Invalid Flight ID.");
        }
    }

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

    private static void ConfirmUpgrade()
    {
        Console.WriteLine("Confirming upgrade...");
        Console.WriteLine("Upgrade confirmed successfully!");
    }

    private static void ViewUpgradeBenefits()
    {
        string seatClass;
        do
        {
            Console.WriteLine("Enter the class you want to view benefits for:");
            seatClass = Console.ReadLine() ?? string.Empty;
        } while (string.IsNullOrWhiteSpace(seatClass));

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

    public static void ModifyPassengerDetails(int flightId, int passengerId)
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

    private static void DisplayAccountDetails(AccountModel account)
    {
        Console.WriteLine("\n--- Account Details ---");
        Console.WriteLine($"Email: {account.EmailAddress}");
        Console.WriteLine($"First Name: {account.FirstName}");
        Console.WriteLine($"Last Name: {account.LastName}");
        Console.WriteLine($"Date of Birth: {account.DateOfBirth:yyyy-MM-dd}");
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
            case 0:
                Console.WriteLine($"Current Email: {account.EmailAddress}");
                Console.WriteLine("Enter new email:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newEmail: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Email updated successfully." : "Failed to update email.");
                break;
            case 1:
                Console.WriteLine("Enter new password:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newPassword: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Password updated successfully." : "Failed to update password.");
                break;
            case 2:
                Console.WriteLine($"Current First Name: {account.FirstName}");
                Console.WriteLine("Enter new first name:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newFirstName: Console.ReadLine());
                Console.WriteLine(
                    updateSuccessful ? "First name updated successfully." : "Failed to update first name.");
                break;
            case 3:
                Console.WriteLine($"Current Last Name: {account.LastName}");
                Console.WriteLine("Enter new last name:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newLastName: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Last name updated successfully." : "Failed to update last name.");
                break;
            case 4:
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
            case 5:
                Console.WriteLine($"Current Gender: {account.Gender ?? "Not provided"}");
                Console.WriteLine("Enter new gender:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newGender: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Gender updated successfully." : "Failed to update gender.");
                break;
            case 6:
                Console.WriteLine($"Current Nationality: {account.Nationality ?? "Not provided"}");
                Console.WriteLine("Enter new nationality:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newNationality: Console.ReadLine());
                Console.WriteLine(updateSuccessful
                    ? "Nationality updated successfully."
                    : "Failed to update nationality.");
                break;
            case 7:
                Console.WriteLine($"Current Phone Number: {account.PhoneNumber ?? "Not provided"}");
                Console.WriteLine("Enter new phone number:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newPhoneNumber: Console.ReadLine());
                Console.WriteLine(updateSuccessful
                    ? "Phone number updated successfully."
                    : "Failed to update phone number.");
                break;
            case 8:
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
        while (true)
        {
            Console.Clear();
            var airportLogic = new AirportLogic();
            var airports = airportLogic.GetAllAirports();

            int currentIndex = 0;
            DisplayCurrentAirport();

            while (true)
            {
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.RightArrow:
                        if (currentIndex < airports.Count - 1)
                        {
                            currentIndex++;
                            DisplayCurrentAirport();
                        }
                        break;

                    case ConsoleKey.LeftArrow:
                        if (currentIndex > 0)
                        {
                            currentIndex--;
                            DisplayCurrentAirport();
                        }
                        break;

                    case ConsoleKey.Escape:
                        return;
                }
            }

            void DisplayCurrentAirport()
            {
                Console.Clear();
                Console.WriteLine("Navigate through airports using the following commands:");
                Console.WriteLine("ESC - Return to main menu");
                Console.WriteLine("← → Arrow keys - Navigate between airports");
                Console.WriteLine(new string('─', Console.WindowWidth - 1));

                Console.WriteLine($"\nAirport {currentIndex + 1} of {airports.Count}");
                AirportUI.DisplayAirportDetails(airports[currentIndex]);
            }
        }
    }

    public static void ShowAvailableFlights()
    {
        FlightsLogic flights = new FlightsLogic();
        var flightsList = flights.GetAllFlights().ToList();

        // Display flight table
        DisplayFlights(flightsList);

        Console.WriteLine("\nCommands:");
        Console.WriteLine("F - Filter flights");
        Console.WriteLine("B - Book a flight");
        Console.WriteLine("ESC - Go back");

        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            
            if (key.Key == ConsoleKey.Escape)
            {
                return;
            }
            
            if (key.Key == ConsoleKey.F)
            {
                Menu.FilterFlightsByPriceUI();
                return;
            }
            
            if (key.Key == ConsoleKey.B && _userAccountService.IsLoggedIn)
            {
                Console.Clear();
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
                            var passengerDetails = new List<PassengerModel>();
                            var seatSelector = new SeatSelectionUI();

                            // Load existing booked seats for this flight
                            var existingBookings = BookingLogic.GetBookingsForFlight(flightId);
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
                                if (seatNumber == null)
                                {
                                    Console.WriteLine("Seat selection cancelled.");
                                    return;
                                }

                                // Mark the seat as occupied for preventing double booking
                                seatSelector.SetSeatOccupied(seatNumber);

                                Console.WriteLine(
                                    $"\nSelected seat: {seatNumber} ({seatSelector.GetSeatClass(seatNumber)} Class)");

                                Console.WriteLine("Does this passenger have checked baggage? (y/n):");
                                bool hasCheckedBaggage = Console.ReadLine()?.ToLower().StartsWith("y") ?? false;

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
                                    flightId,
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
                                    Console.WriteLine(
                                        $"Seat: {passenger.SeatNumber} ({seatSelector.GetSeatClass(passenger.SeatNumber)} Class)");
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
                    Console.WriteLine("Press BACKSPACE to return to the menu...");
                }

                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                Console.WriteLine("Returning to the main menu...");
                return;
            }
        }
    }


    static public void DisplayFlights(List<FlightModel> flights)
    {
        if (!flights.Any())
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nNo flights found matching your criteria.");
            Console.ResetColor();
            return;
        }

        Console.WriteLine($"\nFound {flights.Count} flights matching your criteria:\n");

        // Draw header
        DrawTableHeader();

        foreach (var flight in flights)
        {
            DisplayFlightDetails(flight);
        }

        // Draw footer
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
    }

    static private void DrawTableHeader()
    {
        // Top border
        Console.WriteLine(new string('─', Console.WindowWidth - 1));

        // Header
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(
            $"{"Flight ID",-10} {"Route",-30} {"Departure",-18} {"Arrival",-18} {"Duration",-12} {"Prices (EUR)"}");
        Console.ResetColor();

        // Header-content separator
        Console.WriteLine(new string('─', Console.WindowWidth - 1));
    }

    static private void DisplayFlightDetails(FlightModel flight)
    {
        DateTime departureDateTime = DateTime.Parse(flight.DepartureTime);
        DateTime arrivalDateTime = DateTime.Parse(flight.ArrivalTime);
        TimeSpan duration = arrivalDateTime - departureDateTime;

        // Flight basic info with route formatting
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{flight.FlightId,-10} ");
        Console.ResetColor();

        // Route with arrow
        Console.Write($"{flight.Origin} ");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("→");
        Console.ResetColor();
        Console.Write($" {flight.Destination,-22} ");

        // Times
        Console.Write($"{departureDateTime:HH:mm dd MMM} ");
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("→");
        Console.ResetColor();
        Console.Write($" {arrivalDateTime:HH:mm dd MMM} ");

        // Duration
        string durationStr = $"{duration.Hours}h {duration.Minutes}m";
        Console.Write($"{durationStr,-12} ");

        // Price information
        foreach (var seatOption in flight.SeatClassOptions)
        {
            Console.ForegroundColor = GetPriceColor(seatOption.Class);
            Console.Write($"{seatOption.Class}: {seatOption.Price,4} ");
            Console.ResetColor();
        }

        Console.WriteLine();
    }

    static private ConsoleColor GetPriceColor(string seatClass)
    {
        return seatClass switch
        {
            "Economy" => ConsoleColor.Green,
            "Business" => ConsoleColor.Blue,
            "First" => ConsoleColor.Magenta,
            _ => ConsoleColor.Gray
        };
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
                        $"Flight ID: {flight.FlightId}, From: {flight.Origin}, Departure: {flight.DepartureTime}");

                    Console.WriteLine("Prices:");
                    foreach (var seatOption in flight.SeatClassOptions)
                    {
                        Console.WriteLine($"Class: {seatOption.Class}, Price: {seatOption.Price} EUR");
                    }
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

    private static void UseMilesForUpgrade()
    {
        Console.WriteLine("Enter the number of miles you want to use for the upgrade:");
        if (int.TryParse(Console.ReadLine(), out int miles) && miles > 0)
        {
            var seatUpgradeService = new SeatUpgradeService();
            bool upgradeSuccess = seatUpgradeService.UseMilesForUpgrade(_userAccountService.CurrentUserId, miles);

            Console.WriteLine(upgradeSuccess
                ? "Upgrade using miles successful!"
                : "Failed to use miles for upgrade. Please check your balance.");
        }
        else
        {
            Console.WriteLine("Invalid number of miles.");
        }
    }
}