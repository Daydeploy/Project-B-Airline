using System;

static class MenuNavigation
{
    static private UserAccountService _userAccountService = new UserAccountService();

    // Main entry point for displaying the main menu
    public static void Start()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;
        string[] menuItems = { "Login", "Create Account", "Show available Flights", "Exit" };

        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            int selectedIndex = MenuNavigationService.NavigateMenu(menuItems, AirlineLogo());
            HandleSelection(menuItems[selectedIndex], ref exit);
        }
    }

    // Handles selection of menu options
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

    // Displays the airline logo at the top of the menu
    public static string AirlineLogo()
    {
        return @"
 --------------------------------------------------------------------------------------------------------------------------------------------------------
|                                                                                                                                                        |
| d8888b.  .d88b.  d888888b d888888b d88888b d8888b. d8888b.  .d8b.  .88b  d88.       .d8b.  d888888b d8888b. db      d888888b d8b   db d88888b .d8888.  |
| 88  `8D .8P  Y8. `~~88~~' `~~88~~' 88'     88  `8D 88  `8D d8' `8b 88'YbdP`88      d8' `8b   `88'   88  `8D 88        `88'   888o  88 88'     88'  YP  | 
| 88oobY' 88    88    88       88    88ooooo 88oobY' 88   88 88ooo88 88  88  88      88ooo88    88    88oobY' 88         88    88V8o 88 88ooooo `8bo.    |
| 88`8b   88    88    88       88    88~~~~~ 88`8b   88   88 88~~~88 88  88  88      88~~~88    88    88`8b   88         88    88 V8o88 88~~~~~   `Y8b.  | 
| 88 `88. `8b  d8'    88       88    88.     88 `88. 88  .8D 88   88 88  88  88      88   88   .88.   88 `88. 88booo.   .88.   88  V888 88.     db   8D  |
| 88   YD  `Y88P'     YP       YP    Y88888P 88   YD Y8888D' YP   YP YP  YP  YP      YP   YP Y888888P 88   YD Y88888P Y888888P VP   V8P Y88888P `8888Y'  |
|                                                                                                                                                        |
 --------------------------------------------------------------------------------------------------------------------------------------------------------
";
    }
}
