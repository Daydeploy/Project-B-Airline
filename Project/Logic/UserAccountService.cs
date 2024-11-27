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
        IsLoggedIn = false;
        CurrentUserId = -1;
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

        // Create miles list with default "Not Enrolled" status if no miles exist
        var initialMiles = new List<MilesModel> { new MilesModel(string.Empty, 0, 0, string.Empty) };

        // Create the new account with required properties and default miles status
        var newAccount = new AccountModel(newId, firstName, lastName, dateOfBirth, email, password, initialMiles);

        // Update the list with the new account
        _accountsLogic.UpdateList(newAccount);

        return true;
    }


    public AccountModel Login(string email, string password)
    {
        // Attempt to find the account
        var account = _accountsLogic.CheckLogin(email, password);

        if (account != null)
        {
            IsLoggedIn = true;
            CurrentUserId = account.Id;
            return account;
        }
        else
        {
            IsLoggedIn = false;
            CurrentUserId = -1;
            return null;
        }
    }

    public void Logout()
    {
        IsLoggedIn = false;
        CurrentUserId = -1;
    }

    public bool IsUserLoggedIn()
    {
        return IsLoggedIn;
    }

    public bool ManageAccount(int userId, string newEmail = null, string newPassword = null, string newFirstName = null,
        string newLastName = null, string newGender = null, string newNationality = null,
        string newPhoneNumber = null, PassportDetailsModel newPassportDetails = null,
        DateTime? newDateOfBirth = null, List<MilesModel> newMiles = null)
    {
        // Retrieve the account by user ID
        var account = _accountsLogic.GetById(userId);
        if (account == null)
        {
            return false; // Account not found
        }

        // Update fields if new values are provided
        if (!string.IsNullOrWhiteSpace(newEmail)) account.EmailAddress = newEmail;
        if (!string.IsNullOrWhiteSpace(newPassword)) account.Password = newPassword;
        if (!string.IsNullOrWhiteSpace(newFirstName)) account.FirstName = newFirstName;
        if (!string.IsNullOrWhiteSpace(newLastName)) account.LastName = newLastName;
        if (!string.IsNullOrWhiteSpace(newGender)) account.Gender = newGender;
        if (!string.IsNullOrWhiteSpace(newNationality)) account.Nationality = newNationality;
        if (!string.IsNullOrWhiteSpace(newPhoneNumber)) account.PhoneNumber = newPhoneNumber;
        if (newDateOfBirth.HasValue) account.DateOfBirth = newDateOfBirth.Value;

        if (newPassportDetails != null)
        {
            account.PassportDetails ??= new PassportDetailsModel();

            if (!string.IsNullOrWhiteSpace(newPassportDetails.PassportNumber))
                account.PassportDetails.PassportNumber = newPassportDetails.PassportNumber;

            if (newPassportDetails.IssueDate.HasValue)
                account.PassportDetails.IssueDate = newPassportDetails.IssueDate;

            if (newPassportDetails.ExpirationDate.HasValue)
                account.PassportDetails.ExpirationDate = newPassportDetails.ExpirationDate;

            if (!string.IsNullOrWhiteSpace(newPassportDetails.CountryOfIssue))
                account.PassportDetails.CountryOfIssue = newPassportDetails.CountryOfIssue;
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

        MilesLogic.UpdateFlightExperience(CurrentUserId);
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
            return account.Miles.Sum(m => m.Points); // Ensure that the AccountModel has a 'Miles' property
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