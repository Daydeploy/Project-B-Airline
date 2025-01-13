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
        Console.WriteLine("Note: You can press F2 to toggle password visibility while typing.");
        Console.WriteLine("Press ESC at any time to return to menu\n");
        Console.WriteLine("Create a new account\n");

        bool showPassword = false; // Moved to top level

        // First Name
        string firstName = GetUserInput("Enter your first name: ", isPassword: false, ref showPassword);
        if (firstName == null) return;
        while (!AccountsLogic.IsValidName(firstName))
        {
            Console.WriteLine(
                "First name must be between 2 and 20 characters long, start with a capital letter, and cannot contain numbers.");
            firstName = GetUserInput("Enter your first name: ", isPassword: false, ref showPassword);
            if (firstName == null) return;
        }

        // Last Name
        string lastName = GetUserInput("Enter your last name: ", isPassword: false, ref showPassword);
        if (lastName == null) return;
        while (!AccountsLogic.IsValidName(lastName))
        {
            Console.WriteLine(
                "Last name must be between 2 and 20 characters long, start with a capital letter, and cannot contain numbers.");
            lastName = GetUserInput("Enter your last name: ", isPassword: false, ref showPassword);
            if (lastName == null) return;
        }

        // Email
        string email = GetUserInput("Enter your email address: ", isPassword: false, ref showPassword);
        if (email == null) return;
        while (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
        {
            Console.WriteLine("Error: Email must contain '@' and a domain (For instance: '.com').");
            email = GetUserInput("Please enter your email address again: ", isPassword: false, ref showPassword);
            if (email == null) return;
        }

        // Password
        string password = GetUserInput("Enter your password: ", isPassword: true, ref showPassword);
        if (password == null) return;
        while (!AccountsLogic.IsValidPassword(password))
        {
            Console.WriteLine(
                "Password must contain at least one uppercase letter, one number, and one special character.");
            password = GetUserInput("Enter your password: ", isPassword: true, ref showPassword);
            if (password == null) return;
        }

        string confirmPassword = GetUserInput("Confirm your password: ", isPassword: true, ref showPassword);
        if (confirmPassword == null) return;
        while (!AccountsLogic.IsValidPassword(confirmPassword))
        {
            Console.WriteLine(
                "Password must contain at least one uppercase letter, one number, and one special character.");
            confirmPassword = GetUserInput("Confirm your password: ", isPassword: true, ref showPassword);
            if (confirmPassword == null) return;
        }

        if (password != confirmPassword)
        {
            Console.WriteLine("\nPasswords do not match. Please try again.");
            return;
        }

        // Date of Birth
        string dobInput = GetUserInput("Enter your date of birth (dd-MM-yyyy): ", isPassword: false, ref showPassword);
        if (dobInput == null) return;

        if (!DateTime.TryParse(dobInput, out DateTime dateOfBirth))
        {
            Console.WriteLine("Invalid date format. Please try again.");
            return;
        }

        System.Console.WriteLine("Would you like to add more information? (Y/N) \nOtherwise you can change your information in the manage account menu.");
        string response = Console.ReadLine()?.Trim().ToUpper();

        string gender = null, nationality = null, phoneNumber = null, address = null;
        PassportDetailsModel passportDetails = null;

        if (response == "Y")
        {
            while (true)
            {
                Console.WriteLine("\nEnter gender: (Male/Female)");
                gender = Console.ReadLine()?.Trim();
                if (!AccountsLogic.IsValidGender(gender))
                {
                    Console.WriteLine("Invalid gender. Please enter either 'Male' or 'Female'.");
                    continue;
                }
                break;
            }

            while (true)
            {
                Console.WriteLine("Enter nationality:");
                nationality = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(nationality))
                {
                    Console.WriteLine("Nationality cannot be empty. Please enter a valid nationality.");
                    continue;
                }
                break;
            }

            while (true)
            {
                Console.WriteLine("Enter phone number:");
                phoneNumber = Console.ReadLine()?.Trim();
                if (!AccountsLogic.IsValidPhoneNumber(phoneNumber))
                {
                    Console.WriteLine("Invalid phone number. Number must be between 10-15 digits.");
                    continue;
                }
                break;
            }

            while (true)
            {
                Console.WriteLine("Enter address:");
                address = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(address))
                {
                    Console.WriteLine("Address cannot be empty. Please enter a valid address.");
                    continue;
                }
                break;
            }
            Console.WriteLine("Would you like to add passport details? (Y/N)");
            if (Console.ReadLine()?.Trim().ToUpper() == "Y")
            {
                string passportNumber, countryOfIssue;
                DateTime issueDate, expirationDate;

                while (true)
                {
                    Console.WriteLine("Enter passport number:");
                    passportNumber = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(passportNumber))
                    {
                        Console.WriteLine("Passport number cannot be empty. Please enter a valid passport number.");
                        continue;
                    }
                    break;
                }

                while (true)
                {
                    Console.WriteLine("Enter passport issue date (dd-MM-yyyy):");
                    if (!DateTime.TryParse(Console.ReadLine(), out issueDate))
                    {
                        Console.WriteLine("Invalid date format. Please enter the date in dd-MM-yyyy format.");
                        continue;
                    }
                    break;
                }

                while (true)
                {
                    Console.WriteLine("Enter passport expiration date (dd-MM-yyyy):");
                    if (!DateTime.TryParse(Console.ReadLine(), out expirationDate))
                    {
                        Console.WriteLine("Invalid date format. Please enter the date in dd-MM-yyyy format.");
                        continue;
                    }
                    if (expirationDate <= issueDate)
                    {
                        Console.WriteLine("Expiration date must be after the issue date.");
                        continue;
                    }
                    break;
                }

                while (true)
                {
                    Console.WriteLine("Enter country of issue:");
                    countryOfIssue = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(countryOfIssue))
                    {
                        Console.WriteLine("Country of issue cannot be empty. Please enter a valid country.");
                        continue;
                    }
                    break;
                }

                passportDetails = new PassportDetailsModel(passportNumber, issueDate, expirationDate, countryOfIssue);
            }
        }

        bool accountCreated = UserLogin.UserAccountServiceLogic.CreateAccount(firstName, lastName, email, password, dateOfBirth,
        gender, nationality, phoneNumber, address, passportDetails);

        Console.WriteLine(accountCreated
            ? "\nAccount created successfully. Please login."
            : "\nFailed to create account. Email may already be in use.");
    }

    private static string GetUserInput(string prompt, bool isPassword)
    {
        bool showPassword = false;
        return GetUserInput(prompt, isPassword, ref showPassword);
    }

    private static string GetUserInput(string prompt, bool isPassword, ref bool showPassword)
    {
        Console.Write(prompt);
        string input = "";
        ConsoleKeyInfo key;

        while (true)
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("\nReturning to menu...");
                return null;
            }

            if (key.Key == ConsoleKey.Enter && input.Length > 0)
            {
                Console.WriteLine();
                return input;
            }

            if (isPassword && key.Key == ConsoleKey.F2)
            {
                showPassword = !showPassword;
                Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                Console.Write(prompt + (showPassword ? input : new string('*', input.Length)));
                continue;
            }

            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
                Console.Write(prompt);
                if (isPassword)
                    Console.Write(showPassword ? input : new string('*', input.Length));
                else
                    Console.Write(input);
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
                if (isPassword)
                    Console.Write(showPassword ? key.KeyChar : '*');
                else
                    Console.Write(key.KeyChar);
            }
        }
    }

    // Displays the account details for the user
    private static void DisplayAccountDetails(AccountModel account)
    {
        AccountsAccess.LoadAll();

        Console.WriteLine("\n--- Account Details ---");
        Console.WriteLine($"Email: {account.EmailAddress}");
        Console.WriteLine($"First Name: {account.FirstName}");
        Console.WriteLine($"Last Name: {account.LastName}");
        Console.WriteLine($"Date of Birth: {account.DateOfBirth:dd-MM-yyyy}");
        Console.WriteLine($"Gender: {account.Gender ?? "Not provided"}");
        Console.WriteLine($"Nationality: {account.Nationality ?? "Not provided"}");
        Console.WriteLine($"Phone Number: {account.PhoneNumber ?? "Not provided"}");
        Console.WriteLine($"Address: {account.Address ?? "Not provided"}");

        if (account.PassportDetails != null)
        {
            Console.WriteLine("Passport Details:");
            Console.WriteLine($"  Passport Number: {account.PassportDetails.PassportNumber ?? "Not provided"}");
            Console.WriteLine(
                $"  Issue Date: {account.PassportDetails.IssueDate?.ToString("dd-MM-yyyy") ?? "Not provided"}");
            Console.WriteLine(
                $"  Expiration Date: {account.PassportDetails.ExpirationDate?.ToString("dd-MM-yyyy") ?? "Not provided"}");
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

        var accountToUpdate = accounts.Find(a => a.Id == account.Id);

        switch (optionIndex)
        {
            case 0: // Personal Information
                accountToUpdate = accounts.Find(a => a.Id == account.Id);
                Console.WriteLine("\n--- Personal Information Management ---");

                string[] personalOptions =
                {
                    $"Update Email             Current: {account.EmailAddress}",
                    $"Update Password          Current: {"*".PadRight(account.Password.Length, '*')}",
                    $"Update First Name        Current: {account.FirstName}",
                    $"Update Last Name         Current: {account.LastName}",
                    $"Update Date of Birth     Current: {account.DateOfBirth:dd-MM-yyyy}",
                    $"Update Gender            Current: {account.Gender ?? "Not provided"}",
                    $"Update Nationality       Current: {account.Nationality ?? "Not provided"}",
                    $"Update Phone Number      Current: {account.PhoneNumber ?? "Not provided"}",
                    $"Update Address           Current: {account.Address ?? "Not provided"}",
                    $"Update Passport Details",
                    "Return to Account Management"
                };

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n=== Personal Information Management ===");
                Console.ResetColor();

                int personalOptionIndex = MenuNavigationService.NavigateMenu(personalOptions, "Personal Details");

                switch (personalOptionIndex)
                {
                    case 0: // Email
                        Console.WriteLine($"Current Email: {account.EmailAddress}");
                        Console.Write("Enter new email: ");
                        string newEmail;
                        do
                        {
                            newEmail = GetInputWithEsc();
                            if (newEmail == null) return;
                            if (!IsValidEmail(newEmail))
                            {
                                Console.WriteLine("Invalid email format. Please enter a valid email address.");
                                Console.Write("Enter new email: ");
                            }
                        } while (newEmail != null && !IsValidEmail(newEmail));

                        if (newEmail != null)
                        {
                            updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newEmail);
                            Console.WriteLine(updateSuccessful ? "Email updated successfully." : "Failed to update email.");
                        }
                        break;

                    case 1: // Password
                        Console.Write("Enter new password: ");
                        string newPassword;
                        do
                        {
                            newPassword = GetInputWithEsc();
                            if (newPassword == null) return;
                            if (!AccountsLogic.IsValidPassword(newPassword))
                            {
                                Console.WriteLine("Password must contain at least one uppercase letter, one number, and one special character.");
                                Console.Write("Enter new password: ");
                            }
                        } while (newPassword != null && !AccountsLogic.IsValidPassword(newPassword));

                        if (newPassword != null)
                        {
                            updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newPassword);
                            Console.WriteLine(updateSuccessful ? "Password updated successfully." : "Failed to update password.");
                        }
                        break;

                    case 2: // First Name
                        Console.WriteLine($"Current First Name: {account.FirstName}");
                        Console.Write("Enter new first name: ");
                        string newFirstName = GetInputWithEsc();
                        if (newFirstName == null) return;

                        while (string.IsNullOrWhiteSpace(newFirstName))
                        {
                            Console.WriteLine("Invalid input. Please enter a valid first name.");
                            Console.Write("Enter new first name: ");
                            newFirstName = GetInputWithEsc();
                            if (newFirstName == null) return;
                        }

                        updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newFirstName: newFirstName);
                        Console.WriteLine(updateSuccessful ? "First name updated successfully." : "Failed to update first name.");
                        break;

                    case 3: // Last Name
                        Console.WriteLine($"Current Last Name: {account.LastName}");
                        Console.Write("Enter new last name: ");
                        string newLastName = GetInputWithEsc();
                        if (newLastName == null) return;

                        updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newLastName: newLastName);
                        Console.WriteLine(updateSuccessful ? "Last name updated successfully." : "Failed to update last name.");
                        break;

                    case 4: // Date of Birth
                        Console.WriteLine($"Current Date of Birth: {account.DateOfBirth:dd-MM-yyyy}");
                        Console.Write("Enter new date of birth (dd-MM-yyyy): ");
                        string dateStr = GetInputWithEsc();
                        if (dateStr == null) return;

                        if (DateTime.TryParse(dateStr, out DateTime newDateOfBirth))
                        {
                            updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newDateOfBirth: newDateOfBirth);
                            Console.WriteLine(updateSuccessful ? "Date of birth updated successfully." : "Failed to update date of birth.");
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format. Date of birth not updated.");
                        }
                        break;

                    case 5: // Gender
                        Console.WriteLine($"Current Gender: {account.Gender ?? "Not provided"}");
                        Console.Write("Enter new gender: ");
                        string newGender = GetInputWithEsc();
                        if (newGender == null) return;

                        updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newGender: newGender);
                        Console.WriteLine(updateSuccessful ? "Gender updated successfully." : "Failed to update gender.");
                        break;

                    case 6: // Nationality
                        Console.WriteLine($"Current Nationality: {account.Nationality ?? "Not provided"}");
                        Console.Write("Enter new nationality: ");
                        string newNationality = GetInputWithEsc();
                        if (newNationality == null) return;

                        updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newNationality: newNationality);
                        Console.WriteLine(updateSuccessful ? "Nationality updated successfully." : "Failed to update nationality.");
                        break;

                    case 7: // Phone Number
                        Console.WriteLine($"Current Phone Number: {account.PhoneNumber ?? "Not provided"}");
                        Console.Write("Enter new phone number: ");
                        string newPhone = GetInputWithEsc();
                        if (newPhone == null) return;

                        updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newPhoneNumber: newPhone);
                        Console.WriteLine(updateSuccessful ? "Phone number updated successfully." : "Failed to update phone number.");
                        break;

                    case 8: // Address
                        Console.WriteLine($"Current Address: {account.Address ?? "Not provided"}");
                        Console.Write("Enter new Address: ");
                        string newAddress = GetInputWithEsc();
                        if (newAddress == null) return;

                        updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newAddress: newAddress);
                        Console.WriteLine(updateSuccessful ? "Address updated successfully." : "Failed to update address.");
                        break;

                    case 9: // Passport Details
                        Console.WriteLine($"Current Passport Number: {account.PassportDetails?.PassportNumber ?? "Not provided"}");
                        Console.Write("Enter new passport number: ");
                        string passportNumber = GetInputWithEsc();
                        if (passportNumber == null) return;

                        Console.WriteLine($"Current Issue Date: {account.PassportDetails?.IssueDate?.ToString("dd-MM-yyyy") ?? "Not provided"}");
                        Console.Write("Enter new passport issue date (dd-MM-yyyy): ");
                        string issueDateStr = GetInputWithEsc();
                        if (issueDateStr == null) return;
                        DateTime.TryParse(issueDateStr, out DateTime issueDate);

                        Console.WriteLine($"Current Expiration Date: {account.PassportDetails?.ExpirationDate?.ToString("dd-MM-yyyy") ?? "Not provided"}");
                        Console.Write("Enter new passport expiration date (dd-MM-yyyy): ");
                        string expDateStr = GetInputWithEsc();
                        if (expDateStr == null) return;
                        DateTime.TryParse(expDateStr, out DateTime expirationDate);

                        Console.WriteLine($"Current Country of Issue: {account.PassportDetails?.CountryOfIssue ?? "Not provided"}");
                        Console.Write("Enter new country of issue: ");
                        string countryOfIssue = GetInputWithEsc();
                        if (countryOfIssue == null) return;

                        var newPassportDetails = new PassportDetailsModel(passportNumber, issueDate, expirationDate, countryOfIssue);
                        updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newPassportDetails: newPassportDetails);
                        Console.WriteLine(updateSuccessful ? "Passport details updated successfully." : "Failed to update passport details.");
                        break;

                    case 10: // Return to Account Management
                        break;

                    default:
                        Console.WriteLine("Invalid option selected.");
                        break;
                }
                break;

            case 1: // Payment Information
                Console.WriteLine("\n--- Payment Information Management ---");

                accountToUpdate = accounts.Find(a => a.Id == account.Id);

                if (accountToUpdate.PaymentInformation == null)
                    accountToUpdate.PaymentInformation = new List<PaymentInformationModel>();

                string[] paymentOptions =
                {
                "Update Payment Method",
                "Remove Payment Method",
                "Back to Account Management",
            };

                int paymentOptionIndex = MenuNavigationService.NavigateMenu(paymentOptions, "Payment Details");

                if (paymentOptionIndex == 0)
                {
                    PaymentInformationModel paymentInfo = null;

                    // Card Holder Name
                    Console.Write("\nEnter Card Holder Name: ");
                    string cardHolder = GetInputWithEsc();
                    if (cardHolder == null) return;

                    while (!PaymentLogic.ValidateName(cardHolder))
                    {
                        Console.WriteLine("Card Holder Name cannot be empty, Please try again.");
                        Console.Write("Enter Card Holder Name: ");
                        cardHolder = GetInputWithEsc();
                        if (cardHolder == null) return;
                    }

                    // Card Number
                    Console.Write("\nEnter Card Number (Must be 16 digits): ");
                    string cardNumber = GetInputWithEsc();
                    if (cardNumber == null) return;

                    while (!PaymentLogic.ValidateCardNumber(cardNumber))
                    {
                        Console.WriteLine("Invalid Card number, Must be 16 characters long and only contain digits.");
                        Console.Write("Enter Card Number: ");
                        cardNumber = GetInputWithEsc();
                        if (cardNumber == null) return;
                    }

                    // CVV
                    Console.Write("\nEnter CVV (Must be 3 or 4 digits): ");
                    string cvv = GetInputWithEsc();
                    if (cvv == null) return;

                    while (!PaymentLogic.ValidateCVV(cvv))
                    {
                        Console.WriteLine("Invalid CVV, Must be 3 or 4 digits.");
                        Console.Write("Enter CVV: ");
                        cvv = GetInputWithEsc();
                        if (cvv == null) return;
                    }

                    // Expiration Date
                    Console.Write("\nEnter Expiration Date (MM/YY): ");
                    string expirationDate = GetInputWithEsc();
                    if (expirationDate == null) return;

                    while (!PaymentLogic.ValidateExpirationDate(expirationDate))
                    {
                        Console.WriteLine("Invalid expiration date, Must be in MM/YY format and not expired.");
                        Console.Write("Enter Expiration Date: ");
                        expirationDate = GetInputWithEsc();
                        if (expirationDate == null) return;
                    }

                    // Billing Address
                    Console.Write("\nEnter Billing Address: ");
                    string billingAddress = GetInputWithEsc();
                    if (billingAddress == null) return;

                    while (!PaymentLogic.ValidateAddress(billingAddress))
                    {
                        Console.WriteLine("Invalid billing address, Cannot be empty.");
                        Console.Write("Enter Billing Address: ");
                        billingAddress = GetInputWithEsc();
                        if (billingAddress == null) return;
                    }

                    Console.WriteLine("\nConfirm Payment Method Update:");
                    Console.WriteLine($"Card Holder: {cardHolder}");
                    Console.WriteLine($"Card Number: {cardNumber}");
                    Console.WriteLine($"CVV: {cvv}");
                    Console.WriteLine($"Expiration Date: {expirationDate}");
                    Console.WriteLine($"Billing Address: {billingAddress}");

                    Console.WriteLine("\nPress any key to confirm this payment method, or 'N' to cancel.");
                    var confirmKey = Console.ReadKey(true);

                    if (confirmKey.Key != ConsoleKey.N)
                    {
                        paymentInfo = new PaymentInformationModel(cardHolder, cardNumber, cvv, expirationDate, billingAddress);
                        var newPaymentInfo = new List<PaymentInformationModel> { paymentInfo };

                        updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(accountToUpdate.Id, newPaymentInformation: newPaymentInfo);

                        Console.WriteLine(updateSuccessful
                            ? "Payment method updated successfully."
                            : "Failed to update payment method.");
                    }
                    else
                    {
                        Console.WriteLine("Payment method update cancelled.");
                    }
                }

                if (paymentOptionIndex == 1)
                {
                    Console.Write("\nAre you sure you want to remove the payment method? (Y/N): ");
                    string response = GetInputWithEsc();
                    if (response == null) return;

                    if (response.Trim().ToUpper() == "Y")
                    {
                        updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(account.Id, newPaymentInformation: new List<PaymentInformationModel>());

                        Console.WriteLine(updateSuccessful
                            ? "Payment method removed successfully."
                            : "Failed to remove payment method.");
                    }
                    else
                    {
                        Console.WriteLine("Payment method removal cancelled.");
                    }
                }
                break;

            case 2: // Frequent Flyer Program
                account = accounts.FirstOrDefault(a => a.Id == account.Id);

                if (account.Miles == null || account.Miles.Count == 0)
                {
                    account.Miles = new List<MilesModel> { new MilesModel("Bronze", 0, 0, "Initial enrollment") };
                }

                var milesRecord = account.Miles[0];

                if (milesRecord.Enrolled)
                {
                    Console.WriteLine("\n--- Frequent Flyer Program Details ---");
                    Console.WriteLine($"Current Level: {milesRecord.Level}");
                    Console.WriteLine($"Current XP: {milesRecord.Experience}");
                    Console.WriteLine($"Total Points: {milesRecord.Points}\n");
                }

                Console.WriteLine(milesRecord.Enrolled
                        ? "You are currently enrolled in the Frequent Flyer Program."
                        : "You are not currently enrolled in the Frequent Flyer Program.");

                Console.Write(milesRecord.Enrolled
                    ? "Would you like to unenroll? (Y/N): "
                    : "Would you like to enroll? (Y/N): ");

                string ffpResponse = GetInputWithEsc();
                if (ffpResponse == null) return;

                if (ffpResponse.Trim().ToUpper() == "Y")
                {
                    // Toggle enrollment status
                    milesRecord.Enrolled = !milesRecord.Enrolled;

                    milesRecord.History += $"\n{(milesRecord.Enrolled ? "Enrolled" : "Unenrolled")} at {DateTime.Now:yyyy-MM-dd-HH:mm:ss}";


                    updateSuccessful = UserLogin.UserAccountServiceLogic.ManageAccount(
                        account.Id,
                        newMiles: account.Miles
                    );

                    if (updateSuccessful)
                    {
                        AccountsAccess.WriteAll(accounts);
                        Console.WriteLine(milesRecord.Enrolled
                            ? "Successfully enrolled in the Frequent Flyer Program!"
                            : "Successfully unenrolled from the Frequent Flyer Program.");
                    }
                    else
                    {
                        Console.WriteLine("Failed to update Frequent Flyer Program status.");
                    }
                }
                break;

            default:
                Console.WriteLine("Invalid option selected.");
                break;
        }

        if (updateSuccessful)
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }



    private static string GetInputWithEsc()
    {
        string input = "";
        while (true)
        {
            var key = Console.ReadKey(intercept: true);
            if (key.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("\nOperation cancelled. Returning to menu...");
                return null;
            }
            if (key.Key == ConsoleKey.Enter && !string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine();
                return input;
            }
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        }
    }
    private static bool IsValidEmail(string email)
    {
        return !string.IsNullOrWhiteSpace(email) &&
               email.Contains("@") &&
               email.IndexOf("@") < email.LastIndexOf(".") &&
               email.IndexOf(".") > email.IndexOf("@") + 1;
    }
}