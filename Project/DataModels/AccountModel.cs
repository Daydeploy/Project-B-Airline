using System;
using System.Text.Json.Serialization;

public class AccountModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [JsonPropertyName("dateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    [JsonPropertyName("gender")]
    public string? Gender { get; set; }  // Nullable

    [JsonPropertyName("nationality")]
    public string? Nationality { get; set; }  // Nullable

    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }  // Nullable

    [JsonPropertyName("email")]
    public string EmailAddress { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("passportDetails")]
    public PassportDetailsModel? PassportDetails { get; set; }  // Nullable

    [JsonPropertyName("miles")]
    public int Miles { get; set; }

    public AccountModel(int id, string firstName, string lastName, DateTime dateOfBirth,
                        string emailAddress, string password, int miles = 0)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        EmailAddress = emailAddress;
        Password = password;
        Miles = miles;
    }
}

public class PassportDetailsModel
{
    [JsonPropertyName("passportNumber")]
    public string? PassportNumber { get; set; }  // Nullable

    [JsonPropertyName("issueDate")]
    public DateTime? IssueDate { get; set; }  // Nullable

    [JsonPropertyName("expirationDate")]
    public DateTime? ExpirationDate { get; set; }  // Nullable

    [JsonPropertyName("countryOfIssue")]
    public string? CountryOfIssue { get; set; }  // Nullable

    public PassportDetailsModel(string? passportNumber = null, DateTime? issueDate = null, 
                                DateTime? expirationDate = null, string? countryOfIssue = null)
    {
        PassportNumber = passportNumber;
        IssueDate = issueDate;
        ExpirationDate = expirationDate;
        CountryOfIssue = countryOfIssue;
    }
}
