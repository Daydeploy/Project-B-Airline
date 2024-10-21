using System;
using System.Collections.Generic;

static class UserLogin
{
    static private UserAccountService _userAccountService = new UserAccountService();

    private static bool _isLoggedIn = true;

    public static void Start()
    {
        Console.WriteLine("Welcome to the login page");
        Console.WriteLine("Please enter your email address");
        string email = Console.ReadLine();
        Console.WriteLine("Please enter your password");
        string password = Console.ReadLine();
        AccountModel acc = _userAccountService.Login(email, password);
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

    private static void ShowLoggedInMenu(AccountModel account)
    {
        while (_isLoggedIn)
        {
            Console.WriteLine("\nLogged In Menu:");
            Console.WriteLine("1. View Booked Flights");
            Console.WriteLine("2. Check-in for a Flight");
            Console.WriteLine("3. Modify Booking");
            Console.WriteLine("4. Manage Account");
            Console.WriteLine("5. Show available Flights");
            Console.WriteLine("6. View Airport Information");
            Console.WriteLine("7. Logout");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ViewBookedFlights(account.Id);
                    break;
                case "2":
                    CheckInForFlight();
                    break;
                case "3":
                    ModifyBooking();
                    break;
                case "4":
                    ManageAccount(account);
                    break;
                case "5":
                    ShowAvailableFlights();
                    break;
                case "6":
                    ViewAirportInformation();
                    break;
                case "7":
                    Console.WriteLine("Logging out...");
                    _isLoggedIn = false;
                    Menu.Start();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
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
                Console.WriteLine($"{i + 1}. {passenger.Name} - Seat: {passenger.SeatNumber} - Checked Baggage: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");
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
            Console.WriteLine(
                $"ID: {airport.AirportID}, Name: {airport.Name}, Type: {airport.Type}");
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
        string input = Console.ReadLine();
        if (input == "y" || input == "Y" || input == "yes" || input == "Yes")
        {
            Menu.FilterFlightsByPriceUI();
        }

        // If user is logged allow the user to book a flight
        if (_userAccountService.IsLoggedIn == true)
        {
            Console.Clear();
            Console.WriteLine("Do you want to book a flight (y/n)");

            string input2 = Console.ReadLine().ToLower();

            foreach (var flight in flights.GetAllFlights())
            {
                // Display all flight information
                Console.WriteLine(
                    $"ID: {flight.FlightId} {flight.Origin} to {flight.Destination} at {flight.DepartureTime} for {flight.Price} EUR");
            }

            if (input2 == "y" || input2 == "yes")
            {
                Console.WriteLine("Enter the Flight ID to book:");
                if (int.TryParse(Console.ReadLine(), out int flightId))
                {
                    var selectedFlight = flights.GetAllFlights().FirstOrDefault(f => f.FlightId == flightId);
                    if (selectedFlight != null)
                    {
                        Console.WriteLine("How many passengers? (1-8):");
                        if (int.TryParse(Console.ReadLine(), out int passengerCount) && passengerCount > 0 && passengerCount <= 8)
                        {
                            var passengerDetails = new List<PassengerModel>();

                            for (int i = 0; i < passengerCount; i++)
                            {
                                Console.WriteLine($"\nPassenger {i + 1} Details:");

                                Console.WriteLine("Enter passenger name:");
                                string name = Console.ReadLine();

                                Console.WriteLine("Enter desired seat number:");
                                string seatNumber = Console.ReadLine();

                                Console.WriteLine("Does this passenger have checked baggage? (y/n):");
                                bool hasCheckedBaggage = Console.ReadLine().ToLower().StartsWith("y");

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
                                    Console.WriteLine($"Checked Baggage: {(passenger.HasCheckedBaggage ? "Yes" : "No")}");
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
}