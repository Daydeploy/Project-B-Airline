using System.Text;

internal static class AdminAccountUI
{
    public static void ShowAdminMenu()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        var exit = false;
        while (!exit)
        {
            string[] menuItems = { "Manage Airports", "Manage Flights", "Manage Accounts", "Manage Finance", "Logout" };
            var selectedIndex = MenuNavigationServiceLogic.NavigateMenu(menuItems, "Admin Menu");
            HandleSelection(menuItems[selectedIndex], ref exit);
        }
    }

    private static void HandleSelection(string selectedOption, ref bool exit)
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

    private static void HandleSelectionAirportMenu(string selectedOption, ref bool exit)
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
            case "Exit":
                exit = true;
                break;
        }
    }

    private static void HandleSelectionFlightMenu(string selectedOption, ref bool exit)
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

    private static void HandleSelectionAccountMenu(string selectedOption, ref bool exit)
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
        var exit = false;

        while (!exit)
        {
            string[] menuItems =
            {
                "View Airports", "Edit Airport Information", "Add New Airport", "Exit"
            };
            var selectedIndex = MenuNavigationServiceLogic.NavigateMenu(menuItems, "Airport Menu");

            if (selectedIndex >= 0 && selectedIndex < menuItems.Length)
                HandleSelectionAirportMenu(menuItems[selectedIndex], ref exit);
        }
    }

    public static void ShowFlightMenu()
    {
        Console.CursorVisible = false;
        var exit = false;

        while (!exit)
        {
            string[] menuItems =
                { "View Flights", "Edit Flight Information", "Add New Flight", "Delete Flight Information", "Exit" };
            var selectedIndex = MenuNavigationServiceLogic.NavigateMenu(menuItems, "Flight Menu");

            if (selectedIndex >= 0 && selectedIndex < menuItems.Length)
                HandleSelectionFlightMenu(menuItems[selectedIndex], ref exit);
        }
    }

    public static void ShowAccountMenu()
    {
        Console.CursorVisible = false;
        var exit = false;

        while (!exit)
        {
            string[] menuItems = { "View Accounts", "Add New Account", "Delete Account Information", "Exit" };
            var selectedIndex = MenuNavigationServiceLogic.NavigateMenu(menuItems, "Account Menu");

            if (selectedIndex >= 0 && selectedIndex < menuItems.Length)
                HandleSelectionAccountMenu(menuItems[selectedIndex], ref exit);
        }
    }
}