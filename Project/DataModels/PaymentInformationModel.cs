using System.Text.Json.Serialization;

public class PaymentInformationModel
{
    public PaymentInformationModel(string cardHolder, string cardNumber, string cVV, string expirationDate,
        string billingAddress)
    {
        CardHolder = cardHolder;
        CardNumber = cardNumber;
        CVV = cVV;
        ExpirationDate = expirationDate;
        BillingAddress = billingAddress;
    }

    [JsonPropertyName("cardHolder")] public string CardHolder { get; set; }

    [JsonPropertyName("cardNumber")] public string CardNumber { get; set; }

    [JsonPropertyName("cVV")] public string CVV { get; set; }

    [JsonPropertyName("expirationDate")] public string ExpirationDate { get; set; }

    [JsonPropertyName("billingAddress")] public string BillingAddress { get; set; }
}