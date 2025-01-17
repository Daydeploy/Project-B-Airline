public class SeatUpgradeServiceLogic
{
    private readonly IAccountsAccess _accountsAccess = new AccountsAccess();
    public List<string> ViewAvailableUpgrades(int flightId)
    {
        return new List<string> { "Business Class", "First Class" };
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
                _accountsAccess.WriteAll(accountsLogic._accounts);
                return true;
            }
        }

        return false;
    }

    public string ViewUpgradeBenefits(string seatClass)
    {
        return $"Benefits of {seatClass}: Better legroom, in-flight services, etc.";
    }
}