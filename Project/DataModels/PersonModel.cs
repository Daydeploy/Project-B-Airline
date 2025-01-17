using System.Text.Json.Serialization;

public abstract class PersonModel : ContactInfoModel
{
    [JsonPropertyName("firstName")] public string FirstName { get; set; }

    [JsonPropertyName("lastName")] public string LastName { get; set; }

    [JsonPropertyName("dateOfBirth")] public DateTime DateOfBirth { get; set; }

    [JsonPropertyName("gender")] public string? Gender { get; set; }

    [JsonPropertyName("nationality")] public string? Nationality { get; set; }

    [JsonPropertyName("fullName")] public string FullName => $"{FirstName} {LastName}";
}