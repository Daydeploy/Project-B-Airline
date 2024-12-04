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

    public bool UseMilesForUpgrade(int userId, int milesAmount)
    {
        var accountsLogic = new AccountsLogic();
        var account = accountsLogic.GetById(userId);
        if (account != null && account.Miles.Any(m => m.Points >= milesAmount))
        {
            var milesEntry = account.Miles.FirstOrDefault(m => m.Points >= milesAmount);

            if (milesEntry != null)
            {
                milesEntry.Points -= milesAmount;
                AccountsAccess.WriteAll(accountsLogic._accounts);
                return true;
            }
        }
        return false;
    }

    public bool ConfirmUpgrade(int userId)
    {
        return true;
    }

    public string ViewUpgradeBenefits(string seatClass)
    {
        return $"Benefits of {seatClass}: Better legroom, in-flight services, etc.";
    }

    private int CalculateMilesForUpgrade(string newSeatClass)
    {
        switch (newSeatClass)
        {
            case "Business Class":
                return 5000;
            case "First Class":
                return 10000;
            default:
                return 0;
        }
    }
}