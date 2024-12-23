static class AdminAccountUI
{
    public static void ShowAdminMenu()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.CursorVisible = false;

        bool exit = false;
        while (!exit)
        {
            string[] menuItems = { "Manage airports", "Manage flights", "Manage accounts", "Logout" };
            int selectedIndex = MenuNavigationService.NavigateMenu(menuItems, "Admin Menu");
            HandleSelection(menuItems[selectedIndex], ref exit);
        }
    }

    static private void HandleSelection(string selectedOption, ref bool exit)
    {
        Console.Clear();
        switch (selectedOption)
        {
            case "Manage airports":
                ShowAirportMenu();
                break;
            case "Manage flights":
                ShowFlightMenu();
                break;
            case "Manage accounts":
                ShowAccountMenu();
                break;
            case "Logout":
                UserLogin.UserAccountServiceLogic.Logout();
                MenuNavigation.Start();
                exit = true;
                break;
        }
    }

    static private void HandleSelectionAirportMenu(string selectedOption, ref bool exit){
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
                AirportInformation.DeleteAirportInformation();
                break;
            case "Exit":
                exit = true;
                break;

        }
    }

    static private void HandleSelectionFlightMenu(string selectedOption, ref bool exit){
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
            string[] menuItems = { "View Airports", "Edit Airport Information", "Add New Airport", "Delete Airport Information", "Exit" };
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
            string[] menuItems = { "View Flights", "Edit Flight Information", "Add New Flight", "Delete Flight Information", "Exit" };
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
}   

