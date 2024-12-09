using System.Text.Json.Serialization;

public class PaymentInformationModel
{
    [JsonPropertyName("cardHolder")]
    public string CardHolder { get; set; }

    [JsonPropertyName("cardNumber")]
    public string CardNumber { get; set; }

    [JsonPropertyName("cVV")]
    public string CVV { get; set; }

    [JsonPropertyName("expirationDate")]
    public string ExpirationDate { get; set; }

    public PaymentInformationModel(string cardHolder, string cardNumber, string cVV, string expirationDate)
    {
        CardHolder = cardHolder;
        CardNumber = cardNumber;
        CVV = cVV;
        ExpirationDate = expirationDate;
    }
}