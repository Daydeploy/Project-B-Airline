using System.Text.Json.Serialization;

public class AccountModel
{
    public AccountModel(int id, string firstName, string lastName, DateTime dateOfBirth,
        string emailAddress, string password, string gender, string nationality,
        string phoneNumber, string address, PassportDetailsModel passportDetails,
        List<MilesModel> miles, List<PaymentInformationModel> paymentInformation = null)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        EmailAddress = emailAddress;
        Password = password;
        Gender = gender;
        Nationality = nationality;
        PhoneNumber = phoneNumber;
        Address = address;
        PassportDetails = passportDetails;
        Miles = miles;
        PaymentInformation = paymentInformation;
    }

    // Parameterless constructor for JSON deserialization
    public AccountModel()
    {
        Miles = new List<MilesModel>();
        PaymentInformation = new List<PaymentInformationModel>();
    }

    [JsonPropertyName("id")] public int Id { get; set; }

    [JsonPropertyName("firstName")] public string FirstName { get; set; }

    [JsonPropertyName("lastName")] public string LastName { get; set; }

    [JsonPropertyName("dateOfBirth")] public DateTime DateOfBirth { get; set; }

    [JsonPropertyName("gender")] public string Gender { get; set; }

    [JsonPropertyName("nationality")] public string Nationality { get; set; }

    [JsonPropertyName("phoneNumber")] public string PhoneNumber { get; set; }

    [JsonPropertyName("address")] public string Address { get; set; }

    [JsonPropertyName("email")] public string EmailAddress { get; set; }

    [JsonPropertyName("password")] public string Password { get; set; }

    [JsonPropertyName("passportDetails")] public PassportDetailsModel PassportDetails { get; set; }

    [JsonPropertyName("milesDetails")] public List<MilesModel> Miles { get; set; }

    [JsonPropertyName("paymentInformation")]
    public List<PaymentInformationModel> PaymentInformation { get; set; }

    [JsonPropertyName("privateJet")] public string PrivateJet { get; set; }

    [JsonPropertyName("shoppingCart")] public List<ShopItemModel> ShoppingCart { get; set; } = new();

    [JsonIgnore] public PaymentInformationModel TemporaryPaymentInfo { get; set; }
}