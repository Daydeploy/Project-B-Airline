public class PaymentLogic
{
    private static readonly List<AccountModel> _accounts = AccountsAccess.LoadAll();

    public static bool ValidateName(string name)
    {
        if (!string.IsNullOrEmpty(name))
        {
            return true;
        }
        return false;
    }

    public static bool ValidateCardNumber(string cardNumber)
    {
        if (!string.IsNullOrEmpty(cardNumber))
        {
            if (cardNumber.Length == 16)
            {
                if (cardNumber.All(char.IsDigit))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool ValidateCVV(string cvv)
    {
        if (!string.IsNullOrEmpty(cvv))
        {
            if (cvv.Length == 3 || cvv.Length == 4)
            {
                if (cvv.All(char.IsDigit))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public static bool ValidateExpirationDate(string expirationDate)
    {
        if (string.IsNullOrEmpty(expirationDate))
        {
            return false;
        }

        var parts = expirationDate.Split('/');

        if (parts.Length != 2)
        {
            return false;
        }

        bool isValidMonth = int.TryParse(parts[0], out int month);
        bool isValidYear = int.TryParse(parts[1], out int year);

        if (!isValidMonth || !isValidYear)
        {
            return false;
        }

        if (month < 1 || month > 12)
        {
            return false;
        }

        year = 2000 + year;

        var expirationDateTime = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        if (expirationDateTime < DateTime.Now)
        {
            return false;
        }

        return true;
    }

    public static bool ValidateAddress(string address)
    {
        if (!string.IsNullOrEmpty(address))
        {
            return true;
        }
        return false;
    }

}
