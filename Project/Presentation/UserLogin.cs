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
        Console.WriteLine("Press ESC at any time to return to menu\n");

        bool showPassword = false;

        string email = GetUserInput("Email: ", false, ref showPassword);
        if (email == null)
        {
            MenuNavigation.Start();
            return;
        }

        string password = GetUserInput("Enter your password: ", true, ref showPassword);
        if (password == null)
        {
            MenuNavigation.Start();
            return;
        }

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

        if (account.Id == 0)
        {
            FinancePanelUI.FinanceMainMenu();
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
            "Finance panel",
            "Logout"
        };

        while (_isLoggedIn)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(menuItems, "Logged In Menu");

            switch (selectedIndex)
            {
                case 0: // Book a Flight
                    PaymentAndAccountInformationCheck(account);
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
                case 11: // Finance panel
                    FinanceUserUI.FinanceMainMenu();
                    break;
                case 12: // Logout
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

    public static void PaymentAndAccountInformationCheck(AccountModel account)
    {
        var accounts = AccountsAccess.LoadAll();
        account = accounts.FirstOrDefault(x => x.Id == account.Id);

        if (account.PaymentInformation == null)
        {
            Console.WriteLine("\nPayment information is required to complete a booking.");
            Console.WriteLine("\nWould you like to add payment information now? (Y/N)");

            string response = Console.ReadLine().ToUpper();

            if (response == "Y")
            {
                AccountManagement.HandleManageAccountOption(1, account);

                accounts = AccountsAccess.LoadAll();
                account = accounts.FirstOrDefault(x => x.Id == account.Id);

                if (account.PaymentInformation == null)
                {
                    Console.WriteLine("No payment information added, Booking cannot proceed.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Booking process cancelled due to missing payment information.");
                return;
            }
        }

        if (!AccountsLogic.HasCompleteContactInformation(account.FirstName, account.LastName, account.EmailAddress,
                account.PhoneNumber, account.Address))
        {
            Console.WriteLine("\nComplete contact information is required to complete a booking.");
            Console.WriteLine("Please update the following missing details:\n");

            if (string.IsNullOrEmpty(account.FirstName))
            {
                Console.WriteLine("- First Name");
            }

            if (string.IsNullOrEmpty(account.LastName))
            {
                Console.WriteLine("- Last Name");
            }

            if (string.IsNullOrEmpty(account.EmailAddress))
            {
                Console.WriteLine("- Email Address");
            }

            if (string.IsNullOrEmpty(account.PhoneNumber))
            {
                Console.WriteLine("- Phone Number");
            }

            if (string.IsNullOrEmpty(account.Address))
            {
                Console.WriteLine("- Address");
            }

            Console.WriteLine("Would you like to complete your contact information? (Y/N)");

            string response = Console.ReadLine().ToUpper();

            if (response == "Y")
            {
                AccountManagement.HandleManageAccountOption(0, account);

                accounts = AccountsAccess.LoadAll();
                account = accounts.FirstOrDefault(x => x.Id == account.Id);

                if (!AccountsLogic.HasCompleteContactInformation(account.FirstName, account.LastName,
                        account.EmailAddress, account.PhoneNumber, account.Address))
                {
                    Console.WriteLine("Contact information not updated completely. Booking cannot proceed.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("Booking process cancelled due to incomplete contact information.");
                return;
            }
        }

        FlightManagement.BookAFlight(account);
    }

    private static string GetUserInput(string prompt, bool isPassword, ref bool showPassword)
    {
        Console.Write(prompt);
        string input = "";
        ConsoleKeyInfo key;

        while (true)
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("\nReturning to menu...");
                return null;
            }

            if (key.Key == ConsoleKey.Enter && input.Length > 0)
            {
                Console.WriteLine();
                return input;
            }

            if (isPassword && key.Key == ConsoleKey.F2)
            {
                showPassword = !showPassword;
                Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                Console.Write(prompt + (showPassword ? input : new string('*', input.Length)));
                continue;
            }

            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                Console.Write(prompt);
                if (isPassword)
                    Console.Write(showPassword ? input : new string('*', input.Length));
                else
                    Console.Write(input);
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
                if (isPassword)
                    Console.Write(showPassword ? key.KeyChar : '*');
                else
                    Console.Write(key.KeyChar);
            }
        }
    }
}