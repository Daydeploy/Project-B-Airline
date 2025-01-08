public class FinanceUserUI
{
    private static FinanceUserLogic _financeLogic = new FinanceUserLogic();
    private const int MIN_YEAR = 2024;
    
    private static bool IsAdmin()
    {
        return UserLogin.UserAccountServiceLogic.CurrentAccount?.EmailAddress.ToLower() == "admin";
    }

    public static void FinanceMainMenu()
    {
        if (IsAdmin())
        {
            ShowAdminFinanceMenu();
            return;
        }

        string[] financeMenuOptions =
        {
            "View All Purchases",
            "View Purchases by Period",
            "View Recent Purchases",
            "View Spending Analysis",
            "Back to Main Menu"
        };

        while (true)
        {
            int selectedMenuIndex = MenuNavigationService.NavigateMenu(financeMenuOptions, "Personal Finance Panel");

            switch (selectedMenuIndex)
            {
                case 0:
                    ShowAllPurchases();
                    break;
                case 1:
                    ShowPurchasesByPeriod();
                    break;
                case 2:
                    ShowRecentPurchases();
                    break;
                case 3:
                    ShowSpendingAnalysis();
                    break;
                case 4:
                    return;
            }
        }
    }

    private static void ShowPurchaseDetails(BookingModel booking, bool showVAT = true)
    {
        Console.WriteLine($"\nBooking ID: {booking.BookingId}");
        Console.WriteLine($"Date: {booking.BookingDate:d}");
        Console.WriteLine($"Base Price: €{booking.TotalPrice:N2}");

        decimal vat = booking.TotalPrice * 0.21m; // 21% VAT
        decimal totalWithVAT = booking.TotalPrice + vat;

        if (booking.Passengers?.Any() == true)
        {
            foreach (var passenger in booking.Passengers)
            {
                if (passenger.ShopItems?.Any() == true)
                {
                    Console.WriteLine($"\nPurchases for {passenger.Name}:");
                    foreach (var item in passenger.ShopItems)
                    {
                        Console.WriteLine($"- {item.Name}: €{item.Price:N2}");
                    }
                }
            }
        }

        if (booking.Entertainment?.Any() == true)
        {
            Console.WriteLine("\nEntertainment Purchases:");
            foreach (var item in booking.Entertainment)
            {
                Console.WriteLine($"- {item.Name}: €{item.Cost:N2}");
            }
        }

        if (showVAT)
        {
            Console.WriteLine(new string('-', 40));
            Console.WriteLine($"Subtotal: €{booking.TotalPrice:N2}");
            Console.WriteLine($"VAT (21%): €{vat:N2}");
            Console.WriteLine($"Total with VAT: €{totalWithVAT:N2}");
        }
    }

    private static void ShowSpendingAnalysis()
    {
        var userId = UserLogin.UserAccountServiceLogic.CurrentUserId;
        var analysis = _financeLogic.GetSpendingAnalysis(userId);

        Console.Clear();
        Console.WriteLine("=== Spending Analysis ===\n");

        if (analysis.bookingCount == 0)
        {
            Console.WriteLine("No purchase history found.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"Total Lifetime Spending: €{analysis.totalSpent:N2}");
        Console.WriteLine($"Average Spending per Booking: €{analysis.avgPerBooking:N2}");
        Console.WriteLine($"Highest Single Purchase: €{analysis.mostExpensive:N2}");
        Console.WriteLine($"Most Frequent Travel Class: {analysis.mostFrequentClass}");
        Console.WriteLine($"Total Number of Bookings: {analysis.bookingCount}");

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void ShowPurchasesByPeriod()
    {
        string[] periodOptions =
        {
            "View by Year",
            "View by Quarter",
            "View by Month",
            "Back"
        };

        while (true)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(periodOptions, "Select Period View");

            switch (selectedIndex)
            {
                case 0:
                    ShowPurchasesByYear();
                    break;
                case 1:
                    ShowPurchasesByQuarter();
                    break;
                case 2:
                    ShowPurchasesByMonth();
                    break;
                case 3:
                    return;
            }
        }
    }

    private static void ShowPurchasesByPeriodAdmin()
    {
        string[] periodOptions =
        {
            "View by Year",
            "View by Quarter",
            "View by Month",
            "Back"
        };

        while (true)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(periodOptions, "Select Period View");

            switch (selectedIndex)
            {
                case 0:
                    ShowPurchasesByYear();
                    break;
                case 1:
                    ShowPurchasesByQuarter();
                    break;
                case 2:
                    ShowPurchasesByMonth();
                    break;
                case 3:
                    return;
            }
        }
    }

    private static void ShowPurchasesByYear()
    {
        int currentYear = DateTime.Now.Year;
        int selectedYear = currentYear;
        bool done = false;
        string message = "";
        bool isAdmin = IsAdmin();

        while (!done)
        {
            Console.Clear();
            Console.WriteLine($"=== Select Year ({(isAdmin ? "Admin View" : "User View")}) ===\n");
            Console.WriteLine("← → : Change Year    Enter : Confirm    Esc : Cancel\n");
            Console.WriteLine($"Selected Year: {selectedYear}");

            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine($"\n{message}");
                message = "";
            }

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (_financeLogic.IsValidYear(selectedYear - 1))
                        selectedYear--;
                    else
                        message = $"Cannot view years before {selectedYear}";
                    break;
                case ConsoleKey.RightArrow:
                    selectedYear++;
                    break;
                case ConsoleKey.Enter:
                    List<BookingModel> bookings;
                    if (isAdmin)
                    {
                        bookings = _financeLogic.GetAllBookingsByYear(selectedYear);
                        DisplayPurchases(bookings, $"All Purchases for {selectedYear} (Admin View)");
                    }
                    else
                    {
                        var userId = UserLogin.UserAccountServiceLogic.CurrentUserId;
                        bookings = _financeLogic.GetPurchasesByYear(userId, selectedYear);
                        DisplayPurchases(bookings, $"Purchases for {selectedYear}");
                    }
                    done = true;
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }
    private static void ShowPurchasesByQuarter()
    {
        int currentYear = DateTime.Now.Year;
        int selectedYear = currentYear;
        int selectedQuarter = (DateTime.Now.Month - 1) / 3 + 1;
        bool done = false;
        string message = "";
        bool isAdmin = IsAdmin();

        while (!done)
        {
            Console.Clear();
            Console.WriteLine($"=== Select Quarter ({(isAdmin ? "Admin View" : "User View")}) ===\n");
            Console.WriteLine("← → : Change Quarter    ↑ ↓ : Change Year");
            Console.WriteLine("Enter : Confirm        Esc : Cancel\n");
            Console.WriteLine($"Selected Period: Q{selectedQuarter} {selectedYear}");

            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine($"\n{message}");
                message = "";
            }

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (selectedQuarter > 1)
                        selectedQuarter--;
                    else if (selectedYear > MIN_YEAR)
                    {
                        selectedQuarter = 4;
                        selectedYear--;
                    }
                    else
                        message = $"Cannot view years before {MIN_YEAR}";

                    break;
                case ConsoleKey.RightArrow:
                    if (selectedQuarter < 4)
                        selectedQuarter++;
                    else
                    {
                        selectedQuarter = 1;
                        selectedYear++;
                    }

                    break;
                case ConsoleKey.UpArrow:
                    selectedYear++;
                    break;
                case ConsoleKey.DownArrow:
                    if (selectedYear > MIN_YEAR)
                        selectedYear--;
                    else
                        message = $"Cannot view years before {MIN_YEAR}";
                    break;
                case ConsoleKey.Enter:
                    List<BookingModel> bookings;
                    if (isAdmin)
                    {
                        bookings = _financeLogic.GetAllBookingsByQuarter(selectedYear, selectedQuarter);
                        DisplayPurchases(bookings, $"All Purchases for Q{selectedQuarter} {selectedYear} (Admin View)");
                    }
                    else
                    {
                        var userId = UserLogin.UserAccountServiceLogic.CurrentUserId;
                        bookings = _financeLogic.GetPurchasesByQuarter(userId, selectedYear, selectedQuarter);
                        DisplayPurchases(bookings, $"Purchases for Q{selectedQuarter} {selectedYear}");
                    }
                    done = true;
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    private static void ShowPurchasesByMonth()
    {
        int currentYear = DateTime.Now.Year;
        int currentMonth = DateTime.Now.Month;
        int selectedYear = currentYear;
        int selectedMonth = currentMonth;
        bool done = false;
        string message = "";
        bool isAdmin = IsAdmin();

        while (!done)
        {
            Console.Clear();
            Console.WriteLine($"=== Select Month ({(isAdmin ? "Admin View" : "User View")}) ===\n");
            Console.WriteLine("← → : Change Month    ↑ ↓ : Change Year");
            Console.WriteLine("Enter : Confirm      Esc : Cancel\n");
            Console.WriteLine($"Selected Date: {new DateTime(selectedYear, selectedMonth, 1):MMMM yyyy}");

            if (!string.IsNullOrEmpty(message))
            {
                Console.WriteLine($"\n{message}");
                message = "";
            }

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow:
                    if (selectedMonth > 1)
                        selectedMonth--;
                    else if (selectedYear > MIN_YEAR)
                    {
                        selectedMonth = 12;
                        selectedYear--;
                    }
                    else
                        message = $"Cannot view years before {MIN_YEAR}";

                    break;
                case ConsoleKey.RightArrow:
                    if (selectedMonth < 12)
                        selectedMonth++;
                    else
                    {
                        selectedMonth = 1;
                        selectedYear++;
                    }

                    break;
                case ConsoleKey.UpArrow:
                    selectedYear++;
                    break;
                case ConsoleKey.DownArrow:
                    if (selectedYear > MIN_YEAR)
                        selectedYear--;
                    else
                        message = $"Cannot view years before {MIN_YEAR}";
                    break;
                case ConsoleKey.Enter:
                    if (isAdmin)
                    {
                        var bookings = _financeLogic.GetAllBookingsByMonth(selectedYear, selectedMonth);
                        DisplayPurchases(bookings,
                            $"All Purchases for {new DateTime(selectedYear, selectedMonth, 1):MMMM yyyy} (Admin View)");
                    }
                    else
                    {
                        var userId = UserLogin.UserAccountServiceLogic.CurrentUserId;
                        var bookings = _financeLogic.GetPurchasesByMonth(userId, selectedYear, selectedMonth);
                        DisplayPurchases(bookings,
                            $"Purchases for {new DateTime(selectedYear, selectedMonth, 1):MMMM yyyy}");
                        done = true;
                    }
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    private static (DateTime startDate, DateTime endDate) GetQuarterDates(int year, int quarter)
    {
        var startMonth = (quarter - 1) * 3 + 1;
        var startDate = new DateTime(year, startMonth, 1);
        var endDate = startDate.AddMonths(3).AddDays(-1);
        return (startDate, endDate);
    }

    private static void ShowRecentPurchases(int count = 5)
    {
        var userId = UserLogin.UserAccountServiceLogic.CurrentUserId;
        var bookings = _financeLogic.GetRecentPurchases(userId, count);
        if (!bookings.Any())
        {
            Console.WriteLine("\nNo recent purchases found.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        DisplayPurchases(bookings, $"Last {count} Purchases");
    }

    private static void ShowAllPurchases()
    {
        var userId = UserLogin.UserAccountServiceLogic.CurrentUserId;
        var bookings = BookingAccess.LoadAll()
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ToList();

        DisplayPurchases(bookings, "All Purchases");
    }


    private static void ShowRecentPurchasesAdmin(int count = 5)
    {
        var userId = UserLogin.UserAccountServiceLogic.CurrentUserId;
        var bookings = _financeLogic.GetRecentPurchasesAdmin(count);
        if (!bookings.Any())
        {
            Console.WriteLine("\nNo recent purchases found.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        DisplayPurchases(bookings, $"Last {count} Purchases");
    }

    private static void ShowAllPurchasesAdmin()
    {
        var bookings = BookingAccess.LoadAll();
        DisplayPurchases(bookings, "All Purchases");
    }

    private static void DisplayPurchases(List<BookingModel> bookings, string title)
    {
        Console.Clear();
        Console.WriteLine($"=== {title} ===\n");

        if (!bookings.Any())
        {
            Console.WriteLine("No purchases found for this period.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        decimal totalAmount = 0;
        foreach (var booking in bookings)
        {
            ShowPurchaseDetails(booking);
            totalAmount += booking.TotalPrice;
            Console.WriteLine(new string('=', 40));
        }

        decimal totalVAT = totalAmount * 0.21m;
        decimal grandTotal = totalAmount + totalVAT;

        Console.WriteLine($"\nPeriod Summary:");
        Console.WriteLine($"Total Purchases: {bookings.Count}");
        Console.WriteLine($"Total Amount: €{totalAmount:N2}");
        Console.WriteLine($"Total VAT (21%): €{totalVAT:N2}");
        Console.WriteLine($"Grand Total: €{grandTotal:N2}");

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static void ShowAdminFinanceMenu()
    {
        string[] adminFinanceMenuOptions =
        {
            "View All Purchases",
            "View Purchases by Period",
            "View Recent Purchases",
            "View Spending Analysis",
            "View User Spending Analysis",
            "Back to Main Menu"
        };

        while (true)
        {
            int selectedMenuIndex = MenuNavigationService.NavigateMenu(adminFinanceMenuOptions, "Admin Finance Panel");

            switch (selectedMenuIndex)
            {
                case 0:
                    ShowAllPurchasesAdmin();
                    break;
                case 1:
                    ShowPurchasesByPeriodAdmin();
                    break;
                case 2:
                    ShowRecentPurchasesAdmin();
                    break;
                case 3:
                    ShowSpendingAnalysis();
                    break;
                case 4:
                    ShowUserSpendingAnalysis();
                    break;
                case 5:
                    return;
            }
        }
    }

    private static void ShowUserSpendingAnalysis()
    {
        var users = _financeLogic.GetAllUsers();

        if (!users.Any())
        {
            Console.WriteLine("\nNo users found in the system.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        int selectedIndex = 0;
        bool done = false;

        while (!done)
        {
            Console.Clear();
            Console.WriteLine("=== Select User to View Spending Analysis ===\n");
            Console.WriteLine("↑ ↓ : Navigate    Enter : Select    Esc : Cancel\n");

            // Display users with selection highlight
            for (int i = 0; i < users.Count; i++)
            {
                if (i == selectedIndex)
                    Console.Write("→ ");
                else
                    Console.Write("  ");

                Console.WriteLine($"{users[i].FirstName} {users[i].LastName} ({users[i].EmailAddress})");
            }

            var key = Console.ReadKey(true);
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    if (selectedIndex > 0)
                        selectedIndex--;
                    break;

                case ConsoleKey.DownArrow:
                    if (selectedIndex < users.Count - 1)
                        selectedIndex++;
                    break;

                case ConsoleKey.Enter:
                    ShowUserAnalysis(users[selectedIndex]);
                    done = true;
                    break;

                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    private static void ShowUserAnalysis(AccountModel user)
    {
        var analysis = _financeLogic.GetSpendingAnalysis(user.Id);

        Console.Clear();
        Console.WriteLine($"=== Spending Analysis for {user.FirstName} {user.LastName} ===\n");

        if (analysis.bookingCount == 0)
        {
            Console.WriteLine("No purchase history found for this user.");
        }
        else
        {
            Console.WriteLine($"Total Lifetime Spending: €{analysis.totalSpent:N2}");
            Console.WriteLine($"Average Spending per Booking: €{analysis.avgPerBooking:N2}");
            Console.WriteLine($"Highest Single Purchase: €{analysis.mostExpensive:N2}");
            Console.WriteLine($"Most Frequent Travel Class: {analysis.mostFrequentClass}");
            Console.WriteLine($"Total Number of Bookings: {analysis.bookingCount}");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}