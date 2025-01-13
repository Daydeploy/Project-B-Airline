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
            // "Show Seat Upgrade Options",
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
                // case 7: // Show Seat Upgrade Options
                //     SeatUpgradeOptions.ShowSeatUpgradeOptions();
                //     break;
                case 7: // Advanced flight booking
                    FlightManagement.ShowAvailableFlights();
                    break;
                case 8: // Add Comfort Packages
                    PackagesUI.ShowPackages();
                    break;
                case 9: // Add Entertainment
                    EntertainmentUI.ShowEntertainment();
                    break;
                case 10: // Finance panel
                    FinanceUserUI.FinanceMainMenu();
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

    public static void PaymentAndAccountInformationCheck(AccountModel account)
    {
        var accounts = AccountsAccess.LoadAll();
        account = accounts.FirstOrDefault(x => x.Id == account.Id);
        bool updateNeeded = false;

        if (string.IsNullOrEmpty(account.PhoneNumber) || string.IsNullOrEmpty(account.Address) || account.PaymentInformation == null)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n╔════════════════════════════════════╗");
            Console.WriteLine("║      PERSONAL INFORMATION          ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");
            Console.ResetColor();
        }


        if (string.IsNullOrEmpty(account.PhoneNumber))
        {
            Console.WriteLine("\nPhone number is required to complete booking.");
            Console.Write("Enter your phone number: ");
            string phoneNumber;
            do
            {
                phoneNumber = Console.ReadLine()?.Trim();
                if (!AccountsLogic.IsValidPhoneNumber(phoneNumber))
                {
                    Console.WriteLine("Invalid phone number. Must be between 10-15 digits.");
                    Console.Write("Enter your phone number: ");
                }
            } while (!AccountsLogic.IsValidPhoneNumber(phoneNumber));

            UserAccountServiceLogic.ManageAccount(account.Id, newPhoneNumber: phoneNumber);
            updateNeeded = true;
        }

        if (string.IsNullOrEmpty(account.Address))
        {
            Console.WriteLine("\nAddress is required to complete booking.");
            Console.Write("Enter your address: ");
            string address = Console.ReadLine()?.Trim();
            while (string.IsNullOrWhiteSpace(address))
            {
                Console.WriteLine("Address cannot be empty.");
                Console.Write("Enter your address: ");
                address = Console.ReadLine()?.Trim();
            }

            UserAccountServiceLogic.ManageAccount(account.Id, newAddress: address);
            updateNeeded = true;
        }

        if (account.PaymentInformation == null || !account.PaymentInformation.Any())
        {
            Console.WriteLine("\nPayment information is required to complete booking.");
            Console.WriteLine("Would you like to:");
            Console.WriteLine("1. Add and save payment information for future bookings");
            Console.WriteLine("2. Use payment information only for this booking");
            Console.WriteLine("3. Cancel booking");

            string choice;
            do
            {
                Console.Write("\nEnter your choice (1-3): ");
                choice = Console.ReadLine()?.Trim();
            } while (choice != "1" && choice != "2" && choice != "3");

            if (choice == "3")
            {
                Console.WriteLine("Booking process cancelled.");
                return;
            }

            bool saveForFuture = choice == "1";

            Console.Write("\nEnter Card Holder Name: ");
            string cardHolder;
            do
            {
                cardHolder = Console.ReadLine()?.Trim();
                if (!PaymentLogic.ValidateName(cardHolder))
                {
                    Console.WriteLine("Card Holder Name cannot be empty.");
                    Console.Write("Enter Card Holder Name: ");
                }
            } while (!PaymentLogic.ValidateName(cardHolder));
            Console.Write("\nEnter Card Number (16 digits): ");
            string cardNumber;
            do
            {
                cardNumber = Console.ReadLine()?.Trim();
                if (!PaymentLogic.ValidateCardNumber(cardNumber))
                {
                    Console.WriteLine("Invalid card number. Must be 16 digits.");
                    Console.Write("Enter Card Number: ");
                }
            } while (!PaymentLogic.ValidateCardNumber(cardNumber));

            Console.Write("\nEnter CVV (3 or 4 digits): ");
            string cvv;
            do
            {
                cvv = Console.ReadLine()?.Trim();
                if (!PaymentLogic.ValidateCVV(cvv))
                {
                    Console.WriteLine("Invalid CVV. Must be 3 or 4 digits.");
                    Console.Write("Enter CVV: ");
                }
            } while (!PaymentLogic.ValidateCVV(cvv));

            Console.Write("\nEnter Expiration Date (MM/YY): ");
            string expirationDate;
            do
            {
                expirationDate = Console.ReadLine()?.Trim();
                if (!PaymentLogic.ValidateExpirationDate(expirationDate))
                {
                    Console.WriteLine("Invalid expiration date. Use MM/YY format.");
                    Console.Write("Enter Expiration Date: ");
                }
            } while (!PaymentLogic.ValidateExpirationDate(expirationDate));

            Console.Write("\nEnter Billing Address: ");
            string billingAdress;
            do
            {
                billingAdress = Console.ReadLine()?.Trim();
                if (!PaymentLogic.ValidateAddress(billingAdress))
                {
                    Console.WriteLine("Invalid billing address, Cannot be empty.");
                    Console.Write("Enter Billing Address: ");
                }
            } while (!PaymentLogic.ValidateAddress(billingAdress));

            var paymentInfo = new PaymentInformationModel(cardHolder, cardNumber, cvv, expirationDate, billingAdress);

            if (saveForFuture)
            {
                account.PaymentInformation = new List<PaymentInformationModel> { paymentInfo };
                updateNeeded = true;
                Console.WriteLine("\nPayment information saved for future bookings.");
            }
            else
            {
                account.TemporaryPaymentInfo = paymentInfo;
                Console.WriteLine("\nPayment information will be used only for this booking.");
            }
        }

        if (updateNeeded)
        {
            UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newPaymentInformation: account.PaymentInformation);
            Console.WriteLine("\nInformation updated successfully!");
        }

        if (string.IsNullOrEmpty(account.PhoneNumber) || string.IsNullOrEmpty(account.Address) || account.PaymentInformation == null)
        {
            Console.WriteLine("\nPress any key to continue with booking...");
            Console.ReadKey();
            Console.Clear();
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