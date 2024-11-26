using System;
using System.Collections.Generic;

static class AccountManagement
{
    // Manages the user's account, providing options to update various details
    public static void ManageAccount(AccountModel account)
    {
        string[] options =
        {
            "Change Email",
            "Change Password",
            "Change First Name",
            "Change Last Name",
            "Change Date of Birth",
            "Change Gender",
            "Change Nationality",
            "Change Phone Number",
            "Change Passport Details",
            "View Account Details",
            "Back to Main Menu"
        };

        while (true)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(options, "Manage Account");

            if (selectedIndex == 10) return;
            if (selectedIndex == 9) DisplayAccountDetails(account);
            else HandleManageAccountOption(selectedIndex, account);
        }
    }

    // Prompts the user to create a new account and handles validation
    public static void CreateAccount()
    {
        Console.WriteLine("Note: You can press F2 to toggle password visibility while typing.\n");
        Console.WriteLine("Create a new account");

        Console.WriteLine("Enter your first name:");
        string firstName = Console.ReadLine();
        Console.WriteLine("Enter your last name:");
        string lastName = Console.ReadLine();

        Console.WriteLine("Enter your email address:");
        string email = Console.ReadLine().Trim();

        while (true)
        {
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                Console.WriteLine("Error: Email must contain '@' and a domain (For instance: '.com').");
                Console.Write("Please enter your email address again: ");
                email = Console.ReadLine().Trim();
            }
            else
            {
                break;
            }
        }

        string password = "";
        string confirmPassword = "";
        bool showPassword = false;

        Console.Write("Enter your password: ");
        password = UserLogin.ReadPassword(ref showPassword);

        Console.Write("Confirm your password: ");
        confirmPassword = UserLogin.ReadPassword(ref showPassword);

        if (password != confirmPassword)
        {
            Console.WriteLine("\nPasswords do not match. Please try again.");
            return;
        }

        Console.WriteLine("Enter your date of birth (dd-MM-yyyy):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateOfBirth))
        {
            Console.WriteLine("Invalid date format. Please try again.");
            return;
        }

        bool accountCreated = UserLogin._userAccountService.CreateAccount(email, password, firstName, lastName, dateOfBirth);

        Console.WriteLine(accountCreated
            ? "Account created successfully. Please login."
            : "Failed to create account. Email may already be in use.");
    }

    // Displays the account details for the user
    private static void DisplayAccountDetails(AccountModel account)
    {
        Console.WriteLine("\n--- Account Details ---");
        Console.WriteLine($"Email: {account.EmailAddress}");
        Console.WriteLine($"First Name: {account.FirstName}");
        Console.WriteLine($"Last Name: {account.LastName}");
        Console.WriteLine($"Date of Birth: {account.DateOfBirth:dd-MM-yyyy}");
        Console.WriteLine($"Gender: {account.Gender ?? "Not provided"}");
        Console.WriteLine($"Nationality: {account.Nationality ?? "Not provided"}");
        Console.WriteLine($"Phone Number: {account.PhoneNumber ?? "Not provided"}");

        if (account.PassportDetails != null)
        {
            Console.WriteLine("Passport Details:");
            Console.WriteLine($"  Passport Number: {account.PassportDetails.PassportNumber ?? "Not provided"}");
            Console.WriteLine($"  Issue Date: {account.PassportDetails.IssueDate?.ToString("dd-MM-yyyy") ?? "Not provided"}");
            Console.WriteLine($"  Expiration Date: {account.PassportDetails.ExpirationDate?.ToString("dd-MM-yyyy") ?? "Not provided"}");
            Console.WriteLine($"  Country of Issue: {account.PassportDetails.CountryOfIssue ?? "Not provided"}");
        }
        else
        {
            Console.WriteLine("Passport Details: Not provided");
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    // Handles changes to various account properties
    private static void HandleManageAccountOption(int optionIndex, AccountModel account)
    {
        bool updateSuccessful = false;

        switch (optionIndex)
        {
            case 0:
                Console.WriteLine($"Current Email: {account.EmailAddress}");
                Console.WriteLine("Enter new email:");
                updateSuccessful = UserLogin._userAccountService.ManageAccount(account.Id, newEmail: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Email updated successfully." : "Failed to update email.");
                break;
            case 1:
                Console.WriteLine("Enter new password:");
                updateSuccessful = UserLogin._userAccountService.ManageAccount(account.Id, newPassword: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Password updated successfully." : "Failed to update password.");
                break;
            case 2:
                Console.WriteLine($"Current First Name: {account.FirstName}");
                Console.WriteLine("Enter new first name:");
                string newFirstName = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(newFirstName))
                {
                    Console.WriteLine("Invalid input. Please enter a valid first name.");
                    Console.Write("Enter new first name: ");
                    newFirstName = Console.ReadLine();
                }
                updateSuccessful = UserLogin._userAccountService.ManageAccount(account.Id, newFirstName: newFirstName);
                Console.WriteLine(
                    updateSuccessful ? "First name updated successfully." : "Failed to update first name.");
                break;
            case 3:
                Console.WriteLine($"Current Last Name: {account.LastName}");
                Console.WriteLine("Enter new last name:");
                updateSuccessful = UserLogin._userAccountService.ManageAccount(account.Id, newLastName: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Last name updated successfully." : "Failed to update last name.");
                break;
            case 4:
                Console.WriteLine($"Current Date of Birth: {account.DateOfBirth:dd-MM-yyyy}");
                Console.WriteLine("Enter new date of birth (dd-MM-yyyy):");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime newDateOfBirth))
                {
                    updateSuccessful = UserLogin._userAccountService.ManageAccount(account.Id, newDateOfBirth: newDateOfBirth);
                    Console.WriteLine(updateSuccessful
                        ? "Date of birth updated successfully."
                        : "Failed to update date of birth.");
                }
                else
                {
                    Console.WriteLine("Invalid date format. Date of birth not updated.");
                }
                break;
            case 5:
                Console.WriteLine($"Current Gender: {account.Gender ?? "Not provided"}");
                Console.WriteLine("Enter new gender:");
                updateSuccessful = UserLogin._userAccountService.ManageAccount(account.Id, newGender: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Gender updated successfully." : "Failed to update gender.");
                break;
            case 6:
                Console.WriteLine($"Current Nationality: {account.Nationality ?? "Not provided"}");
                Console.WriteLine("Enter new nationality:");
                updateSuccessful = UserLogin._userAccountService.ManageAccount(account.Id, newNationality: Console.ReadLine());
                Console.WriteLine(updateSuccessful
                    ? "Nationality updated successfully."
                    : "Failed to update nationality.");
                break;
            case 7:
                Console.WriteLine($"Current Phone Number: {account.PhoneNumber ?? "Not provided"}");
                Console.WriteLine("Enter new phone number:");
                updateSuccessful = UserLogin._userAccountService.ManageAccount(account.Id, newPhoneNumber: Console.ReadLine());
                Console.WriteLine(updateSuccessful
                    ? "Phone number updated successfully."
                    : "Failed to update phone number.");
                break;
            case 8:
                Console.WriteLine($"Current Passport Number: {account.PassportDetails?.PassportNumber ?? "Not provided"}");
                Console.WriteLine("Enter new passport number:");
                string passportNumber = Console.ReadLine() ?? string.Empty;

                Console.WriteLine($"Current Issue Date: {account.PassportDetails?.IssueDate?.ToString("dd-MM-yyyy") ?? "Not provided"}");
                Console.WriteLine("Enter new passport issue date (dd-MM-yyyy):");
                DateTime.TryParse(Console.ReadLine(), out DateTime issueDate);

                Console.WriteLine($"Current Expiration Date: {account.PassportDetails?.ExpirationDate?.ToString("dd-MM-yyyy") ?? "Not provided"}");
                Console.WriteLine("Enter new passport expiration date (dd-MM-yyyy):");
                DateTime.TryParse(Console.ReadLine(), out DateTime expirationDate);

                Console.WriteLine($"Current Country of Issue: {account.PassportDetails?.CountryOfIssue ?? "Not provided"}");
                Console.WriteLine("Enter new country of issue:");
                string countryOfIssue = Console.ReadLine() ?? string.Empty;

                var newPassportDetails = new PassportDetailsModel(passportNumber, issueDate, expirationDate, countryOfIssue);
                updateSuccessful = UserLogin._userAccountService.ManageAccount(account.Id, newPassportDetails: newPassportDetails);
                Console.WriteLine(updateSuccessful
                    ? "Passport details updated successfully."
                    : "Failed to update passport details.");
                break;
            default:
                Console.WriteLine("Invalid option selected.");
                break;
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) && 
               email.Contains("@") && 
               email.IndexOf("@") < email.LastIndexOf(".") && 
               email.IndexOf(".") > email.IndexOf("@") + 1;
    }
}
