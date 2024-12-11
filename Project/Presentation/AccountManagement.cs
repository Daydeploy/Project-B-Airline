using System;
using System.Collections.Generic;

static class AccountManagement
{
    // Manages the user's account, providing options to update various details
    public static void ManageAccount(AccountModel account)
    {
        string[] options =
        {
            "Personal Details",
            "Payment Details",
            "Frequent Flyer Program",
            "View Account Details",
            "Back to Main Menu"
        };

        while (true)
        {
            int selectedIndex = MenuNavigationService.NavigateMenu(options, "Manage Account");

            if (selectedIndex == 4) return;
            if (selectedIndex == 3) DisplayAccountDetails(account);
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
    public static void HandleManageAccountOption(int optionIndex, AccountModel account)
    {
        bool updateSuccessful = false;

        var accounts = AccountsAccess.LoadAll();

        switch (optionIndex)
        {
            case 0: // Personal Information
                Console.WriteLine("\n--- Personal Information Management ---");

                string[] personalOptions = {
                    "Update Email",
                    "Update Password",
                    "Update First Name",
                    "Update Last Name",
                    "Update Date of Birth",
                    "Update Gender",
                    "Update Nationality",
                    "Update Phone Number",
                    "Update Address",
                    "Update Passport Details",
                };

                int personalOptionIndex = MenuNavigationService.NavigateMenu(personalOptions, "Personal Details");

                switch (personalOptionIndex)
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
                        Console.WriteLine($"Current Address: {account.Address ?? "Not provided"}");
                        Console.WriteLine("Enter new Address:");
                        updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newAddress: Console.ReadLine());

                        Console.WriteLine(updateSuccessful ? "Address updated successfully." : "Failed to update address.");
                        break;

                    case 9:
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

                    default:
                        Console.WriteLine("Invalid option selected.");
                        break;
                }
                break;

            case 1: // Payment Information
                var accountToUpdate = accounts.Find(a => a.Id == account.Id);

                Console.WriteLine("\n--- Payment Information Management ---");

                if (accountToUpdate.PaymentInformation == null) accountToUpdate.PaymentInformation = new List<PaymentInformationModel>();

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
                        string _cardHolder;
                        while (true)
                        {
                            Console.WriteLine("\nEnter Card Holder Name:");
                            _cardHolder = Console.ReadLine();

                            if (!PaymentLogic.ValidateName(_cardHolder))
                            {
                                Console.WriteLine("Card Holder Name cannot be empty, Please try again.");
                                continue;
                            }
                            break;
                        }

                        string _cardNumber;
                        while (true)
                        {
                            Console.WriteLine("\nEnter Card Number:");
                            _cardNumber = Console.ReadLine();

                            if (!PaymentLogic.ValidateCardNumber(_cardNumber))
                            {
                                Console.WriteLine("Invalid Card number, Must be 16 characters long and only contain digits.");
                                continue;
                            }
                            break;
                        }

                        string _cVV;
                        while (true)
                        {
                            Console.WriteLine("\nEnter CVV");
                            _cVV = Console.ReadLine();

                            if (!PaymentLogic.ValidateCVV(_cVV))
                            {
                                Console.WriteLine("Invalid CVV, Must be 3 or 4 digits.");
                                continue;
                            }
                            break;
                        }

                        string _expirationDate;
                        while (true)
                        {
                            Console.WriteLine("\nEnter Expiration Date");
                            _expirationDate = Console.ReadLine();

                            if (!PaymentLogic.ValidateExpirationDate(_expirationDate))
                            {
                                Console.WriteLine("Invalid expiration date, Must be in MM/YY format and not expired.");
                                continue;
                            }
                            break;
                        }

                        string _billingAddress;
                        while (true)
                        {
                            Console.WriteLine("\nEnter Billing Address");
                            _billingAddress = Console.ReadLine();

                            if (!PaymentLogic.ValidateAddress(_billingAddress))
                            {
                                Console.WriteLine("Invalid billing address, Cannot be empty.");
                                continue;
                            };
                            break;
                        }

                        Console.WriteLine("\nConfirm Payment Method Update:");
                        Console.WriteLine($"Card Holder: {_cardHolder}");
                        Console.WriteLine($"Card Number: {_cardNumber}");
                        Console.WriteLine($"Expiration Date: {_expirationDate}");
                        Console.WriteLine($"Billing Address: {_billingAddress}");

                        Console.WriteLine("\nPress any key to confirm this payment method, or 'N' to cancel.");
                        var confirmKey = Console.ReadKey(true);

                        if (confirmKey.Key == ConsoleKey.N)
                        {
                            Console.WriteLine("Payment method update cancelled.");
                            break;
                        }

                        paymentInfo = new PaymentInformationModel(_cardHolder, _cardNumber, _cVV, _expirationDate, _billingAddress);
                        isValidPaymentInformation = true;
                    }

                    if (paymentInfo != null)
                    {
                        accountToUpdate.PaymentInformation.Clear();
                        accountToUpdate.PaymentInformation.Add(paymentInfo);

                        AccountsAccess.WriteAll(accounts);

                        Console.WriteLine("Payment method updated successfully.");
                    }
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
                Console.Clear();
                break;

            case 2: // Frequent Flyer Program enrollment/unenrollment

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
