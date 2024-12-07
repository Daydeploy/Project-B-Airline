using System;

static class SeatUpgradeOptions
{
    // Displays seat upgrade options for a logged-in user
    public static void ShowSeatUpgradeOptions()
    {
        string[] upgradeOptions = new[]
        {
            "View Available Upgrades",
            "Request Upgrade",
            "Use Miles for Upgrade",
            "Confirm Upgrade",
            "View Upgrade Benefits",
            "Back to Main Menu"
        };

        while (true)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(upgradeOptions, "Seat Upgrade Options");
            if (selectedIndex == 5) break;

            switch (selectedIndex)
            {
                case 0:
                    ViewAvailableUpgrades();
                    break;
                case 1:
                    // Placeholder for RequestUpgrade()
                    break;
                case 2:
                    UseMilesForUpgrade();
                    break;
                case 3:
                    ConfirmUpgrade();
                    break;
                case 4:
                    ViewUpgradeBenefits();
                    break;
            }
        }
    }

    // Shows available seat upgrades for a specified flight ID
    private static void ViewAvailableUpgrades()
    {
        Console.WriteLine("Enter your flight ID to view available upgrades:");
        if (int.TryParse(Console.ReadLine(), out int flightId))
        {
            var seatUpgradeService = new SeatUpgradeServiceLogic();
            var availableUpgrades = seatUpgradeService.ViewAvailableUpgrades(flightId);

            if (availableUpgrades.Count > 0)
            {
                Console.WriteLine("Available upgrades:");
                foreach (var upgrade in availableUpgrades)
                {
                    Console.WriteLine(upgrade);
                }
            }
            else
            {
                Console.WriteLine("No available upgrades for this flight.");
            }
        }
        else
        {
            Console.WriteLine("Invalid flight ID.");
        }
    }

    // Uses miles to upgrade the seat for a user
    private static void UseMilesForUpgrade()
    {
        Console.WriteLine("Enter the number of miles you want to use for the upgrade:");
        if (int.TryParse(Console.ReadLine(), out int miles) && miles > 0)
        {
            var seatUpgradeService = new SeatUpgradeServiceLogic();
            bool upgradeSuccess = seatUpgradeService.UseMilesForUpgrade(UserLogin.UserAccountServiceLogic.CurrentUserId, miles);

            Console.WriteLine(upgradeSuccess
                ? "Upgrade using miles successful!"
                : "Failed to use miles for upgrade. Please check your balance.");
        }
        else
        {
            Console.WriteLine("Invalid number of miles.");
        }
    }

    // Confirms a seat upgrade
    private static void ConfirmUpgrade()
    {
        Console.WriteLine("Confirming upgrade...");
        Console.WriteLine("Upgrade confirmed successfully!");
    }

    // Shows benefits of upgrading to a specified seat class
    private static void ViewUpgradeBenefits()
    {
        Console.WriteLine("Enter the class you want to view benefits for:");
        string seatClass = Console.ReadLine() ?? string.Empty;

        var seatUpgradeService = new SeatUpgradeServiceLogic();
        string benefits = seatUpgradeService.ViewUpgradeBenefits(seatClass);
        Console.WriteLine(benefits);
    }
}
