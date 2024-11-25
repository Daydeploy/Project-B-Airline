using System;
using System.Collections.Generic;
using System.Linq;

public class UserAccountService
{
    public AccountsLogic _accountsLogic;
    private List<BookingModel> _bookings;

    public bool IsLoggedIn { get; set; }
    public int CurrentUserId { get; set; }

    public UserAccountService()
    {
        _accountsLogic = new AccountsLogic();
        _bookings = BookingAccess.LoadAll();
    }

    public bool CreateAccount(string firstName, string lastName, string email, string password, DateTime dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            Console.WriteLine("Error: First name and last name must be filled.");
            return false;
        }

        while (true)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || !email.Contains("."))
            {
                Console.WriteLine("Error: Email must contain '@' and a domain (For instance: '.com').");
                Console.Write("Please enter your email address again: ");
                email = Console.ReadLine();
            }
            else
            {
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Error: Password must be filled.");
            return false;
        }

        // Check if an account with this email already exists
        var existingAccount = _accountsLogic._accounts
            .FirstOrDefault(a => a.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (existingAccount != null)
        {
            Console.WriteLine("Error: An account with this email already exists.");
            return false; // Return false if the account already exists
        }

        // Create a new unique ID for the account
        int newId = _accountsLogic._accounts.Max(a => a.Id) + 1;
        CurrentUserId = newId;

        // Create the new account with required properties
        var newAccount = new AccountModel(newId, firstName, lastName, dateOfBirth, email, password);

        // Update the list with the new account
        _accountsLogic.UpdateList(newAccount);

        return true;
    }


    public AccountModel Login(string email, string password)
    {
        // Set current user id
        CurrentUserId =
            _accountsLogic._accounts.FirstOrDefault(a => a.EmailAddress == email)?.Id ?? -1; // Set to -1 if not found
        var account = _accountsLogic.CheckLogin(email, password);
        if (account != null)
        {
            IsLoggedIn = true;
        }
        else
        {
            IsLoggedIn = false;
        }

        return account;
    }

    public bool ManageAccount(int userId, string newEmail = null, string newPassword = null, string newFirstName = null,
        string newLastName = null, string newGender = null, string newNationality = null,
        string newPhoneNumber = null, PassportDetailsModel newPassportDetails = null,
        DateTime? newDateOfBirth = null)
    {
        // Retrieve the account by user ID
        var account = _accountsLogic.GetById(userId);
        if (account == null)
        {
            return false; // Account not found
        }

        // Update email if provided
        if (!string.IsNullOrWhiteSpace(newEmail))
        {
            account.EmailAddress = newEmail;
        }

        // Update password if provided
        if (!string.IsNullOrWhiteSpace(newPassword))
        {
            account.Password = newPassword;
        }

        // Update first name if provided
        if (!string.IsNullOrWhiteSpace(newFirstName))
        {
            account.FirstName = newFirstName;
        }

        // Update last name if provided
        if (!string.IsNullOrWhiteSpace(newLastName))
        {
            account.LastName = newLastName;
        }

        // Update gender if provided
        if (!string.IsNullOrWhiteSpace(newGender))
        {
            account.Gender = newGender;
        }

        // Update nationality if provided
        if (!string.IsNullOrWhiteSpace(newNationality))
        {
            account.Nationality = newNationality;
        }

        // Update phone number if provided
        if (!string.IsNullOrWhiteSpace(newPhoneNumber))
        {
            account.PhoneNumber = newPhoneNumber;
        }

        // Update date of birth if provided
        if (newDateOfBirth.HasValue)
        {
            account.DateOfBirth = newDateOfBirth.Value;
        }

        // Update passport details if provided (replace or set specific fields)
        if (newPassportDetails != null)
        {
            if (account.PassportDetails == null)
            {
                account.PassportDetails = new PassportDetailsModel();
            }

            // Update individual passport details only if provided
            if (!string.IsNullOrWhiteSpace(newPassportDetails.PassportNumber))
            {
                account.PassportDetails.PassportNumber = newPassportDetails.PassportNumber;
            }

            if (newPassportDetails.IssueDate.HasValue)
            {
                account.PassportDetails.IssueDate = newPassportDetails.IssueDate;
            }

            if (newPassportDetails.ExpirationDate.HasValue)
            {
                account.PassportDetails.ExpirationDate = newPassportDetails.ExpirationDate;
            }

            if (!string.IsNullOrWhiteSpace(newPassportDetails.CountryOfIssue))
            {
                account.PassportDetails.CountryOfIssue = newPassportDetails.CountryOfIssue;
            }
        }

        // Save changes to the account list
        _accountsLogic.UpdateList(account);
        return true;
    }


    public List<FlightBooking> GetBookedFlights(int userId)
    {
        var userBookings = _bookings.Where(b => b.UserId == userId).ToList();
        var flightsLogic = new FlightsLogic();
        var flights = flightsLogic.GetAllFlights();

        return userBookings.Select(booking =>
        {
            var flight = flights.FirstOrDefault(f => f.FlightId == booking.FlightId);
            return new FlightBooking
            {
                FlightId = booking.FlightId,
                FlightNumber = flight?.FlightNumber ?? "Unknown",
                DepartureTime = flight?.DepartureTime ?? DateTime.MinValue.ToString(),
                ArrivalTime = flight?.ArrivalTime ?? DateTime.MinValue.ToString()
            };
        }).ToList();
    }

    public bool CheckIn(int flightId)
    {
        // For now, we'll just return true to simulate a successful check-in
        // In a real application, you'd want to update the booking status
        return true;
    }

    public bool ModifyBooking(int flightId, int passengerId, BookingDetails newDetails)
    {
        var booking = _bookings.FirstOrDefault(b => b.FlightId == flightId);
        if (booking == null || booking.Passengers == null || passengerId >= booking.Passengers.Count)
        {
            return false;
        }

        // Create a new passenger with updated details
        var oldPassenger = booking.Passengers[passengerId];
        var updatedPassenger = new PassengerModel(
            name: oldPassenger.Name, // Keep the original name
            seatNumber: newDetails.SeatNumber, // Update seat number
            hasCheckedBaggage: newDetails.HasCheckedBaggage // Update baggage status
        );

        // Replace the passenger in the list
        booking.Passengers[passengerId] = updatedPassenger;

        // Save the updated bookings
        BookingAccess.WriteAll(_bookings);

        return true;
    }

    public int GetCurrentMiles(int userId)
    {
        var account = AccountsLogic.CurrentAccount; // Assuming you have a way to get the current account
        if (account != null)
        {
            return account.Miles; // Ensure that the AccountModel has a 'Miles' property
        }
        return 0; // Return 0 if no account is found
    }
}

// Placeholder classes for flight-related functionality
public class FlightBooking
{
    public int FlightId { get; set; }
    public string FlightNumber { get; set; }
    public string DepartureTime { get; set; }
    public string ArrivalTime { get; set; }
}

public class BookingDetails
{
    public string SeatNumber { get; set; }
    public bool HasCheckedBaggage { get; set; }
}