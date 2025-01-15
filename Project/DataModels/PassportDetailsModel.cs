using System.Text.Json.Serialization;

public class PassportDetailsModel
{
    public PassportDetailsModel(string? passportNumber = null, DateTime? issueDate = null,
        DateTime? expirationDate = null, string? countryOfIssue = null)
    {
        PassportNumber = passportNumber;
        IssueDate = issueDate;
        ExpirationDate = expirationDate;
        CountryOfIssue = countryOfIssue;
    }

    [JsonPropertyName("passportNumber")] public string? PassportNumber { get; set; }

    [JsonPropertyName("issueDate")] public DateTime? IssueDate { get; set; }

    [JsonPropertyName("expirationDate")] public DateTime? ExpirationDate { get; set; }

    [JsonPropertyName("countryOfIssue")] public string? CountryOfIssue { get; set; }
}