using System;
using System.Collections.Generic;
using System.Linq;

public class UserAccountService
{
    private AccountsLogic _accountsLogic;

    public UserAccountService()
    {
        _accountsLogic = new AccountsLogic();
    }

    public bool CreateAccount(string email, string password, string fullName)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(fullName))
        {
            return false;
        }

        var existingAccount = _accountsLogic._accounts.FirstOrDefault(a => a.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (existingAccount != null)
        {
            return false; // Account with this email already exists
        }

        int newId = _accountsLogic._accounts.Max(a => a.Id) + 1;
        var newAccount = new AccountModel(newId, email, password, fullName);
        _accountsLogic.UpdateList(newAccount);
        return true;
    }

    public AccountModel Login(string email, string password)
    {
        return _accountsLogic.CheckLogin(email, password);
    }

    public bool ManageAccount(int userId, string newEmail = null, string newPassword = null, string newFullName = null)
    {
        var account = _accountsLogic.GetById(userId);
        if (account == null)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(newEmail))
        {
            account.EmailAddress = newEmail;
        }
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            account.Password = newPassword;
        }
        if (!string.IsNullOrWhiteSpace(newFullName))
        {
            account.FullName = newFullName;
        }

        _accountsLogic.UpdateList(account);
        return true;
    }

    // Note: The following methods are placeholders and would need to be implemented
    // with actual flight booking data and logic.

    public List<FlightBooking> GetBookedFlights(int userId)
    {
        // Placeholder implementation
        return new List<FlightBooking>();
    }

    public bool CheckIn(int flightId)
    {
        // Placeholder implementation
        return true;
    }

    public bool ModifyBooking(int flightId, BookingDetails newDetails)
    {
        // Placeholder implementation
        return true;
    }
}

// Placeholder classes for flight-related functionality
public class FlightBooking
{
    public int FlightId { get; set; }
    public string FlightNumber { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
}

public class BookingDetails
{
    public string SeatNumber { get; set; }
    public bool HasCheckedBaggage { get; set; }
}