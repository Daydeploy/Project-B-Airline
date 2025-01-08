using System.Collections.Generic;

public class SeatUpgradeService
{
    public List<string> ViewAvailableUpgrades(int flightId)
    {
        return new List<string> { "Business Class", "First Class" };
    }

    public bool RequestUpgrade(int userId, string newSeatClass)
    {
        UserAccountService userAccountService = new UserAccountService();   
        int requiredMiles = CalculateMilesForUpgrade(newSeatClass);
        if (userAccountService.GetCurrentMiles(userId) >= requiredMiles)
        {
            UseMilesForUpgrade(userId, requiredMiles);
        }
        return false;
    }

    // Method to use miles for an upgrade
    public bool UseMilesForUpgrade(int userId, int milesAmount)
    {
        var accountsLogic = new AccountsLogic(); // Create an instance
        var account = accountsLogic.GetById(userId); // Get the account by userId
        if (account != null && account.Miles >= milesAmount)
        {
            account.Miles -= milesAmount; // Deduct miles
            AccountsAccess.WriteAll(accountsLogic._accounts); // Save updated accounts
            return true; // Assume miles were successfully deducted
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