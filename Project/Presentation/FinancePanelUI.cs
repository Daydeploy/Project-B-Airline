public class FinancePanelUI
{
    static public UserAccountServiceLogic UserAccountServiceLogic = new UserAccountServiceLogic();
    private static bool _isLoggedIn = true;
    public static void FinanceMainMenu()
    {
        string[] financeMenuOptions = {
            "Year",
            "Month",
            "Day",
            "Logout",
        };

        while (_isLoggedIn)
        {
            int selectedMenuIndex = MenuNavigationService.NavigateMenu(financeMenuOptions, "Finance Panel Menu");

            switch (selectedMenuIndex)
            {
                case 0:
                    Console.WriteLine("Year");
                    break;
                case 1:
                    Console.WriteLine("Month");
                    break;
                case 2:
                    Console.WriteLine("Day");
                    break;
                case 3:
                    Console.Clear();
                    UserAccountServiceLogic.Logout();
                    Console.WriteLine("You have successfully logged out.\nReturning to the main menu....");
                    MenuNavigation.Start();
                    _isLoggedIn = false;
                    return;
            }
        }
    }
}