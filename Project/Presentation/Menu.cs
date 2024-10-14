static class Menu
{
    static private UserAccountService _userAccountService = new UserAccountService();

    static public void Start()
    {
        while (true)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Create Account");
            Console.WriteLine("3. Exit");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    UserLogin.Start();
                    break;
                case "2":
                    CreateAccount();
                    break;
                case "3":
                    Console.WriteLine("Thank you for using our service. Goodbye!");
                    return;
                default:
                    Console.WriteLine("Invalid input. Please try again.");
                    break;
            }
        }
    }

    static private void CreateAccount()
    {
        Console.WriteLine("Create a new account");
        Console.WriteLine("Enter your email address:");
        string email = Console.ReadLine();
        Console.WriteLine("Enter your password:");
        string password = Console.ReadLine();
        Console.WriteLine("Enter your full name:");
        string fullName = Console.ReadLine();

        if (_userAccountService.CreateAccount(email, password, fullName))
        {
            Console.WriteLine("Account created successfully. Please login.");
        }
        else
        {
            Console.WriteLine("Failed to create account. Email may already be in use.");
        }
    }
}