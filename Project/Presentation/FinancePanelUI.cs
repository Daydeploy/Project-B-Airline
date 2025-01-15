public class FinancePanelUI
{
    static public UserAccountServiceLogic UserAccountServiceLogic = new UserAccountServiceLogic();
    private static bool _isLoggedIn = true;

    public static void FinanceMainMenu()
    {
        string[] financeMenuOptions =
        {
            "Date",
            "Month",
            "Year",
            "Logout",
        };

        while (_isLoggedIn)
        {
            int selectedMenuIndex = MenuNavigationServiceLogic.NavigateMenu(financeMenuOptions, "Finance Panel Menu");

            switch (selectedMenuIndex)
            {
                case 0:
                    ShowDailyDataUI();
                    break;
                case 1:
                    ShowMonthlyDataUI();
                    break;
                case 2:
                    ShowYearlyDataUI();
                    break;
                case 3:
                    Console.Clear();
                    UserAccountServiceLogic.Logout();
                    MenuNavigation.Start();
                    _isLoggedIn = false;
                    return;
            }
        }
    }

    private static void DisplayFinancialMetrics(FinancePanelLogic.FinancialMetrics metrics)
    {
        Console.WriteLine("\nFinancial Summary");
        Console.WriteLine("----------------");
        Console.WriteLine($"Period: {metrics.StartDate:d} to {metrics.EndDate:d}");
        Console.WriteLine($"Total Revenue: €{metrics.TotalRevenue:N0}");
        Console.WriteLine($"Number of Bookings: {metrics.BookingCount}");
        Console.WriteLine($"Average Booking Value: €{metrics.AverageBookingValue:N0}");

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
        Console.Clear();
    }


    private static void ShowYearlyDataUI()
    {
        Console.Clear();
        Console.Write("Enter year (YYYY): ");
        if (int.TryParse(Console.ReadLine(), out int year))
        {
            var metrics = FinancePanelLogic.ShowYearlyData(year);
            DisplayFinancialMetrics(metrics);
        }
        else
        {
            Console.WriteLine("Invalid year format. Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    private static void ShowMonthlyDataUI()
    {
        Console.Clear();
        Console.Write("Enter year (YYYY): ");
        if (!int.TryParse(Console.ReadLine(), out int year))
        {
            Console.WriteLine("Invalid year format. Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        Console.Write("Enter month (1-12): ");
        if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
        {
            Console.WriteLine("Invalid month format. Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        var metrics = FinancePanelLogic.ShowMonthlyData(year, month);
        DisplayFinancialMetrics(metrics);
    }

    private static void ShowDailyDataUI()
    {
        Console.Clear();
        Console.Write("Enter date (DD/MM/YYYY): ");
        string userInput = Console.ReadLine();

        if (!System.Text.RegularExpressions.Regex.IsMatch(userInput, @"^\d{2}/\d{2}/\d{4}$"))
        {
            Console.WriteLine("Invalid date format. Please use DD/MM/YYYY format with '/' separators.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        if (DateTime.TryParse(userInput, out DateTime date))
        {
            if (date > DateTime.Now)
            {
                Console.WriteLine("Date must be in the past.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                Console.Clear();
                return;
            }

            var metrics = FinancePanelLogic.ShowDailyData(date);
            DisplayFinancialMetrics(metrics);
        }
        else
        {
            Console.WriteLine("Invalid date. Press any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}