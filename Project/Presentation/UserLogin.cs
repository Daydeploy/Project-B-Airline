using System;
using System.Linq;

static class UserLogin
{
    static public UserAccountServiceLogic UserAccountServiceLogic = new UserAccountServiceLogic();
    private static bool _isLoggedIn = true;

    public static void Start()
    {
        AccountModel? acc = null;
        Console.WriteLine("Welcome to the login page");
        Console.WriteLine("Note: You can press F2 to toggle password visibility while typing.");
        Console.Write("Please enter your login details:\nEmail: ");
        string email = Console.ReadLine();
        // while (!IsValidEmail(email))
        // {
        //     Console.WriteLine("Invalid input. Please enter a valid email address.");
        //     Console.Write("Email: ");
        //     email = Console.ReadLine();
        // }

        string password = "";
        bool showPassword = false;

        Console.Write("Enter your password: ");
        password = ReadPassword(ref showPassword);

        acc = UserAccountServiceLogic.Login(email, password);

        if (acc != null)
        {
            Console.WriteLine($"Welcome back {acc.FirstName} {acc.LastName}");
            Console.WriteLine($"Your email is {acc.EmailAddress}");
            _isLoggedIn = true;
            MilesLogic.UpdateAllAccountLevels();
            ShowLoggedInMenu(acc);
        }
        else
        {
            Console.WriteLine("No account found with that email and password combination.");
            Console.WriteLine("\nPress any key to return to the main menu...");
            Console.ReadKey();  
            MenuNavigation.Start();
        }
    }

    private static void ShowLoggedInMenu(AccountModel account)
    {   
        if (account.EmailAddress.ToLower() == "admin")
        {
            AdminAccountUI.ShowAdminMenu();
            return;
        }
        
        string[] menuItems = new[]
        {
            "Book a Flight",
            "Book Private Jet",
            "View Booked Flights",
            "Check-in for a Flight",
            "Manage Account",
            "View Airport Information",
            "Browse Destinations",
            "Show Seat Upgrade Options",
            "Advanced flight booking",
            "Add Comfort Packages",
            "Add Entertainment",
            "Logout"
        };

        while (_isLoggedIn)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(menuItems, "Logged In Menu");

            switch (selectedIndex)
            {
                case 0: // Book a Flight
                    FlightManagement.BookAFlight(account);
                    break;
                case 1: // Book Private Jet
                    FlightManagement.BookPrivateJet(account.Id);
                    break;
                case 2: // View Booked Flights
                    FlightManagement.ViewBookedFlights(account.Id);
                    break;
                case 3: // Check-in for a Flight
                    FlightManagement.CheckInForFlight();
                    break;
                case 4: // Manage Account
                    AccountManagement.ManageAccount(account);
                    break;
                case 5: // View Airport Information
                    AirportInformation.ViewAirportInformation();
                    break;
                case 6: // Browse Destinations
                    AirportInformation.BrowseDestinations();
                    break;
                case 7: // Show Seat Upgrade Options
                    SeatUpgradeOptions.ShowSeatUpgradeOptions();
                    break;
                case 8: // Advanced flight booking
                    FlightManagement.ShowAvailableFlights();
                    break;
                case 9: // Add Comfort Packages
                    PackagesUI.ShowPackages();
                    break;
                case 10: // Add Entertainment
                    EntertainmentUI.ShowEntertainment();
                    break;
                case 11: // Logout
                    Console.Clear();
                    UserAccountServiceLogic.Logout();
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

    // public static string ReadPassword(ref bool showPassword)
    // {
    //     string pass = "";
    //     ConsoleKeyInfo key;

    //     do
    //     {
    //         key = Console.ReadKey(true);

    //         if (key.Key == ConsoleKey.F2)
    //         {
    //             showPassword = !showPassword;
    //             Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
    //             Console.Write("Enter your password: " + (showPassword ? pass : new string('*', pass.Length)));
    //         }
    //         else if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
    //         {
    //             pass = pass.Substring(0, pass.Length - 1);
    //             Console.Write("\b \b");
    //         }
    //         else if (key.Key != ConsoleKey.Enter)
    //         {
    //             pass += key.KeyChar;
    //             Console.Write(showPassword ? key.KeyChar : '*');
    //         }
    //     } while (key.Key != ConsoleKey.Enter);

    //     Console.WriteLine();

    //     if (!IsValidPassword(pass))
    //     {
    //         Console.WriteLine("Invalid password. Password must contain at least one uppercase letter, one number, and one special character.");
    //         Console.Write("Enter your password: ");
    //         return ReadPassword(ref showPassword);
    //     }


    //     return pass;
    // }
    private static string ReadPassword(ref bool showPassword)
    {
        string pass = "";
        ConsoleKeyInfo key;
    
        do
        {
            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.F2)
            {
                showPassword = !showPassword;
                Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
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

    private static bool IsValidPassword(string password)
    {
        return password.Any(char.IsUpper) &&
               password.Any(char.IsDigit) &&
               password.Any(ch => !char.IsLetterOrDigit(ch));
    }
}