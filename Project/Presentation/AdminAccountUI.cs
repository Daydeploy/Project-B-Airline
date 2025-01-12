static class AdminAccountUI
{
    public static void ShowAdminMenu()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        bool exit = false;
        while (!exit)
        {
            string[] menuItems = { "Manage Airports", "Manage Flights", "Manage Accounts", "Manage Finance", "Logout" };
            int selectedIndex = MenuNavigationService.NavigateMenu(menuItems, "Admin Menu");
            HandleSelection(menuItems[selectedIndex], ref exit);
        }
    }

    static private void HandleSelection(string selectedOption, ref bool exit)
    {
        Console.Clear();
        switch (selectedOption)
        {
            case "Manage Airports":
                ShowAirportMenu();
                break;
            case "Manage Flights":
                ShowFlightMenu();
                break;
            case "Manage Accounts":
                ShowAccountMenu();
                break;
            case "Manage Finance":
                FinanceUserUI.FinanceMainMenu();
                exit = true; 
                break;
            case "Logout":
                UserLogin.UserAccountServiceLogic.Logout();
                MenuNavigation.Start();
                exit = true;
                break;
        }
    }

    static private void HandleSelectionAirportMenu(string selectedOption, ref bool exit)
    {
        Console.Clear();
        switch (selectedOption)
        {
            case "View Airports":
                AirportInformation.ViewAirportInformation();
                break;
            case "Edit Airport Information":
                AirportInformation.EditAirportInformation();
                break;
            case "Add New Airport":
                AirportInformation.AddNewAirport();
                break;
            case "Delete Airport Information":
                DeleteAirport();
                break;
            case "Exit":
                exit = true;
                break;
        }
    }

    static private void HandleSelectionFlightMenu(string selectedOption, ref bool exit)
    {
        Console.Clear();
        switch (selectedOption)
        {
            case "View Flights":
                FlightManagement.ShowAvailableFlights();
                break;
            case "Edit Flight Information":
                FlightInformation.EditFlightInformation();
                break;
            case "Add New Flight":
                FlightInformation.AddNewFlight();
                break;
            case "Delete Flight Information":
                FlightInformation.DeleteFlightInformation();
                break;
            case "Exit":
                exit = true;
                break;
        }
    }

    static private void HandleSelectionAccountMenu(string selectedOption, ref bool exit)
    {
        Console.Clear();
        switch (selectedOption)
        {
            case "View Accounts":
                AccountInformation.ViewAccountInformation();
                break;
            // case "Edit Account Information":
            //     AccountInformation.EditAccountInformation();
            //     break;
            case "Add New Account":
                AccountInformation.AddNewAccount();
                break;
            case "Delete Account Information":
                AccountInformation.DeleteAccountInformation();
                break;
            case "Exit":
                exit = true;
                break;
        }
    }

    public static void ShowAirportMenu()
    {
        Console.CursorVisible = false;
        bool exit = false;

        while (!exit)
        {
            string[] menuItems =
            {
                "View Airports", "Edit Airport Information", "Add New Airport", "Delete Airport Information", "Exit"
            };
            int selectedIndex = MenuNavigationService.NavigateMenu(menuItems, "Airport Menu");

            if (selectedIndex >= 0 && selectedIndex < menuItems.Length)
            {
                HandleSelectionAirportMenu(menuItems[selectedIndex], ref exit);
            }
        }
    }

    public static void ShowFlightMenu()
    {
        Console.CursorVisible = false;
        bool exit = false;

        while (!exit)
        {
            string[] menuItems =
                { "View Flights", "Edit Flight Information", "Add New Flight", "Delete Flight Information", "Exit" };
            int selectedIndex = MenuNavigationService.NavigateMenu(menuItems, "Flight Menu");

            if (selectedIndex >= 0 && selectedIndex < menuItems.Length)
            {
                HandleSelectionFlightMenu(menuItems[selectedIndex], ref exit);
            }
        }
    }

    public static void ShowAccountMenu()
    {
        Console.CursorVisible = false;
        bool exit = false;

        while (!exit)
        {
            string[] menuItems = { "View Accounts", "Add New Account", "Delete Account Information", "Exit" };
            int selectedIndex = MenuNavigationService.NavigateMenu(menuItems, "Account Menu");

            if (selectedIndex >= 0 && selectedIndex < menuItems.Length)
            {
                HandleSelectionAccountMenu(menuItems[selectedIndex], ref exit);
            }
        }
    }

    public static void DeleteAirport()
    {
        Console.Clear();
        var airportLogic = new AirportLogic();
        var airports = airportLogic.GetAllAirports();

        if (!airports.Any())
        {
            Console.WriteLine("No airports available to delete.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Available Airports:");
        foreach (var airport in airports)
        {
            Console.WriteLine($"Code: {airport.Code} - Name: {airport.Name}");
        }

        Console.Write("\nEnter the airport code to delete: ");
        string airportCode = Console.ReadLine()?.Trim().ToUpper() ?? "";

        if (string.IsNullOrEmpty(airportCode))
        {
            Console.WriteLine("Invalid airport code.");
            return;
        }

        Console.Write("\nAre you sure you want to delete this airport? This will also delete all related flights (Y/N): ");
        if (Console.ReadLine()?.Trim().ToUpper() != "Y")
        {
            Console.WriteLine("Operation cancelled.");
            return;
        }

        bool success = airportLogic.DeleteAirport(airportCode);
        if (success)
        {
            Console.WriteLine("Airport and related flights successfully deleted.");
        }
        else
        {
            Console.WriteLine("Airport not found or could not be deleted.");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}