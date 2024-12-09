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
            "Frequent Flyer Program",
            "Payment Details",
            "View Account Details",
            "Back to Main Menu"
        };

        while (true)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(options, "Manage Account");

            if (selectedIndex == 12) return;
            if (selectedIndex == 11) DisplayAccountDetails(account);
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

        bool accountCreated = UserLogin.UserAccountServiceLogic.CreateAccount(firstName, lastName, email, password, dateOfBirth);

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

        var accounts = AccountsAccess.LoadAll();

        switch (optionIndex)
        {
            case 0:
                Console.WriteLine($"Current Email: {account.EmailAddress}");
                Console.WriteLine("Enter new email:");
                updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newEmail: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Email updated successfully." : "Failed to update email.");
                break;
            case 1:
                Console.WriteLine("Enter new password:");
                updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newPassword: Console.ReadLine());
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
                updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newFirstName: newFirstName);
                Console.WriteLine(
                    updateSuccessful ? "First name updated successfully." : "Failed to update first name.");
                break;
            case 3:
                Console.WriteLine($"Current Last Name: {account.LastName}");
                Console.WriteLine("Enter new last name:");
                updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newLastName: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Last name updated successfully." : "Failed to update last name.");
                break;
            case 4:
                Console.WriteLine($"Current Date of Birth: {account.DateOfBirth:dd-MM-yyyy}");
                Console.WriteLine("Enter new date of birth (dd-MM-yyyy):");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime newDateOfBirth))
                {
                    updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newDateOfBirth: newDateOfBirth);
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
                updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newGender: Console.ReadLine());
                Console.WriteLine(updateSuccessful ? "Gender updated successfully." : "Failed to update gender.");
                break;
            case 6:
                Console.WriteLine($"Current Nationality: {account.Nationality ?? "Not provided"}");
                Console.WriteLine("Enter new nationality:");
                updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newNationality: Console.ReadLine());
                Console.WriteLine(updateSuccessful
                    ? "Nationality updated successfully."
                    : "Failed to update nationality.");
                break;
            case 7:
                Console.WriteLine($"Current Phone Number: {account.PhoneNumber ?? "Not provided"}");
                Console.WriteLine("Enter new phone number:");
                updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newPhoneNumber: Console.ReadLine());
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
                updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newPassportDetails: newPassportDetails);
                Console.WriteLine(updateSuccessful
                    ? "Passport details updated successfully."
                    : "Failed to update passport details.");
                break;

            case 9: // Frequent Flyer Program enrollment/unenrollment

                account = accounts.FirstOrDefault(a => a.Id == account.Id);


                if (account.Miles != null && account.Miles.Count > 0)
                {
                    var milesRecord = account.Miles[0];

                    if (milesRecord.Enrolled)
                    {
                        Console.WriteLine("\n--- Frequent Flyer Program Details ---");
                        Console.WriteLine($"Current Level: {milesRecord.Level}");
                        Console.WriteLine($"Current XP: {milesRecord.Experience}");
                        Console.WriteLine($"Total Points: {milesRecord.Points}\n");
                    }

                    // Prompt for enrollment or unenrollment
                    Console.WriteLine(milesRecord.Enrolled
                        ? "You are currently enrolled in the Frequent Flyer Program."
                        : "You are not currently enrolled in the Frequent Flyer Program.");

                    Console.WriteLine(milesRecord.Enrolled
                        ? "Would you like to unenroll? (Y/N)"
                        : "Would you like to enroll? (Y/N)");

                    string response = Console.ReadLine()?.Trim().ToUpper();

                    if (response == "Y")
                    {
                        // Toggle enrollment status
                        milesRecord.Enrolled = !milesRecord.Enrolled;

                        updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(
                            account.Id,
                            newMiles: account.Miles
                        );

                        Console.WriteLine(updateSuccessful
                            ? (milesRecord.Enrolled
                                ? "Successfully enrolled in the Frequent Flyer Program!"
                                : "Successfully unenrolled from the Frequent Flyer Program.")
                            : "Failed to update Frequent Flyer Program status.");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Miles information is not available.");
                }
                break;
            case 10:
                var accountToUpdate = accounts.Find(a => a.Id == account.Id);

                Console.WriteLine("\n--- Payment Information Management ---");

                if (accountToUpdate.PaymentInformation == null) accountToUpdate.PaymentInformation = new List<PaymentInformationModel>();

                if (accountToUpdate.PaymentInformation.Count > 0)
                {
                    var currentPayment = accountToUpdate.PaymentInformation[0];
                    Console.WriteLine("Current Payment Methods:");
                    Console.WriteLine($"Card Holder: {currentPayment.CardHolder}");
                    Console.WriteLine($"Card Number: {currentPayment.CardNumber}");
                    Console.WriteLine($"Expiration Date: {currentPayment.ExpirationDate}");
                }
                else
                {
                    Console.WriteLine("No Payment methods currently saved.");
                }

                string[] paymentOptions = {
                    "Update Payment Method",
                    "Remove Payment Method",
                    "Back to Account Management",
                };

                int paymentOptionIndex = MenuNavigationService.NavigateMenu(paymentOptions, "Payment Details");

                if (paymentOptionIndex == 0)
                {
                    bool isValidPaymentInformation = false;
                    PaymentInformationModel paymentInfo = null;

                    while (!isValidPaymentInformation)
                    {
                        Console.WriteLine("Enter Card Holder Name:");
                        string _cardHolder = Console.ReadLine();

                        Console.WriteLine("Enter Card Number:");
                        string _cardNumber = Console.ReadLine();

                        Console.WriteLine("Enter CVV");
                        string _cVV = Console.ReadLine();

                        Console.WriteLine("Enter Expiration Date");
                        string _expirationDate = Console.ReadLine();

                        if (!PaymentLogic.ValidateCardNumber(_cardNumber))
                        {
                            Console.WriteLine("Invalid Card number. Must be 16 characters long and only contain digits.");
                            continue;
                        }

                        if (!PaymentLogic.ValidateCVV(_cVV))
                        {
                            Console.WriteLine("Invalid CVV. Must be 3 or 4 digits.");
                            continue;
                        }

                        if (!PaymentLogic.ValidateExpirationDate(_expirationDate))
                        {
                            Console.WriteLine("Invalid expiration date. Must be in MM/YY format and not expired");
                            continue;
                        }

                        paymentInfo = new PaymentInformationModel(_cardHolder, _cardNumber, _cVV, _expirationDate);
                        isValidPaymentInformation = true;
                    }

                    accountToUpdate.PaymentInformation.Clear();
                    accountToUpdate.PaymentInformation.Add(paymentInfo);

                    AccountsAccess.WriteAll(accounts);

                    Console.WriteLine("Payment method updated successfully.");
                }

                if (paymentOptionIndex == 1)
                {
                    accountToUpdate.PaymentInformation.Clear();
                    AccountsAccess.WriteAll(accounts);

                    Console.WriteLine("Payment method removed successfully.");
                }

                if (paymentOptionIndex == 2)
                {
                    return;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
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
