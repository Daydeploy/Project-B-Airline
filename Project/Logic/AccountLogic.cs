using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class AccountsLogic
{
    public List<AccountModel> _accounts;

    static public AccountModel? CurrentAccount { get; private set; }

    public AccountsLogic()
    {
        _accounts = AccountsAccess.LoadAll();
    }

    public void UpdateList(AccountModel acc)
    {
        int index = _accounts.FindIndex(s => s.Id == acc.Id);

        if (index != -1)
        {
            _accounts[index] = acc;
        }
        else
        {
            _accounts.Add(acc);
        }

        AccountsAccess.WriteAll(_accounts);

        _accounts = AccountsAccess.LoadAll();
    }

    public AccountModel GetById(int id)
    {
        return _accounts.Find(i => i.Id == id);
    }

    public AccountModel CheckLogin(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return null;
        }

        CurrentAccount = _accounts.Find(i =>
            i.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase) && i.Password == password);
        return CurrentAccount;
    }

    public static bool HasCompleteContactInformation(string FirstName, string LastName, string Email, string PhoneNumber, string Address)
    {
        if (FirstName == null || LastName == null || Email == null || PhoneNumber == null || Address == null)
        {
            return false;
        }
        return true;
    }

    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return false;
        }

        bool hasUpperCase = Regex.IsMatch(password, @"[A-Z]");
        bool hasNumber = Regex.IsMatch(password, @"[0-9]");
        bool hasSpecialChar = Regex.IsMatch(password, @"[!@#$%^&*(),.?""':;{}|<>]");

        return hasUpperCase && hasNumber && hasSpecialChar;
    }

    public static bool IsValidName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        name = name.Trim();

        if (name.Length < 2 || name.Length > 20)
        {
            return false;
        }

        if (name.Any(char.IsDigit))
        {
            return false;
        }

        if (name.Any(char.IsPunctuation))
        {
            return false;
        }

        name = char.ToUpper(name[0]) + name.Substring(1).ToLower();


        return true;
    }

    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        if (!email.Contains("@") || !email.Contains("."))
        {
            return false;
        }
        return true;
    }

    public bool DeleteAccount(int accountId)
    {
        var accountToDelete = _accounts.FirstOrDefault(a => a.Id == accountId);
        if (accountToDelete == null || accountToDelete.EmailAddress.ToLower() == "admin")
        {
            return false;
        }

        _accounts.Remove(accountToDelete);
        AccountsAccess.WriteAll(_accounts);
        // Reload accounts after deletion
        _accounts = AccountsAccess.LoadAll();
        return true;
    }

    public List<AccountModel> GetAllAccounts()
    {
        return _accounts
            .Where(a => !a.EmailAddress.ToLower().Equals("admin"))
            .ToList();
    }


    public static bool IsValidDateOfBirth(DateTime dateOfBirth)
    {
        int minAge = 0;
        int maxAge = 150;
        var now = DateTime.Now;
        var age = now.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > now.AddYears(-age))
            age--;

        return age >= minAge && age <= maxAge;
    }

    public static bool IsValidGender(string gender)
    {
        if (string.IsNullOrWhiteSpace(gender))
            return false;

        gender = gender.Trim().ToLower();
        return gender == "male" || gender == "female";
    }

    public static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber)) { return false; }

        phoneNumber = phoneNumber.Trim();

        if (phoneNumber.StartsWith("+")) { phoneNumber = phoneNumber.Substring(1); }

        if (!phoneNumber.All(char.IsDigit)) { return false; }

        return phoneNumber.Length >= 10 && phoneNumber.Length <= 15;
    }

    public static bool IsValidAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return false;
        }
        return true;
    }

    public static bool IsValidPassportDetails(PassportDetailsModel passport)
    {
        if (passport == null)
            return true;

        if (string.IsNullOrWhiteSpace(passport.PassportNumber) ||
            string.IsNullOrWhiteSpace(passport.CountryOfIssue))
            return false;

        if (!passport.IssueDate.HasValue || !passport.ExpirationDate.HasValue)
            return false;


        var now = DateTime.Now.Date;
        return passport.ExpirationDate.Value > now &&
               passport.IssueDate.Value <= now &&
               passport.ExpirationDate.Value > passport.IssueDate.Value;
    }

    public bool ValidateAccountCreation(
        string firstName, string lastName, string email,
        string password, DateTime dateOfBirth, string gender = null,
        string nationality = null, string phoneNumber = null,
        string address = null, PassportDetailsModel passportDetails = null)
    {
        if (string.IsNullOrWhiteSpace(firstName) || !IsValidName(firstName))
            return false;

        if (string.IsNullOrWhiteSpace(lastName) || !IsValidName(lastName))
            return false;

        if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            return false;

        if (string.IsNullOrWhiteSpace(password) || !IsValidPassword(password))
            return false;

        if (!IsValidDateOfBirth(dateOfBirth))
            return false;

        if (gender != null && !IsValidGender(gender))
            return false;

        if (phoneNumber != null && !IsValidPhoneNumber(phoneNumber))
            return false;

        if (passportDetails != null && !IsValidPassportDetails(passportDetails))
            return false;

        var existingAccount = _accounts
            .FirstOrDefault(a => a.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));

        return existingAccount == null;
    }
}