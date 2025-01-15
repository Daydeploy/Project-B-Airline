public class UserAccountServiceLogic
{
    public AccountsLogic _accountsLogic;
    private readonly List<BookingModel> _bookings;

    public UserAccountServiceLogic()
    {
        _accountsLogic = new AccountsLogic();
        _bookings = BookingAccess.LoadAll();
        IsLoggedIn = false;
        CurrentUserId = -1;
    }

    public bool IsLoggedIn { get; set; }
    public int CurrentUserId { get; set; }

    public AccountModel CurrentAccount => IsLoggedIn ? _accountsLogic.GetById(CurrentUserId) : null;

    public bool CreateAccount(string firstName, string lastName, string email, string password,
        DateTime dateOfBirth, string gender, string nationality, string phoneNumber,
        string address, PassportDetailsModel passportDetails)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return false;
        }

        var existingAccount = _accountsLogic._accounts
            .FirstOrDefault(a => a.EmailAddress.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (existingAccount != null)
        {
            return false;
        }

        Console.WriteLine("\nWould you like to enroll in our Frequent Flyer Program? (Y/N)");
        var enrollResponse = Console.ReadLine()?.Trim().ToUpper();
        var isEnrolled = enrollResponse == "Y" || enrollResponse == "YES";

        var newId = _accountsLogic._accounts.Max(a => a.Id) + 1;
        CurrentUserId = newId;

        var initialMiles = new List<MilesModel>
            { new(string.Empty, 0, 0, string.Empty) { Enrolled = isEnrolled } };

        var newAccount = new AccountModel(
            newId,
            firstName,
            lastName,
            dateOfBirth,
            email,
            password,
            gender,
            nationality,
            phoneNumber,
            address,
            passportDetails,
            initialMiles
        );

        _accountsLogic.UpdateList(newAccount);

        if (isEnrolled)
            Console.WriteLine("Congratulations! You have been enrolled in the Frequent Flyer Program.");
        else
            Console.WriteLine("You have chosen not to enroll in the Frequent Flyer Program at this time.");

        return true;
    }

    public AccountModel Login(string email, string password)
    {
        var account = _accountsLogic.CheckLogin(email, password);

        if (account != null)
        {
            IsLoggedIn = true;
            CurrentUserId = account.Id;
            MilesLogic.UpdateAllAccountLevels();
            return account;
        }

        IsLoggedIn = false;
        CurrentUserId = -1;
        return null;
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
        string newPhoneNumber = null, string newAddress = null, PassportDetailsModel newPassportDetails = null,
        DateTime? newDateOfBirth = null, List<MilesModel> newMiles = null,
        List<PaymentInformationModel> newPaymentInformation = null)
    {
        var account = _accountsLogic.GetById(userId);
        if (account == null) return false;

        if (!string.IsNullOrWhiteSpace(newEmail)) account.EmailAddress = newEmail;
        if (!string.IsNullOrWhiteSpace(newPassword)) account.Password = newPassword;
        if (!string.IsNullOrWhiteSpace(newFirstName)) account.FirstName = newFirstName;
        if (!string.IsNullOrWhiteSpace(newLastName)) account.LastName = newLastName;
        if (!string.IsNullOrWhiteSpace(newGender)) account.Gender = newGender;
        if (!string.IsNullOrWhiteSpace(newNationality)) account.Nationality = newNationality;
        if (!string.IsNullOrWhiteSpace(newPhoneNumber)) account.PhoneNumber = newPhoneNumber;
        if (!string.IsNullOrWhiteSpace(newAddress)) account.Address = newAddress;
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

        if (newMiles != null) account.Miles = newMiles;

        if (newPaymentInformation != null) account.PaymentInformation = newPaymentInformation;

        _accountsLogic.UpdateList(account);
        return true;
    }

    public bool CheckIn(int flightId)
    {
        MilesLogic.UpdateFlightExperience(CurrentUserId);
        return true;
    }

    public bool ModifyBooking(int flightId, int passengerId, BookingDetails newDetails)
    {
        var booking = _bookings.FirstOrDefault(b => b.FlightId == flightId);
        if (booking == null || booking.Passengers == null || passengerId >= booking.Passengers.Count) return false;

        var oldPassenger = booking.Passengers[passengerId];
        var updatedPassenger = new PassengerModel(
            oldPassenger.Name,
            newDetails.SeatNumber,
            newDetails.HasCheckedBaggage
        );

        booking.Passengers[passengerId] = updatedPassenger;

        BookingAccess.WriteAll(_bookings);

        return true;
    }
}

public class BookingDetails
{
    public string SeatNumber { get; set; }
    public bool HasCheckedBaggage { get; set; }
}