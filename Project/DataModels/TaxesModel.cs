using System.Text.Json.Serialization;

public class Taxes
{
    public Taxes(double country, Dictionary<string, int> airport)
    {
        Country = country;
        Airport = airport;
    }

    [JsonPropertyName("country")] public double Country { get; set; }

    [JsonPropertyName("airport")]
    public Dictionary<string, int> Airport { get; set; } // Mapping van luchthaven-code naar specifieke belasting
}