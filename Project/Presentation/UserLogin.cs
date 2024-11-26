using System;

static class UserLogin
{
    static public UserAccountService _userAccountService = new UserAccountService();
    private static bool _isLoggedIn = true;

    // Entry point for user login
    public static void Start()
    {
        AccountModel? acc = null;
        Console.WriteLine("Welcome to the login page");
        Console.WriteLine("Note: You can press F2 to toggle password visibility while typing.");
        Console.Write("Please enter your login details:\nEmail: ");
        string email = Console.ReadLine();
        while (!IsValidEmail(email))
        {
            Console.WriteLine("Invalid input. Please enter a valid email address.");
            Console.Write("Email: ");
            email = Console.ReadLine();
        }

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
            MenuNavigation.Start();
        }
    }

    // Method to read password input with optional visibility toggle
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

    // Displays the menu options available to a logged-in user
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
                    FlightManagement.ViewBookedFlights(account.Id);
                    break;
                case 1:
                    FlightManagement.CheckInForFlight();
                    break;
                case 2:
                    AccountManagement.ManageAccount(account);
                    break;
                case 3:
                    FlightManagement.ShowAvailableFlights();
                    break;
                case 4:
                    AirportInformation.ViewAirportInformation();
                    break;
                case 5:
                    AirportInformation.BrowseDestinations();
                    break;
                case 6:
                    SeatUpgradeOptions.ShowSeatUpgradeOptions();
                    break;
                case 7:
                    Console.Clear();
                    _userAccountService.Logout();
                    Console.WriteLine("You have successfully logged out.");
                    Console.WriteLine("Returning to the main menu...");
                    MenuNavigation.Start();
                    _isLoggedIn = false;
                    return;
            }
        }
    }

    private static bool IsValidEmail(string email)
    {
        return email.Contains("@") && email.Contains(".");
    }
}