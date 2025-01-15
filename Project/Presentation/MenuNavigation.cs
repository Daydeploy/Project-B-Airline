using System.Text;

internal static class MenuNavigation
{
    public static void Start()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;
        string[] menuItems = { "Login", "Create Account", "Show available Flights", "Exit" };

        var exit = false;

        while (!exit)
        {
            Console.Clear();
            var selectedIndex =
                MenuNavigationServiceLogic.NavigateMenu(menuItems, MenuNavigationServiceLogic.AirlineLogo());
            HandleSelection(menuItems[selectedIndex], ref exit);
        }
    }

    private static void HandleSelection(string selectedOption, ref bool exit)
    {
        Console.Clear();
        switch (selectedOption)
        {
            case "Login":
                UserLogin.Start();
                break;
            case "Create Account":
                AccountManagement.CreateAccount();
                break;
            case "Show available Flights":
                FlightManagement.ShowAvailableFlights();
                break;
            case "Exit":
                exit = true;
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid input. Please try again.");
                break;
        }

        if (!exit)
        {
            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(true);
        }
    }
}