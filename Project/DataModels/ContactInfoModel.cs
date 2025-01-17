using System.Text.Json.Serialization;

public abstract class ContactInfoModel : BaseModel
{
    [JsonPropertyName("phoneNumber")] public string? PhoneNumber { get; set; }

    [JsonPropertyName("address")] public string? Address { get; set; }
}