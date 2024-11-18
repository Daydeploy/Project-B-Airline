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
