using System;
using System.Linq;

public class AccountCreationUI
{
    public void CreateAccount()
    {
        try
        {
            Console.WriteLine("Enter your first name:");
            string firstName = Console.ReadLine();

            if (!AccountsLogic.IsValidFirstName(firstName))
            {
                Console.WriteLine("First name must be between 2 and 20 characters long, start with a capital letter, and cannot contain numbers.");
                return;
            }

            Console.WriteLine("Enter your last name:");
            string lastName = Console.ReadLine();

            Console.WriteLine("Enter your email address:");
            string email = Console.ReadLine();

            Console.WriteLine("Enter your password:");
            string password = Console.ReadLine();

            Console.WriteLine("Confirm your password:");
            string confirmPassword = Console.ReadLine();

            Console.WriteLine("Enter your date of birth (dd-MM-yyyy):");
            DateTime dateOfBirth = DateTime.ParseExact(Console.ReadLine(), "dd-MM-yyyy", null);

            Console.WriteLine("Would you like to enroll in our Frequent Flyer Program? (Y/N)");
            bool enrollFrequentFlyer = Console.ReadLine().Trim().ToUpper() == "Y";

            AccountsLogic.CreateAccount(firstName, lastName, email, password, confirmPassword, dateOfBirth, enrollFrequentFlyer);
            Console.WriteLine("Account created successfully! Please login.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}