using System.Collections.Generic;

public class SeatUpgradeService
{
    // Method to view available upgrades for a specific flight
    public List<string> ViewAvailableUpgrades(int flightId)
    {
        // Logic to fetch available higher-class seats based on the user's current seat
        // This is a placeholder implementation
        return new List<string> { "Business Class", "First Class" };
    }

    // Method to request an upgrade
    public bool RequestUpgrade(int userId, string newSeatClass)
    {
        UserAccountService userAccountService = new UserAccountService();
        // Check if the user has enough miles or if they want to pay
        int requiredMiles = CalculateMilesForUpgrade(newSeatClass);
        if (userAccountService.GetCurrentMiles(userId) >= requiredMiles)
        {
            UseMilesForUpgrade(userId, requiredMiles);
            // Proceed with the upgrade
        }
        // Additional logic for payment or upgrade confirmation
        return false; // Upgrade request failed
    }

    // Method to use miles for an upgrade
    public bool UseMilesForUpgrade(int userId, int milesAmount)
    {
        var accountsLogic = new AccountsLogic(); // Create an instance
        var account = accountsLogic.GetById(userId); // Get the account by userId
        if (account != null && account.Miles.Any(m => m.Points >= milesAmount))
        {
            var milesEntry = account.Miles.FirstOrDefault(m => m.Points >= milesAmount);

            if (milesEntry != null)
            {
                milesEntry.Points -= milesAmount;
                AccountsAccess.WriteAll(accountsLogic._accounts); // Save updated accounts
                return true;
            }
        }
        return false; // Not enough miles
    }

    // Method to confirm the upgrade
    public bool ConfirmUpgrade(int userId)
    {
        // Logic to confirm the upgrade and update the user's seat in the booking
        return true; // Assume confirmation is successful
    }

    // Method to view upgrade benefits
    public string ViewUpgradeBenefits(string seatClass)
    {
        // Logic to display benefits of the higher-class seat
        return $"Benefits of {seatClass}: Better legroom, in-flight services, etc.";
    }

    private int CalculateMilesForUpgrade(string newSeatClass)
    {
        // Define miles required for each seat class
        switch (newSeatClass)
        {
            case "Business Class":
                return 5000; // Example miles for Business Class
            case "First Class":
                return 10000; // Example miles for First Class
            default:
                return 0; // No miles required for other classes
        }
    }
}