using System;

static class MenuNavigation
{
    public static void Start()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
        string[] menuItems = { "Login", "Create Account", "Show available Flights", "Exit" };

        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            int selectedIndex = MenuNavigationService.NavigateMenu(menuItems, MenuNavigationService.AirlineLogo());
            HandleSelection(menuItems[selectedIndex], ref exit);
        }
    }

    static private void HandleSelection(string selectedOption, ref bool exit)
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