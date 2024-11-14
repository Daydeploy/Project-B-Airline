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
    private static void CreateAccount()
    {
        Console.WriteLine("Note: You can press F2 to toggle password visibility while typing.\n");
        Console.WriteLine("Create a new account");

        Console.WriteLine("Enter your first name:");
        string firstName = Console.ReadLine();
        Console.WriteLine("Enter your last name:");
        string lastName = Console.ReadLine();

        Console.WriteLine("Enter your email address:");
        string email = Console.ReadLine();

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

        Console.WriteLine("Enter your date of birth (yyyy-mm-dd):");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime dateOfBirth))
        {
            Console.WriteLine("Invalid date format. Please try again.");
            return;
        }

        bool accountCreated = _userAccountService.CreateAccount(email, password, firstName, lastName, dateOfBirth);

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
        Console.WriteLine($"Date of Birth: {account.DateOfBirth:yyyy-MM-dd}");
        Console.WriteLine($"Gender: {account.Gender ?? "Not provided"}");
        Console.WriteLine($"Nationality: {account.Nationality ?? "Not provided"}");
        Console.WriteLine($"Phone Number: {account.PhoneNumber ?? "Not provided"}");

        if (account.PassportDetails != null)
        {
            Console.WriteLine("Passport Details:");
            Console.WriteLine($"  Passport Number: {account.PassportDetails.PassportNumber ?? "Not provided"}");
            Console.WriteLine($"  Issue Date: {account.PassportDetails.IssueDate?.ToString("yyyy-MM-dd") ?? "Not provided"}");
            Console.WriteLine($"  Expiration Date: {account.PassportDetails.ExpirationDate?.ToString("yyyy-MM-dd") ?? "Not provided"}");
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
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newEmail: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Email updated successfully." : "Failed to update email.");
                break;
            case 1:
                Console.WriteLine("Enter new password:");
                updateSuccessful = _userAccountService.ManageAccount(account.Id, newPassword: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Password updated successfully." : "Failed to update password.");
                break;
            // Additional cases would handle the remaining account properties
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    // Modifies passenger details within a specific booking
    public static void ModifyPassengerDetails(int flightId, int passengerId)
    {
        Console.WriteLine("Enter new seat number:");
        string seatNumber = Console.ReadLine() ?? string.Empty;

        Console.WriteLine("Do you have checked baggage? (y/n):");
        bool hasCheckedBaggage = Console.ReadLine()?.ToLower() == "y";

        var newDetails = new BookingDetails
        {
            SeatNumber = seatNumber,
            HasCheckedBaggage = hasCheckedBaggage
        };

        bool success = _userAccountService.ModifyBooking(flightId, passengerId, newDetails);
        Console.WriteLine(success ? "Booking modified successfully." : "Failed to modify booking. Please try again or contact support.");
    }
}
