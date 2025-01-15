public static class AccountsAccess
{
    private static string _filePath =
        System.IO.Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"DataSources/accounts.json"));

    private static GenericJsonAccess<AccountModel> _accountAccess = new GenericJsonAccess<AccountModel>(_filePath);

    public static List<AccountModel> LoadAll()
    {
        return _accountAccess.LoadAll();
    }

    public static void WriteAll(List<AccountModel> accounts)
    {
        _accountAccess.WriteAll(accounts);
    }
}