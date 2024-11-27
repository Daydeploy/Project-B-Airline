using System.Text.Json.Serialization;

public class AirportModel
{
    [JsonPropertyName("airportId")]
    public int AirportID { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }

    [JsonPropertyName("city")]
    public string City { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("phoneNumber")]
    public string PhoneNumber { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    public AirportModel(int airportID, string country, string city, string name, string code, string type,
        string phoneNumber, string address)
    {
        AirportID = airportID;
        Country = country;
        City = city;
        Name = name;
        Code = code;
        Type = type;
        PhoneNumber = phoneNumber;
        Address = address;
    }
}