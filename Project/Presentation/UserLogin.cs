using System;
using System.Collections.Generic;

static class UserLogin
{
    static private UserAccountService _userAccountService = new UserAccountService();

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
        while (true)
        {
            Console.WriteLine("\nLogged In Menu:");
            Console.WriteLine("1. View Booked Flights");
            Console.WriteLine("2. Check-in for a Flight");
            Console.WriteLine("3. Modify Booking");
            Console.WriteLine("4. Manage Account");
            Console.WriteLine("5. View Airport Information");
            Console.WriteLine("6. Logout");

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
                    ViewAirportInformation();
                    break;
                case "6":
                    Console.WriteLine("Logging out...");
                    Menu.Start();
                    return;
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
                Console.WriteLine($"Flight ID: {flight.FlightId}, Number: {flight.FlightNumber}, Departure: {flight.DepartureTime}, Arrival: {flight.ArrivalTime}");
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
            Console.WriteLine("Enter new seat number:");
            string seatNumber = Console.ReadLine();
            Console.WriteLine("Do you have checked baggage? (y/n):");
            bool hasCheckedBaggage = Console.ReadLine().ToLower() == "y";

            var newDetails = new BookingDetails
            {
                SeatNumber = seatNumber,
                HasCheckedBaggage = hasCheckedBaggage
            };

            bool success = _userAccountService.ModifyBooking(flightId, newDetails);
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
            Console.WriteLine($"ID: {airport.AirportID}, Name: {airport.Name}, Type: {airport.Type}, Luxurious: {airport.IsLuxurious}");
        }

        Console.WriteLine("\nPress any key to return to the menu...");
        Console.ReadKey();
    }
}