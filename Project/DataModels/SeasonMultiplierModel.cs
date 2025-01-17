using System.Text.Json.Serialization;

public class SeasonalMultiplier
{
    public SeasonalMultiplier(double summer, double winter)
    {
        Summer = summer;
        Winter = winter;
    }

    [JsonPropertyName("summer")] public double Summer { get; set; }

    [JsonPropertyName("winter")] public double Winter { get; set; }
}