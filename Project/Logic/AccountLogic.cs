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

    public static bool CreateAccount(string firstName, string lastName, string email, string password, string confirmPassword, DateTime dateOfBirth, bool enrollFrequentFlyer)
    {
        if (password != confirmPassword)
        {
            throw new Exception("Passwords do not match.");
        }

        if (!IsValidPassword(password))
        {
            throw new Exception("Invalid password. Password must contain at least one uppercase letter, one number, and one special character.");
        }

        return true;
    }

    public static bool IsValidFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return false;
        }

        firstName = firstName.Trim();

        if (firstName.Length < 2 || firstName.Length > 20)
        {
            return false;
        }

        if (firstName.Any(char.IsDigit))
        {
            return false;
        }

        firstName = char.ToUpper(firstName[0]) + firstName.Substring(1).ToLower();


        return true;
    }

    public static bool IsValidLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return false;
        }

        lastName = lastName.Trim();

        if (lastName.Length < 2 || lastName.Length > 20)
        {
            return false;
        }

        if (lastName.Any(char.IsDigit))
        {
            return false;
        }

        lastName = char.ToUpper(lastName[0]) + lastName.Substring(1).ToLower();


        return true;
    }
}