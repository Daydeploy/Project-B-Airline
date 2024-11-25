using System.Text.Json.Serialization;

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

    public PassportDetailsModel(string? passportNumber = null, DateTime? issueDate = null, DateTime? expirationDate = null, string? countryOfIssue = null)
    {
        PassportNumber = passportNumber;
        IssueDate = issueDate;
        ExpirationDate = expirationDate;
        CountryOfIssue = countryOfIssue;
    }
}