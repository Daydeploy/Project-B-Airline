public static class AccountInformation
{
    public static void ViewAccountInformation()
    {
        Console.Clear();
        var accountsLogic = new AccountsLogic();
        var accounts = accountsLogic.GetAllAccounts();

        Console.WriteLine("=== View Account Information ===\n");
        foreach (var account in accounts)
        {
            Console.WriteLine($"ID: {account.Id}");
            Console.WriteLine($"Name: {account.FirstName} {account.LastName}");
            Console.WriteLine($"Email: {account.EmailAddress}");
            Console.WriteLine($"Date of Birth: {account.DateOfBirth:d}");
            Console.WriteLine(new string('-', 50));
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public static void AddNewAccount()
    {
        Console.Clear();
        Console.WriteLine("=== Add New Account ===\n");

        AccountManagement.CreateAccount();
    }

    public static void DeleteAccountInformation()
    {
        Console.Clear();
        var accountsLogic = new AccountsLogic();
        var accounts = accountsLogic.GetAllAccounts()
            .Where(a => !a.EmailAddress.ToLower().Equals("admin"))
            .ToList();

        Console.WriteLine("=== Delete Account Information ===\n");

        foreach (var account in accounts)
        {
            Console.WriteLine($"{account.Id}. {account.FirstName} {account.LastName} ({account.EmailAddress})");
        }

        Console.Write("\nEnter Account ID to delete (0 to cancel): ");
        if (!int.TryParse(Console.ReadLine(), out int accountId) || accountId == 0)
        {
            return;
        }

        var selectedAccount = accounts.FirstOrDefault(a => a.Id == accountId);
        if (selectedAccount == null)
        {
            Console.WriteLine("Account not found.");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            return;
        }

        Console.Write(
            $"\nAre you sure you want to delete account for {selectedAccount.FirstName} {selectedAccount.LastName}? (Y/N): ");
        if (Console.ReadLine()?.ToUpper() == "Y")
        {
            if (accountsLogic.DeleteAccount(accountId))
            {
                Console.WriteLine("Account deleted successfully!");
            }
            else
            {
                Console.WriteLine("Failed to delete account.");
            }
        }
        else
        {
            Console.WriteLine("Deletion cancelled.");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}